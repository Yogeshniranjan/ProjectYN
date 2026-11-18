using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs;

/// <summary>Request body for creating a department.</summary>
public sealed record CreateDepartmentRequest
{
    /// <summary>Department's unique name.</summary>
    [Required, StringLength(100)]
    public required string Name { get; init; }

    /// <summary>Optional description of the department.</summary>
    [StringLength(500)]
    public string? Description { get; init; }
}

/// <summary>Request body for replacing a department.</summary>
public sealed record UpdateDepartmentRequest
{
    /// <summary>Department's unique name.</summary>
    [Required, StringLength(100)]
    public required string Name { get; init; }

    /// <summary>Optional description of the department.</summary>
    [StringLength(500)]
    public string? Description { get; init; }
}

/// <summary>A department returned by the API.</summary>
/// <param name="DepartmentId">Unique department identifier.</param>
/// <param name="Name">Department name.</param>
/// <param name="Description">Department description, if provided.</param>
/// <param name="CreatedDate">UTC creation timestamp.</param>
/// <param name="ModifiedDate">UTC last-modified timestamp.</param>
public sealed record DepartmentResponse(
    int DepartmentId,
    string Name,
    string? Description,
    DateTimeOffset CreatedDate,
    DateTimeOffset ModifiedDate);
