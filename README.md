# I am Yogesh Niranjan and this is my first project
Update on 18 November 2026

## Employee Management API

The ASP.NET Core Web API solution is organized by responsibility:

```text
EmployeeManagement.Api/
|-- Controllers/
|-- Data/
|-- DTOs/
|-- Interfaces/
|-- Mappings/
|-- Middleware/
|-- Models/
|-- Migrations/
|-- Repositories/
`-- Services/
```

### Run locally

Requirements: .NET 10 SDK and a running SQL Server default instance on `localhost`. The configured connection uses Windows authentication and trusts the local SQL Server certificate; no database credentials are stored in the repository. The Windows account running the API must have permission to access `EmployeeManagementDB`.

From the repository root in the VS Code terminal:

```powershell
dotnet tool restore
dotnet build EmployeeManagement.sln
dotnet run --project EmployeeManagement.Api --launch-profile http
```

In Development, the API applies pending EF Core migrations when it starts. The initial migration creates the schema and inserts repeatable sample data. Open the API document at `http://localhost:5046/openapi/v1.json`; send requests using the [API request collection](./EmployeeManagement.Api/EmployeeManagement.Api.http).

If SQL Server uses a different server or instance, override the connection string without editing committed configuration:

```powershell
$env:ConnectionStrings__EmployeeDatabase = "Server=YOUR_SERVER;Database=EmployeeManagementDB;Trusted_Connection=True;TrustServerCertificate=True"
```

### Database scripts and migrations

The SQL scripts create `EmployeeManagementDB`, its tables and constraints, and sample data:

1. Run [001_CreateDatabase.sql](./Database/Scripts/001_CreateDatabase.sql) in SQL Server Management Studio or with `sqlcmd`.
2. Run [002_SeedSampleData.sql](./Database/Scripts/002_SeedSampleData.sql) to insert sample departments, employees, and an address.

Alternatively, restore the local EF tool and apply migrations explicitly:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef database update --project EmployeeManagement.Api
```

The initial migration is safe to run against the database created by the SQL scripts; it preserves existing tables and sample rows. `ModifiedDate` defaults to the current UTC time on insert and is updated by the application on changes.

### API overview

- `GET /api/employees?page=1&pageSize=10&search=avery&departmentId=1` — paginated, searchable and department-filtered employee list.
- `GET`, `POST`, `PUT`, `DELETE /api/employees/{id}` — employee CRUD.
- `GET`, `POST`, `PUT`, `DELETE /api/departments/{id}` — department CRUD; a department with employees cannot be deleted.

Successful responses use a `{ "success": true, "data": ... }` envelope. Errors use Problem Details; duplicate emails and department names return `409 Conflict`, invalid request data returns `400 Bad Request`, and missing records return `404 Not Found`. Create operations return `201 Created` with a `Location` header; deletes return `204 No Content`.

Example employee request:

```json
{
  "departmentId": 1,
  "firstName": "Morgan",
  "lastName": "Reed",
  "email": "morgan.reed@example.com",
  "hireDate": "2025-03-10T00:00:00Z",
  "salary": 88000,
  "isActive": true
}
```

Example successful response:

```json
{
  "success": true,
  "data": {
    "employeeId": 3,
    "departmentId": 1,
    "departmentName": "Engineering",
    "firstName": "Morgan",
    "lastName": "Reed",
    "email": "morgan.reed@example.com",
    "phoneNumber": null,
    "jobTitle": null,
    "hireDate": "2025-03-10T00:00:00+00:00",
    "salary": 88000,
    "isActive": true,
    "address": null,
    "createdDate": "2026-11-18T10:00:00+00:00",
    "modifiedDate": "2026-11-18T10:00:00+00:00"
  },
  "message": "Employee created."
}
```
