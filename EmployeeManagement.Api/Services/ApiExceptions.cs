namespace EmployeeManagement.Api.Services;

public sealed class ResourceNotFoundException(string message) : Exception(message)
{
}

public sealed class ConflictException(string message) : Exception(message)
{
}
