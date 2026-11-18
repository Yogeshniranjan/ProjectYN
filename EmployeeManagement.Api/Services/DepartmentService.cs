using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Mappings;
using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Services;

public sealed class DepartmentService(IDepartmentRepository repository) : IDepartmentService
{
    public async Task<IReadOnlyList<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var departments = await repository.GetAllAsync(cancellationToken);
        return departments.Select(department => department.ToResponse()).ToArray();
    }

    public async Task<DepartmentResponse> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        (await GetDepartmentOrThrowAsync(id, cancellationToken)).ToResponse();

    public async Task<DepartmentResponse> CreateAsync(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await repository.NameExistsAsync(name, null, cancellationToken))
        {
            throw new ConflictException("A department with this name already exists.");
        }

        var department = new Department
        {
            Name = name,
            Description = request.Description?.Trim()
        };
        await repository.AddAsync(department, cancellationToken);
        return department.ToResponse();
    }

    public async Task<DepartmentResponse> UpdateAsync(
        int id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var department = await GetDepartmentOrThrowAsync(id, cancellationToken);
        var name = request.Name.Trim();

        if (await repository.NameExistsAsync(name, id, cancellationToken))
        {
            throw new ConflictException("A department with this name already exists.");
        }

        department.Name = name;
        department.Description = request.Description?.Trim();
        await repository.SaveChangesAsync(cancellationToken);
        return department.ToResponse();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var department = await GetDepartmentOrThrowAsync(id, cancellationToken);
        if (await repository.HasEmployeesAsync(id, cancellationToken))
        {
            throw new ConflictException("A department with employees cannot be deleted.");
        }

        repository.Remove(department);
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Department> GetDepartmentOrThrowAsync(int id, CancellationToken cancellationToken) =>
        await repository.GetByIdAsync(id, cancellationToken)
        ?? throw new ResourceNotFoundException($"Department {id} was not found.");
}
