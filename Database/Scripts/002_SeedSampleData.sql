USE EmployeeManagementDB;
GO

INSERT INTO dbo.Departments (Name, Description)
SELECT source.Name, source.Description
FROM
(
    VALUES
        (N'Engineering', N'Software development and technology.'),
        (N'Human Resources', N'People operations and employee support.')
) AS source (Name, Description)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Departments AS department
    WHERE department.Name = source.Name
);
GO

INSERT INTO dbo.Employees
(
    DepartmentId,
    FirstName,
    LastName,
    Email,
    PhoneNumber,
    JobTitle,
    HireDate,
    Salary
)
SELECT
    department.DepartmentId,
    source.FirstName,
    source.LastName,
    source.Email,
    source.PhoneNumber,
    source.JobTitle,
    source.HireDate,
    source.Salary
FROM
(
    VALUES
        (N'Avery', N'Chen', N'avery.chen@example.com', N'555-0101', N'Software Engineer', CONVERT(date, '20240115', 112), CONVERT(decimal(18,2), 95000.00), N'Engineering'),
        (N'Jordan', N'Patel', N'jordan.patel@example.com', N'555-0102', N'HR Specialist', CONVERT(date, '20230901', 112), CONVERT(decimal(18,2), 72000.00), N'Human Resources')
) AS source (FirstName, LastName, Email, PhoneNumber, JobTitle, HireDate, Salary, DepartmentName)
INNER JOIN dbo.Departments AS department
    ON department.Name = source.DepartmentName
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Employees AS employee
    WHERE employee.Email = source.Email
);
GO

INSERT INTO dbo.EmployeeAddresses
(
    EmployeeId,
    AddressLine1,
    City,
    StateProvince,
    PostalCode,
    Country
)
SELECT
    employee.EmployeeId,
    N'100 Main Street',
    N'Seattle',
    N'Washington',
    N'98101',
    N'United States'
FROM dbo.Employees AS employee
WHERE employee.Email = N'avery.chen@example.com'
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.EmployeeAddresses AS address
      WHERE address.EmployeeId = employee.EmployeeId
  );
GO
