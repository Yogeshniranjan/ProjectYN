namespace EmployeeManagement.Api.DTOs;

/// <summary>Standard success envelope for API responses.</summary>
/// <typeparam name="T">The response payload type.</typeparam>
public sealed record ApiResponse<T>(bool Success, T Data, string? Message = null);

/// <summary>A page of results with pagination metadata.</summary>
/// <typeparam name="T">The item type.</typeparam>
public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
