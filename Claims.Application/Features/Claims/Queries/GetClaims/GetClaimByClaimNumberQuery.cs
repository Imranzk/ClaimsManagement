using Claims.Application.DTOs;
using MediatR;

namespace Claims.Application.Features.Claims.Queries.GetClaims;

public record GetClaimByClaimNumberQuery(string ClaimNumber)
    : IRequest<ClaimDto?>;
