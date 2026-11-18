using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs;

/// <summary>Address supplied when creating or replacing an employee.</summary>
public sealed record EmployeeAddressRequest
{
    /// <summary>First address line.</summary>
    [Required, StringLength(200)]
    public required string AddressLine1 { get; init; }

    /// <summary>Second address line, if applicable.</summary>
    [StringLength(200)]
    public string? AddressLine2 { get; init; }

    /// <summary>City or locality.</summary>
    [Required, StringLength(100)]
    public required string City { get; init; }

    /// <summary>State or province.</summary>
    [Required, StringLength(100)]
    public required string StateProvince { get; init; }

    /// <summary>Postal or ZIP code.</summary>
    [Required, StringLength(20)]
    public required string PostalCode { get; init; }

    /// <summary>Country.</summary>
    [Required, StringLength(100)]
    public required string Country { get; init; }
}

/// <summary>Request body for creating an employee.</summary>
public sealed record CreateEmployeeRequest
{
    /// <summary>Department to which the employee belongs.</summary>
    [Range(1, int.MaxValue)]
    public int DepartmentId { get; init; }

    /// <summary>Employee's first name.</summary>
    [Required, StringLength(100)]
    public required string FirstName { get; init; }

    /// <summary>Employee's last name.</summary>
    [Required, StringLength(100)]
    public required string LastName { get; init; }

    /// <summary>Employee's unique email address.</summary>
    [Required, EmailAddress, StringLength(320)]
    public required string Email { get; init; }

    /// <summary>Optional phone number.</summary>
    [StringLength(30)]
    public string? PhoneNumber { get; init; }

    /// <summary>Optional job title.</summary>
    [StringLength(100)]
    public string? JobTitle { get; init; }

    /// <summary>Employee hire date and offset.</summary>
    public DateTimeOffset HireDate { get; init; }

    /// <summary>Non-negative annual salary.</summary>
    [Range(typeof(decimal), "0", "9999999999999999.99")]
    public decimal Salary { get; init; }

    /// <summary>Whether the employee is active.</summary>
    public bool IsActive { get; init; } = true;

    /// <summary>Optional employee address.</summary>
    public EmployeeAddressRequest? Address { get; init; }
}

/// <summary>Request body for replacing an employee.</summary>
public sealed record UpdateEmployeeRequest
{
    /// <summary>Department to which the employee belongs.</summary>
    [Range(1, int.MaxValue)]
    public int DepartmentId { get; init; }

    /// <summary>Employee's first name.</summary>
    [Required, StringLength(100)]
    public required string FirstName { get; init; }

    /// <summary>Employee's last name.</summary>
    [Required, StringLength(100)]
    public required string LastName { get; init; }

    /// <summary>Employee's unique email address.</summary>
    [Required, EmailAddress, StringLength(320)]
    public required string Email { get; init; }

    /// <summary>Optional phone number.</summary>
    [StringLength(30)]
    public string? PhoneNumber { get; init; }

    /// <summary>Optional job title.</summary>
    [StringLength(100)]
    public string? JobTitle { get; init; }

    /// <summary>Employee hire date and offset.</summary>
    public DateTimeOffset HireDate { get; init; }

    /// <summary>Non-negative annual salary.</summary>
    [Range(typeof(decimal), "0", "9999999999999999.99")]
    public decimal Salary { get; init; }

    /// <summary>Whether the employee is active.</summary>
    public bool IsActive { get; init; }

    /// <summary>Employee address; null removes an existing address.</summary>
    public EmployeeAddressRequest? Address { get; init; }
}

/// <summary>An employee address returned by the API.</summary>
/// <param name="AddressLine1">First address line.</param>
/// <param name="AddressLine2">Second address line, if present.</param>
/// <param name="City">City or locality.</param>
/// <param name="StateProvince">State or province.</param>
/// <param name="PostalCode">Postal or ZIP code.</param>
/// <param name="Country">Country.</param>
public sealed record EmployeeAddressResponse(
    string AddressLine1,
    string? AddressLine2,
    string City,
    string StateProvince,
    string PostalCode,
    string Country);

/// <summary>An employee returned by the API.</summary>
/// <param name="EmployeeId">Unique employee identifier.</param>
/// <param name="DepartmentId">Owning department identifier.</param>
/// <param name="DepartmentName">Owning department name.</param>
/// <param name="FirstName">Employee's first name.</param>
/// <param name="LastName">Employee's last name.</param>
/// <param name="Email">Employee's email address.</param>
/// <param name="PhoneNumber">Employee's phone number, if present.</param>
/// <param name="JobTitle">Employee's job title, if present.</param>
/// <param name="HireDate">Employee hire date as a UTC timestamp.</param>
/// <param name="Salary">Employee salary.</param>
/// <param name="IsActive">Whether the employee is active.</param>
/// <param name="Address">Employee's address, if present.</param>
/// <param name="CreatedDate">UTC creation timestamp.</param>
/// <param name="ModifiedDate">UTC last-modified timestamp.</param>
public sealed record EmployeeResponse(
    int EmployeeId,
    int DepartmentId,
    string DepartmentName,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? JobTitle,
    DateTimeOffset HireDate,
    decimal Salary,
    bool IsActive,
    EmployeeAddressResponse? Address,
    DateTimeOffset CreatedDate,
    DateTimeOffset ModifiedDate);
