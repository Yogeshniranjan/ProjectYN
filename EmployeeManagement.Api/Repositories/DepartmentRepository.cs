using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Repositories;

public sealed class DepartmentRepository(EmployeeDbContext dbContext) : IDepartmentRepository
{
    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Departments
            .AsNoTracking()
            .OrderBy(department => department.Name)
            .ToListAsync(cancellationToken);

    public Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Departments.SingleOrDefaultAsync(
            department => department.DepartmentId == id,
            cancellationToken);

    public Task<bool> NameExistsAsync(
        string name,
        int? excludedDepartmentId,
        CancellationToken cancellationToken) =>
        dbContext.Departments.AnyAsync(
            department => department.Name == name
                && (!excludedDepartmentId.HasValue || department.DepartmentId != excludedDepartmentId.Value),
            cancellationToken);

    public Task<bool> HasEmployeesAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Employees.AnyAsync(employee => employee.DepartmentId == id, cancellationToken);

    public async Task AddAsync(Department department, CancellationToken cancellationToken)
    {
        await dbContext.Departments.AddAsync(department, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public void Remove(Department department) => dbContext.Departments.Remove(department);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
