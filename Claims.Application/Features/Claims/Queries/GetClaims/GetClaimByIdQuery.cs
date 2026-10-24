
using Claims.Application.DTOs;
using MediatR;

namespace Claims.Application.Features.Claims.Queries.GetClaims;

public record GetClaimByIdQuery(Guid Id) : IRequest<ClaimDto?>;
