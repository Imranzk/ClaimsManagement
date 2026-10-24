
using Claims.Application.DTOs;
using MediatR;

namespace Claims.Application.Features.Claims.Queries.GetClaims;

public record GetClaimsQuery(
    string? Status = null,
    string? Market = null,
    string? AssignedOfficer = null,
    string? ClaimantEmail = null
) : IRequest<List<ClaimDto>>;
