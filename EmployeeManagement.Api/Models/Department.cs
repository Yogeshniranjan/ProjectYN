using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Models;

public class Department : IAuditableEntity
{
    public int DepartmentId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
