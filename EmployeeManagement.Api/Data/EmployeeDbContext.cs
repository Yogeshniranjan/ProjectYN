using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

public sealed class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
    : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<EmployeeAddress> EmployeeAddresses => Set<EmployeeAddress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(department => department.DepartmentId);
            entity.Property(department => department.Name)
                .HasMaxLength(100)
                .IsRequired();
            entity.HasIndex(department => department.Name)
                .IsUnique()
                .HasDatabaseName("UQ_Departments_Name");
            entity.Property(department => department.Description).HasMaxLength(500);
            ConfigureAuditColumns(entity);

            entity.HasMany(department => department.Employees)
                .WithOne(employee => employee.Department)
                .HasForeignKey(employee => employee.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees", table =>
                table.HasCheckConstraint("CK_Employees_Salary_NonNegative", "[Salary] >= 0"));
            entity.HasKey(employee => employee.EmployeeId);
            entity.Property(employee => employee.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(employee => employee.LastName).HasMaxLength(100).IsRequired();
            entity.Property(employee => employee.Email).HasMaxLength(320).IsRequired();
            entity.HasIndex(employee => employee.Email)
                .IsUnique()
                .HasDatabaseName("UQ_Employees_Email");
            entity.HasIndex(employee => new
                {
                    employee.DepartmentId,
                    employee.LastName,
                    employee.FirstName
                })
                .HasDatabaseName("IX_Employees_DepartmentId_LastName_FirstName");
            entity.Property(employee => employee.PhoneNumber).HasMaxLength(30);
            entity.Property(employee => employee.JobTitle).HasMaxLength(100);
            entity.Property(employee => employee.HireDate).HasColumnType("date");
            entity.Property(employee => employee.Salary).HasPrecision(18, 2);
            entity.Property(employee => employee.IsActive).HasDefaultValue(true);
            ConfigureAuditColumns(entity);
        });

        modelBuilder.Entity<EmployeeAddress>(entity =>
        {
            entity.ToTable("EmployeeAddresses");
            entity.HasKey(address => address.EmployeeAddressId);
            entity.HasIndex(address => address.EmployeeId)
                .IsUnique()
                .HasDatabaseName("UQ_EmployeeAddresses_EmployeeId");
            entity.Property(address => address.AddressLine1).HasMaxLength(200).IsRequired();
            entity.Property(address => address.AddressLine2).HasMaxLength(200);
            entity.Property(address => address.City).HasMaxLength(100).IsRequired();
            entity.Property(address => address.StateProvince).HasMaxLength(100).IsRequired();
            entity.Property(address => address.PostalCode).HasMaxLength(20).IsRequired();
            entity.Property(address => address.Country).HasMaxLength(100).IsRequired();
            ConfigureAuditColumns(entity);

            entity.HasOne(address => address.Employee)
                .WithOne(employee => employee.Address)
                .HasForeignKey<EmployeeAddress>(address => address.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private static void ConfigureAuditColumns<TEntity>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> entity)
        where TEntity : class, IAuditableEntity
    {
        entity.Property(item => item.CreatedDate)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        entity.Property(item => item.ModifiedDate)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("SYSUTCDATETIME()");
    }

    private void ApplyAuditTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = now;
                entry.Entity.ModifiedDate = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(entity => entity.CreatedDate).IsModified = false;
                entry.Entity.ModifiedDate = now;
            }
        }
    }
}
