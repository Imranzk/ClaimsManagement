
using Claims.Application.DTOs;
using MediatR;

namespace Claims.Application.Features.Claims.Commands.CreateClaim;

public record CreateClaimCommand(CreateClaimDto Claim)
    : IRequest<ClaimDto>;
