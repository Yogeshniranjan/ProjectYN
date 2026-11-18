using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Services;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<DepartmentResponse> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken);

    Task<DepartmentResponse> UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
