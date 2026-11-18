using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Interfaces;

public interface IEmployeeRepository
{
    Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetPageAsync(
        int page,
        int pageSize,
        string? search,
        int? departmentId,
        CancellationToken cancellationToken);

    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(string email, int? excludedEmployeeId, CancellationToken cancellationToken);

    Task<bool> DepartmentExistsAsync(int departmentId, CancellationToken cancellationToken);

    Task AddAsync(Employee employee, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    void Remove(Employee employee);
}
