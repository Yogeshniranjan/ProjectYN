using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Repositories;

public sealed class EmployeeRepository(EmployeeDbContext dbContext) : IEmployeeRepository
{
    public async Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetPageAsync(
        int page,
        int pageSize,
        string? search,
        int? departmentId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Employees
            .AsNoTracking()
            .Include(employee => employee.Department)
            .Include(employee => employee.Address)
            .AsQueryable();

        if (departmentId.HasValue)
        {
            query = query.Where(employee => employee.DepartmentId == departmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(employee =>
                employee.FirstName.Contains(term)
                || employee.LastName.Contains(term)
                || employee.Email.Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .ThenBy(employee => employee.EmployeeId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Employees
            .Include(employee => employee.Department)
            .Include(employee => employee.Address)
            .SingleOrDefaultAsync(employee => employee.EmployeeId == id, cancellationToken);

    public Task<bool> EmailExistsAsync(
        string email,
        int? excludedEmployeeId,
        CancellationToken cancellationToken) =>
        dbContext.Employees.AnyAsync(
            employee => employee.Email == email
                && (!excludedEmployeeId.HasValue || employee.EmployeeId != excludedEmployeeId.Value),
            cancellationToken);

    public Task<bool> DepartmentExistsAsync(int departmentId, CancellationToken cancellationToken) =>
        dbContext.Departments.AnyAsync(
            department => department.DepartmentId == departmentId,
            cancellationToken);

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        await dbContext.Employees.AddAsync(employee, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);

    public void Remove(Employee employee) => dbContext.Employees.Remove(employee);
}
