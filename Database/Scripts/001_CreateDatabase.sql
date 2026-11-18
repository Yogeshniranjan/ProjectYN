IF DB_ID(N'EmployeeManagementDB') IS NULL
BEGIN
    CREATE DATABASE EmployeeManagementDB;
END;
GO

USE EmployeeManagementDB;
GO

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
        CONSTRAINT PK_Departments PRIMARY KEY CLUSTERED (DepartmentId),
        CONSTRAINT UQ_Departments_Name UNIQUE (Name)
    );
END;
GO

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
        CONSTRAINT UQ_Employees_Email UNIQUE (Email),
        CONSTRAINT CK_Employees_Salary_NonNegative CHECK (Salary >= 0),
        CONSTRAINT FK_Employees_Departments
            FOREIGN KEY (DepartmentId) REFERENCES dbo.Departments (DepartmentId)
            ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Employees_DepartmentId_LastName_FirstName'
      AND object_id = OBJECT_ID(N'dbo.Employees')
)
BEGIN
    CREATE INDEX IX_Employees_DepartmentId_LastName_FirstName
        ON dbo.Employees (DepartmentId, LastName, FirstName);
END;
GO

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
GO
