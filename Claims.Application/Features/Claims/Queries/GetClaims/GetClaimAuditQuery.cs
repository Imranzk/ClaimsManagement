
using Claims.Application.DTOs;
using MediatR;

namespace Claims.Application.Features.Claims.Queries.GetClaims;

public record GetClaimAuditQuery(Guid ClaimId)
    : IRequest<List<ClaimAuditDto>>;
