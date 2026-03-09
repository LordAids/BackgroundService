using SomeService.Data.Entities;

namespace SomeService.Services.Interfaces
{
    public interface IOfficeImportService
    {
        Task<List<Office>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddRangeAsync(List<Office> offices, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(List<Office> offices, CancellationToken cancellationToken = default);
    }
}
