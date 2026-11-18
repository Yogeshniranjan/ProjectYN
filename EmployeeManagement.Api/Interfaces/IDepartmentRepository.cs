using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Interfaces;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken);

    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(string name, int? excludedDepartmentId, CancellationToken cancellationToken);

    Task<bool> HasEmployeesAsync(int id, CancellationToken cancellationToken);

    Task AddAsync(Department department, CancellationToken cancellationToken);

    void Remove(Department department);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
