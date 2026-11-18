using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController(IDepartmentService departmentService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List all departments.")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DepartmentResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DepartmentResponse>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var departments = await departmentService.GetAllAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<DepartmentResponse>>(true, departments));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Get a department by ID.")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DepartmentResponse>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var department = await departmentService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponse<DepartmentResponse>(true, department));
    }

    [HttpPost]
    [EndpointSummary("Create a department.")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<DepartmentResponse>>> Create(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var department = await departmentService.CreateAsync(request, cancellationToken);
        var response = new ApiResponse<DepartmentResponse>(true, department, "Department created.");
        return CreatedAtAction(nameof(GetById), new { id = department.DepartmentId }, response);
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Replace a department.")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<DepartmentResponse>>> Update(
        int id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var department = await departmentService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponse<DepartmentResponse>(true, department, "Department updated."));
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Delete a department with no employees.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await departmentService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
