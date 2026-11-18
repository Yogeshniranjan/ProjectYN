using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Services;

public interface IEmployeeService
{
    Task<PagedResponse<EmployeeResponse>> GetPageAsync(
        int page,
        int pageSize,
        string? search,
        int? departmentId,
        CancellationToken cancellationToken);

    Task<EmployeeResponse> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken);

    Task<EmployeeResponse> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
