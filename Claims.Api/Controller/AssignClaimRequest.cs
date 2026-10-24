using Claims.Application.Features.Claims.Commands.AssignClaim;
namespace Claims.Api.Controllers;

public class AssignClaimRequest
{
    public string AssignedOfficer { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}
