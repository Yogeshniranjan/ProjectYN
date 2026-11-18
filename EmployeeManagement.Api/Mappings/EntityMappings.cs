using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Mappings;

public static class EntityMappings
{
    public static DepartmentResponse ToResponse(this Department department) =>
        new(
            department.DepartmentId,
            department.Name,
            department.Description,
            AsUtcOffset(department.CreatedDate),
            AsUtcOffset(department.ModifiedDate));

    public static EmployeeResponse ToResponse(this Employee employee) =>
        new(
            employee.EmployeeId,
            employee.DepartmentId,
            employee.Department.Name,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.PhoneNumber,
            employee.JobTitle,
            AsUtcOffset(employee.HireDate),
            employee.Salary,
            employee.IsActive,
            employee.Address is null
                ? null
                : new EmployeeAddressResponse(
                    employee.Address.AddressLine1,
                    employee.Address.AddressLine2,
                    employee.Address.City,
                    employee.Address.StateProvince,
                    employee.Address.PostalCode,
                    employee.Address.Country),
            AsUtcOffset(employee.CreatedDate),
            AsUtcOffset(employee.ModifiedDate));

    public static void ApplyAddress(Employee employee, EmployeeAddressRequest? request)
    {
        if (request is null)
        {
            employee.Address = null;
            return;
        }

        if (employee.Address is null)
        {
            employee.Address = new EmployeeAddress();
        }

        employee.Address.AddressLine1 = request.AddressLine1.Trim();
        employee.Address.AddressLine2 = request.AddressLine2?.Trim();
        employee.Address.City = request.City.Trim();
        employee.Address.StateProvince = request.StateProvince.Trim();
        employee.Address.PostalCode = request.PostalCode.Trim();
        employee.Address.Country = request.Country.Trim();
    }

    private static DateTimeOffset AsUtcOffset(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc));
}
