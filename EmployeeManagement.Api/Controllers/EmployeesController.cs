using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/employees")]
public sealed class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List employees with pagination and optional search and department filters.")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<EmployeeResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResponse<EmployeeResponse>>>> GetAll(
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery, Range(1, int.MaxValue)] int? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        var employees = await employeeService.GetPageAsync(
            page,
            pageSize,
            search,
            departmentId,
            cancellationToken);
        return Ok(new ApiResponse<PagedResponse<EmployeeResponse>>(true, employees));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Get an employee by ID.")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EmployeeResponse>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var employee = await employeeService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponse<EmployeeResponse>(true, employee));
    }

    [HttpPost]
    [EndpointSummary("Create an employee.")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<EmployeeResponse>>> Create(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await employeeService.CreateAsync(request, cancellationToken);
        var response = new ApiResponse<EmployeeResponse>(true, employee, "Employee created.");
        return CreatedAtAction(nameof(GetById), new { id = employee.EmployeeId }, response);
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Replace an employee and their optional address.")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<EmployeeResponse>>> Update(
        int id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await employeeService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponse<EmployeeResponse>(true, employee, "Employee updated."));
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Delete an employee.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await employeeService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
