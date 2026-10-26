using Claims.Application.Features.Claims.Queries.GetClaims;
using Claims.Application.DTOs;
using Claims.Application.Features.Claims.Commands.CreateClaim;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Claims.Application.Features.Claims.Commands.AssignClaim;
using Claims.Application.Features.Claims.Commands.RequestInformation;
using Claims.Application.Features.Claims.Commands.ProvideInformation;
using Claims.Application.Features.Claims.Commands.ApproveClaim;
using Claims.Application.Features.Claims.Commands.RejectClaim;
using Claims.Application.Features.Claims.Commands.SettleClaim;

namespace Claims.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly ISender _sender;

    public ClaimsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateClaimDto request,
        CancellationToken cancellationToken)
    {
        var claim = await _sender.Send(
            new CreateClaimCommand(request),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = claim.Id },
            claim);
    }

   // GET: api/Claims
[HttpGet]
public async Task<IActionResult> GetAll(
    [FromQuery] string? status,
    [FromQuery] string? market,
    [FromQuery] string? assignedOfficer,
    [FromQuery] string? claimantEmail,
    CancellationToken cancellationToken)
{
    var claims = await _sender.Send(
        new GetClaimsQuery(status, market, assignedOfficer, claimantEmail),
        cancellationToken);

    return Ok(claims);
}

// GET: api/Claims/track/{claimNumber}
[HttpGet("track/{claimNumber}")]
public async Task<IActionResult> GetByClaimNumber(
    string claimNumber,
    CancellationToken cancellationToken)
{
    var claim = await _sender.Send(
        new GetClaimByClaimNumberQuery(claimNumber),
        cancellationToken);

    if (claim is null)
    {
        return NotFound(new
        {
            message = $"Claim with number '{claimNumber}' was not found."
        });
    }

    return Ok(claim);
}

// GET: api/Claims/{id}
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetById(
    Guid id,
    CancellationToken cancellationToken)
{
    var claim = await _sender.Send(
        new GetClaimByIdQuery(id),
        cancellationToken);

    if (claim is null)
    {
        return NotFound(new
        {
            message = $"Claim with ID '{id}' was not found."
        });
    }

    return Ok(claim);
}


[HttpPost("{id:guid}/assign")]
public async Task<IActionResult> Assign(
    Guid id,
    [FromBody] AssignClaimRequest request,
    CancellationToken cancellationToken)
{
    try
    {
        await _sender.Send(
            new AssignClaimCommand(
                id,
                request.AssignedOfficer,
                request.PerformedBy),
            cancellationToken);

        return Ok(new
        {
            message = "Claim assigned successfully."
        });
    }
    catch (KeyNotFoundException)
    {
        return NotFound(new { message = "Claim not found." });
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new { message = ex.Message });
    }
}

[HttpPost("{id:guid}/request-information")]
public async Task<IActionResult> RequestInformation(
    Guid id,
    [FromBody] RequestInformationRequest request,
    CancellationToken cancellationToken)
{
    try
    {
        await _sender.Send(
            new RequestInformationCommand(
                id,
                request.InformationRequest,
                request.PerformedBy),
            cancellationToken);

        return Ok(new
        {
            message = "Additional information requested successfully."
        });
    }
    catch (KeyNotFoundException)
    {
        return NotFound(new { message = "Claim not found." });
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new { message = ex.Message });
    }
}


[HttpPost("{id:guid}/provide-information")]
public async Task<IActionResult> ProvideInformation(
    Guid id,
    [FromBody] ProvideInformationRequest request,
    CancellationToken cancellationToken)
{
    try
    {
        await _sender.Send(
            new ProvideInformationCommand(
                id,
                request.AdditionalInformation,
                request.PerformedBy),
            cancellationToken);

        return Ok(new
        {
            message = "Additional information provided successfully."
        });
    }
    catch (KeyNotFoundException)
    {
        return NotFound(new { message = "Claim not found." });
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new { message = ex.Message });
    }
}


[HttpPost("{id:guid}/approve")]
public async Task<IActionResult> Approve(
    Guid id,
    [FromBody] ApproveClaimRequest request,
    CancellationToken cancellationToken)
{
    try
    {
        await _sender.Send(
            new ApproveClaimCommand(
                id,
                request.DecisionReason,
                request.PerformedBy),
            cancellationToken);

        return Ok(new
        {
            message = "Claim approved successfully."
        });
    }
    catch (KeyNotFoundException)
    {
        return NotFound(new { message = "Claim not found." });
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new { message = ex.Message });
    }
}


[HttpPost("{id:guid}/reject")]
public async Task<IActionResult> Reject(
    Guid id,
    [FromBody] RejectClaimRequest request,
    CancellationToken cancellationToken)
{
    try
    {
        await _sender.Send(
            new RejectClaimCommand(
                id,
                request.DecisionReason,
                request.PerformedBy),
            cancellationToken);

        return Ok(new
        {
            message = "Claim rejected successfully."
        });
    }
    catch (KeyNotFoundException)
    {
        return NotFound(new { message = "Claim not found." });
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new { message = ex.Message });
    }
}


[HttpPost("{id:guid}/settle")]
public async Task<IActionResult> Settle(
    Guid id,
    [FromBody] SettleClaimRequest request,
    CancellationToken cancellationToken)
{
    try
    {
        await _sender.Send(
            new SettleClaimCommand(
                id,
                request.PerformedBy),
            cancellationToken);

        return Ok(new
        {
            message = "Claim settled successfully."
        });
    }
    catch (KeyNotFoundException)
    {
        return NotFound(new
        {
            message = "Claim not found."
        });
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new
        {
            message = ex.Message
        });
    }
}


[HttpGet("{id:guid}/audit")]
public async Task<IActionResult> GetAudit(
    Guid id,
    CancellationToken cancellationToken)
{
    try
    {
        var auditHistory = await _sender.Send(
            new GetClaimAuditQuery(id),
            cancellationToken);

        return Ok(auditHistory);
    }
    catch (KeyNotFoundException)
    {
        return NotFound(new
        {
            message = "Claim not found."
        });
    }
}

}
