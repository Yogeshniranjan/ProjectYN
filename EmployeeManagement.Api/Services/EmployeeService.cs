using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Mappings;
using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Services;

public sealed class EmployeeService(IEmployeeRepository repository) : IEmployeeService
{
    public async Task<PagedResponse<EmployeeResponse>> GetPageAsync(
        int page,
        int pageSize,
        string? search,
        int? departmentId,
        CancellationToken cancellationToken)
    {
        if (page < 1)
        {
            throw new ArgumentException("Page must be greater than zero.", nameof(page));
        }

        if (pageSize is < 1 or > 100)
        {
            throw new ArgumentException("Page size must be between 1 and 100.", nameof(pageSize));
        }

        if (departmentId is <= 0)
        {
            throw new ArgumentException("Department ID must be greater than zero.", nameof(departmentId));
        }

        var (items, totalCount) = await repository.GetPageAsync(
            page,
            pageSize,
            search,
            departmentId,
            cancellationToken);

        return new PagedResponse<EmployeeResponse>(
            items.Select(employee => employee.ToResponse()).ToArray(),
            page,
            pageSize,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)pageSize));
    }

    public async Task<EmployeeResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await GetEmployeeOrThrowAsync(id, cancellationToken);
        return employee.ToResponse();
    }

    public async Task<EmployeeResponse> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        if (await repository.EmailExistsAsync(email, null, cancellationToken))
        {
            throw new ConflictException("An employee with this email address already exists.");
        }

        if (!await repository.DepartmentExistsAsync(request.DepartmentId, cancellationToken))
        {
            throw new ResourceNotFoundException("The selected department was not found.");
        }

        var employee = new Employee
        {
            DepartmentId = request.DepartmentId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            PhoneNumber = request.PhoneNumber?.Trim(),
            JobTitle = request.JobTitle?.Trim(),
            HireDate = request.HireDate.Date,
            Salary = request.Salary,
            IsActive = request.IsActive
        };

        EntityMappings.ApplyAddress(employee, request.Address);
        await repository.AddAsync(employee, cancellationToken);
        return (await GetEmployeeOrThrowAsync(employee.EmployeeId, cancellationToken)).ToResponse();
    }

    public async Task<EmployeeResponse> UpdateAsync(
        int id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await GetEmployeeOrThrowAsync(id, cancellationToken);
        var email = request.Email.Trim();

        if (await repository.EmailExistsAsync(email, id, cancellationToken))
        {
            throw new ConflictException("An employee with this email address already exists.");
        }

        if (!await repository.DepartmentExistsAsync(request.DepartmentId, cancellationToken))
        {
            throw new ResourceNotFoundException("The selected department was not found.");
        }

        employee.DepartmentId = request.DepartmentId;
        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = email;
        employee.PhoneNumber = request.PhoneNumber?.Trim();
        employee.JobTitle = request.JobTitle?.Trim();
        employee.HireDate = request.HireDate.Date;
        employee.Salary = request.Salary;
        employee.IsActive = request.IsActive;
        EntityMappings.ApplyAddress(employee, request.Address);

        await repository.SaveChangesAsync(cancellationToken);
        return employee.ToResponse();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await GetEmployeeOrThrowAsync(id, cancellationToken);
        repository.Remove(employee);
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Employee> GetEmployeeOrThrowAsync(int id, CancellationToken cancellationToken) =>
        await repository.GetByIdAsync(id, cancellationToken)
        ?? throw new ResourceNotFoundException($"Employee {id} was not found.");
}
