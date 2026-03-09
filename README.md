# Справочник терминалов — BackgroundService

Фоновая служба для ежедневной загрузки справочника терминалов из JSON-файла и синхронизации данных с PostgreSQL. Разработана как часть информационной системы `SomeService`.

## Структура решения

```
BackgroundService/
├── BackgroundService/                   # Консольное приложение, точка входа
│   ├── DTOs/
│   │   └── TerminalsJsonDto.cs          # DTO для десериализации формата Dellin API
│   ├── Services/
│   │   ├── TerminalImportService.cs     # BackgroundService — оркестратор импорта
│   │   └── OfficeFileReader.cs          # Чтение JSON и маппинг в сущности
│   ├── appsettings.json
│   ├── appsettings.Development.json     # Локальные настройки (не в git)
│   └── Program.cs
├── SomeService.Data/                    # EF Core — сущности, DbContext, миграции
│   ├── Entities/
│   │   ├── BaseEntity.cs
│   │   ├── Office.cs
│   │   ├── Phone.cs
│   │   ├── Coordinates.cs
│   │   └── OfficeType.cs
│   ├── Migrations/
│   └── DellinDictionaryDbContext.cs
└── SomeService.Services/                # Бизнес-логика — может использоваться вне BackgroundService
    ├── Interfaces/
    │   └── IOfficeImportService.cs
    └── Services/
        └── OfficeImportService.cs
```

## Технический стек

- **Язык:** C# 13 (.NET 9)
- **Платформа:** Hosted Service (без Web API)
- **ORM:** Entity Framework Core 9 + Npgsql
- **DI:** Microsoft.Extensions.DependencyInjection
- **Логирование:** ILogger (структурированные логи в консоль)

## Требования

- .NET 9 SDK
- PostgreSQL 14+
- Файл `files/terminals.json` в формате Dellin API в корневой папке приложения

## Настройка

### 1. Конфигурация подключения

Создай `appsettings.Development.json` (не коммитится в git):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=dellin_dictionary;Username=postgres;Password=yourpassword"
  }
}
```

### 2. Путь к файлу

В `appsettings.json`:

```json
{
  "Import": {
    "FilePath": "files/terminals.json"
  }
}
```

### 3. Применение миграций

```powershell
Update-Database -Project SomeService.Data -StartupProject BackgroundService
```

### 4. Запуск

```bash
dotnet run --project BackgroundService
```

## Логика работы

1. При запуске сервис вычисляет задержку до **02:00 МСК** (23:00 UTC) и уходит в ожидание
2. В назначенное время читает `terminals.json`, десериализует через DTO и маппит в сущности `Office`
3. Сравнивает ID из файла с ID в БД:
   - Есть в файле, нет в БД → **добавить** (bulk insert через `ExecuteSqlRaw`)
   - Есть в БД, нет в файле → **удалить** (`DELETE ... WHERE id = ANY(ARRAY[...])`)
4. Все операции выполняются в транзакции — при ошибке данные остаются нетронутыми
5. После завершения сервис снова уходит в ожидание до следующего дня

## Формат входного файла

Файл соответствует формату ответа Dellin API:

```json
{
  "city": [
    {
      "cityID": 200601,
      "name": "Санкт-Петербург",
      "terminals": {
        "terminal": [
          {
            "id": "1",
            "fullAddress": "194292, Санкт-Петербург г, 1-й Верхний пер, дом № 12",
            "latitude": "60.063004",
            "longitude": "30.381796",
            "isPVZ": false,
            "calcSchedule": {
              "derival": "пн: 08:00-00:00; вт-пт: круглосуточно"
            },
            "phones": [
              { "number": "7 (812) 448-88-88", "comment": "" }
            ]
          }
        ]
      }
    }
  ]
}
```

## Маппинг полей

| Поле `Office` | Источник в JSON |
|---|---|
| `Id` | `terminal.id` (строка → int) |
| `CityCode` | `city.cityID` (nullable, при null → 0) |
| `Type` | `terminal.isPVZ` → `PVZ`, иначе `WAREHOUSE` |
| `WorkTime` | `terminal.calcSchedule.derival` |
| `AddressCity` | `city.name` |
| `AddressRegion` | `city.name` |
| `AddressStreet` | `terminal.fullAddress` |
| `Coordinates` | `terminal.latitude` + `terminal.longitude` (строки, InvariantCulture) |
| `Phones.PhoneNumber` | `terminal.phones[0].number` |
| `Phones.Additional` | `terminal.phones[0].comment` |

## Схема БД

**Таблица `Offices`:**

| Колонка | Тип | Описание |
|---|---|---|
| `id` | int | PK, из JSON |
| `Code` | text | Код терминала |
| `CityCode` | int | Код города |
| `Uuid` | text | UUID |
| `Type` | int | Тип (enum: PVZ=0, POSTAMAT=1, WAREHOUSE=2) |
| `CountryCode` | text | Код страны |
| `WorkTime` | text | Расписание работы |
| `AddressRegion` | text | Регион |
| `AddressCity` | text | Город |
| `AddressStreet` | text | Полный адрес |
| `AddressHouseNumber` | text | Номер дома |
| `AddressApartment` | int | Номер офиса |
| `Coordinates` | jsonb | Координаты `{Latitude, Longitude}` |

**Таблица `Phones`:**

| Колонка | Тип | Описание |
|---|---|---|
| `Id` | int | PK, autoincrement |
| `OfficeId` | int | FK → Offices.id (CASCADE DELETE) |
| `PhoneNumber` | text | Номер телефона |
| `Additional` | text | Комментарий |

**Индексы:** `ix_offices_city_code`, `ix_offices_address_city`, `ix_offices_code`, `ix_offices_uuid`
