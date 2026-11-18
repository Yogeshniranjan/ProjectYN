using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Models;

public class EmployeeAddress : IAuditableEntity
{
    public int EmployeeAddressId { get; set; }

    public int EmployeeId { get; set; }

    [Required]
    [StringLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string StateProvince { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public Employee Employee { get; set; } = null!;
}
