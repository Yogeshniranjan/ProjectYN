using EmployeeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagement.Api.Migrations;

[DbContext(typeof(EmployeeDbContext))]
public sealed class EmployeeDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.15")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        modelBuilder.Entity("EmployeeManagement.Api.Models.Department", entity =>
        {
            entity.Property<int>("DepartmentId")
                .ValueGeneratedOnAdd()
                .HasColumnType("int")
                .UseIdentityColumn();

            entity.Property<DateTime>("CreatedDate")
                .ValueGeneratedOnAdd()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property<string>("Description")
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            entity.Property<DateTime>("ModifiedDate")
                .ValueGeneratedOnAdd()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

            entity.HasKey("DepartmentId");
            entity.HasIndex("Name")
                .IsUnique()
                .HasDatabaseName("UQ_Departments_Name");
            entity.ToTable("Departments");
        });

        modelBuilder.Entity("EmployeeManagement.Api.Models.Employee", entity =>
        {
            entity.Property<int>("EmployeeId")
                .ValueGeneratedOnAdd()
                .HasColumnType("int")
                .UseIdentityColumn();

            entity.Property<DateTime>("CreatedDate")
                .ValueGeneratedOnAdd()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property<int>("DepartmentId").HasColumnType("int");
            entity.Property<string>("Email")
                .IsRequired()
                .HasMaxLength(320)
                .HasColumnType("nvarchar(320)");
            entity.Property<string>("FirstName")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");
            entity.Property<DateTime>("HireDate").HasColumnType("date");
            entity.Property<bool>("IsActive")
                .ValueGeneratedOnAdd()
                .HasColumnType("bit")
                .HasDefaultValue(true);
            entity.Property<string>("JobTitle")
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");
            entity.Property<string>("LastName")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");
            entity.Property<DateTime>("ModifiedDate")
                .ValueGeneratedOnAdd()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property<string>("PhoneNumber")
                .HasMaxLength(30)
                .HasColumnType("nvarchar(30)");
            entity.Property<decimal>("Salary")
                .HasPrecision(18, 2)
                .HasColumnType("decimal(18,2)");

            entity.HasKey("EmployeeId");
            entity.HasIndex("DepartmentId", "LastName", "FirstName")
                .HasDatabaseName("IX_Employees_DepartmentId_LastName_FirstName");
            entity.HasIndex("Email")
                .IsUnique()
                .HasDatabaseName("UQ_Employees_Email");
            entity.ToTable("Employees", table =>
                table.HasCheckConstraint("CK_Employees_Salary_NonNegative", "[Salary] >= 0"));
        });

        modelBuilder.Entity("EmployeeManagement.Api.Models.EmployeeAddress", entity =>
        {
            entity.Property<int>("EmployeeAddressId")
                .ValueGeneratedOnAdd()
                .HasColumnType("int")
                .UseIdentityColumn();

            entity.Property<string>("AddressLine1")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");
            entity.Property<string>("AddressLine2")
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");
            entity.Property<string>("City")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");
            entity.Property<string>("Country")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");
            entity.Property<DateTime>("CreatedDate")
                .ValueGeneratedOnAdd()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property<int>("EmployeeId").HasColumnType("int");
            entity.Property<string>("PostalCode")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("nvarchar(20)");
            entity.Property<string>("StateProvince")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");
            entity.Property<DateTime>("ModifiedDate")
                .ValueGeneratedOnAdd()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasKey("EmployeeAddressId");
            entity.HasIndex("EmployeeId")
                .IsUnique()
                .HasDatabaseName("UQ_EmployeeAddresses_EmployeeId");
            entity.ToTable("EmployeeAddresses");
        });

        modelBuilder.Entity("EmployeeManagement.Api.Models.Employee", entity =>
        {
            entity.HasOne("EmployeeManagement.Api.Models.Department", "Department")
                .WithMany("Employees")
                .HasForeignKey("DepartmentId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.Navigation("Department");
        });

        modelBuilder.Entity("EmployeeManagement.Api.Models.EmployeeAddress", entity =>
        {
            entity.HasOne("EmployeeManagement.Api.Models.Employee", "Employee")
                .WithOne("Address")
                .HasForeignKey("EmployeeManagement.Api.Models.EmployeeAddress", "EmployeeId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            entity.Navigation("Employee");
        });

        modelBuilder.Entity("EmployeeManagement.Api.Models.Department", entity =>
        {
            entity.Navigation("Employees");
        });

        modelBuilder.Entity("EmployeeManagement.Api.Models.Employee", entity =>
        {
            entity.Navigation("Address");
        });
#pragma warning restore 612, 618
    }
}
