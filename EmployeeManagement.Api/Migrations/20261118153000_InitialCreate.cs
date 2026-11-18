using EmployeeManagement.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagement.Api.Migrations;

[DbContext(typeof(EmployeeDbContext))]
[Migration("20261118153000_InitialCreate")]
public sealed class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DECLARE @CreatedSchema bit =
                CASE WHEN OBJECT_ID(N'dbo.Departments', N'U') IS NULL
                       AND OBJECT_ID(N'dbo.Employees', N'U') IS NULL
                       AND OBJECT_ID(N'dbo.EmployeeAddresses', N'U') IS NULL
                     THEN 1 ELSE 0 END;

            IF OBJECT_ID(N'dbo.__EmployeeManagementInitialMigration', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.__EmployeeManagementInitialMigration
                (
                    MigrationId tinyint NOT NULL
                        CONSTRAINT PK_EmployeeManagementInitialMigration PRIMARY KEY,
                    CreatedSchema bit NOT NULL
                );
                INSERT INTO dbo.__EmployeeManagementInitialMigration (MigrationId, CreatedSchema)
                VALUES (1, @CreatedSchema);
            END;

            IF OBJECT_ID(N'dbo.Departments', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Departments
                (
                    DepartmentId int IDENTITY(1,1) NOT NULL,
                    Name nvarchar(100) NOT NULL,
                    Description nvarchar(500) NULL,
                    CreatedDate datetime2(7) NOT NULL
                        CONSTRAINT DF_Departments_CreatedDate DEFAULT (SYSUTCDATETIME()),
                    ModifiedDate datetime2(7) NOT NULL
                        CONSTRAINT DF_Departments_ModifiedDate DEFAULT (SYSUTCDATETIME()),
                    CONSTRAINT PK_Departments PRIMARY KEY CLUSTERED (DepartmentId)
                );
                CREATE UNIQUE INDEX UQ_Departments_Name ON dbo.Departments (Name);
            END;

            IF OBJECT_ID(N'dbo.Employees', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Employees
                (
                    EmployeeId int IDENTITY(1,1) NOT NULL,
                    DepartmentId int NOT NULL,
                    FirstName nvarchar(100) NOT NULL,
                    LastName nvarchar(100) NOT NULL,
                    Email nvarchar(320) NOT NULL,
                    PhoneNumber nvarchar(30) NULL,
                    JobTitle nvarchar(100) NULL,
                    HireDate date NOT NULL,
                    Salary decimal(18,2) NOT NULL,
                    IsActive bit NOT NULL
                        CONSTRAINT DF_Employees_IsActive DEFAULT (1),
                    CreatedDate datetime2(7) NOT NULL
                        CONSTRAINT DF_Employees_CreatedDate DEFAULT (SYSUTCDATETIME()),
                    ModifiedDate datetime2(7) NOT NULL
                        CONSTRAINT DF_Employees_ModifiedDate DEFAULT (SYSUTCDATETIME()),
                    CONSTRAINT PK_Employees PRIMARY KEY CLUSTERED (EmployeeId),
                    CONSTRAINT CK_Employees_Salary_NonNegative CHECK (Salary >= 0),
                    CONSTRAINT FK_Employees_Departments
                        FOREIGN KEY (DepartmentId) REFERENCES dbo.Departments (DepartmentId)
                        ON DELETE NO ACTION
                );
                CREATE UNIQUE INDEX UQ_Employees_Email ON dbo.Employees (Email);
                CREATE INDEX IX_Employees_DepartmentId_LastName_FirstName
                    ON dbo.Employees (DepartmentId, LastName, FirstName);
            END;

            IF OBJECT_ID(N'dbo.EmployeeAddresses', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.EmployeeAddresses
                (
                    EmployeeAddressId int IDENTITY(1,1) NOT NULL,
                    EmployeeId int NOT NULL,
                    AddressLine1 nvarchar(200) NOT NULL,
                    AddressLine2 nvarchar(200) NULL,
                    City nvarchar(100) NOT NULL,
                    StateProvince nvarchar(100) NOT NULL,
                    PostalCode nvarchar(20) NOT NULL,
                    Country nvarchar(100) NOT NULL,
                    CreatedDate datetime2(7) NOT NULL
                        CONSTRAINT DF_EmployeeAddresses_CreatedDate DEFAULT (SYSUTCDATETIME()),
                    ModifiedDate datetime2(7) NOT NULL
                        CONSTRAINT DF_EmployeeAddresses_ModifiedDate DEFAULT (SYSUTCDATETIME()),
                    CONSTRAINT PK_EmployeeAddresses PRIMARY KEY CLUSTERED (EmployeeAddressId),
                    CONSTRAINT UQ_EmployeeAddresses_EmployeeId UNIQUE (EmployeeId),
                    CONSTRAINT FK_EmployeeAddresses_Employees
                        FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees (EmployeeId)
                        ON DELETE CASCADE
                );
            END;

            IF NOT EXISTS
            (
                SELECT 1 FROM dbo.Departments WHERE Name = N'Engineering'
            )
            BEGIN
                INSERT INTO dbo.Departments (Name, Description)
                VALUES (N'Engineering', N'Software development and technology.');
            END;
            IF NOT EXISTS
            (
                SELECT 1 FROM dbo.Departments WHERE Name = N'Human Resources'
            )
            BEGIN
                INSERT INTO dbo.Departments (Name, Description)
                VALUES (N'Human Resources', N'People operations and employee support.');
            END;

            IF NOT EXISTS
            (
                SELECT 1 FROM dbo.Employees WHERE Email = N'avery.chen@example.com'
            )
            BEGIN
                INSERT INTO dbo.Employees
                    (DepartmentId, FirstName, LastName, Email, PhoneNumber, JobTitle, HireDate, Salary)
                SELECT DepartmentId, N'Avery', N'Chen', N'avery.chen@example.com',
                    N'555-0101', N'Software Engineer', CONVERT(date, '20240115', 112), 95000.00
                FROM dbo.Departments WHERE Name = N'Engineering';
            END;
            IF NOT EXISTS
            (
                SELECT 1 FROM dbo.Employees WHERE Email = N'jordan.patel@example.com'
            )
            BEGIN
                INSERT INTO dbo.Employees
                    (DepartmentId, FirstName, LastName, Email, PhoneNumber, JobTitle, HireDate, Salary)
                SELECT DepartmentId, N'Jordan', N'Patel', N'jordan.patel@example.com',
                    N'555-0102', N'HR Specialist', CONVERT(date, '20230901', 112), 72000.00
                FROM dbo.Departments WHERE Name = N'Human Resources';
            END;

            IF NOT EXISTS
            (
                SELECT 1
                FROM dbo.EmployeeAddresses AS address
                INNER JOIN dbo.Employees AS employee ON employee.EmployeeId = address.EmployeeId
                WHERE employee.Email = N'avery.chen@example.com'
            )
            BEGIN
                INSERT INTO dbo.EmployeeAddresses
                    (EmployeeId, AddressLine1, City, StateProvince, PostalCode, Country)
                SELECT EmployeeId, N'100 Main Street', N'Seattle', N'Washington',
                    N'98101', N'United States'
                FROM dbo.Employees WHERE Email = N'avery.chen@example.com';
            END;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF EXISTS
            (
                SELECT 1
                FROM dbo.__EmployeeManagementInitialMigration
                WHERE MigrationId = 1 AND CreatedSchema = 1
            )
            BEGIN
                DROP TABLE IF EXISTS dbo.EmployeeAddresses;
                DROP TABLE IF EXISTS dbo.Employees;
                DROP TABLE IF EXISTS dbo.Departments;
            END;

            DROP TABLE IF EXISTS dbo.__EmployeeManagementInitialMigration;
            """);
    }
}
