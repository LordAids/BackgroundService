# Справочник терминалов — BackgroundService

Фоновая служба для периодической загрузки справочника терминалов из JSON-файла и синхронизации данных с PostgreSQL. Разработана как часть информационной системы `SomeService`.

## Структура решения

```
BackgroundService/
├── BackgroundService/               # Консольное приложение, точка входа
│   ├── Services/
│   │   ├── TerminalImportService.cs # BackgroundService — оркестратор импорта
│   │   └── OfficeFileReader.cs      # Чтение и десериализация JSON
│   ├── appsettings.json
│   ├── appsettings.Development.json # Локальные настройки (не в git)
│   └── Program.cs
├── SomeService.Data/                # EF Core — сущности, DbContext, миграции
│   ├── Entities/
│   │   ├── Office.cs
│   │   ├── Address.cs
│   │   ├── Coordinates.cs
│   │   ├── Phone.cs
│   │   ├── OfficeType.cs
│   │   └── BaseEntity.cs
│   ├── Migrations/
│   └── DellinDictionaryDbContext.cs
└── SomeService.Services/            # Бизнес-логика — может использоваться вне BackgroundService
    ├── Interfaces/
    │   └── IOfficeImportService.cs
    └── Services/
        └── OfficeImportService.cs
```

## Технический стек

- **Язык:** C# 13 (.NET 9)
- **Платформа:** Hosted Service (без Web API)
- **ORM:** Entity Framework Core 9 + Npgsql
- **Bulk-операции:** EFCore.BulkExtensions.PostgreSql
- **DI:** Microsoft.Extensions.DependencyInjection
- **Логирование:** ILogger (структурированные логи в консоль)

## Требования

- .NET 9 SDK
- PostgreSQL 14+
- Доступ к файлу `files/terminals.json` в корневой папке приложения

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

### 2. Настройка расписания и пути к файлу

В `appsettings.json`:

```json
{
  "Import": {
    "FilePath": "files/terminals.json"
  }
}
```

### 3. Применение миграций

```bash
dotnet ef database update --project SomeService.Data --startup-project BackgroundService
```

### 4. Запуск

```bash
dotnet run --project BackgroundService
```

## Логика работы

1. При запуске сервис вычисляет задержку до **02:00 МСК** (23:00 UTC) и уходит в ожидание
2. В назначенное время читает `terminals.json` и десериализует список терминалов
3. Сравнивает ID из файла с ID в БД:
   - Есть в файле, нет в БД → **добавить**
   - Есть в БД, нет в файле → **удалить**
4. Операции выполняются в транзакции — при ошибке данные в БД остаются нетронутыми
5. После завершения сервис снова уходит в ожидание до следующего дня

