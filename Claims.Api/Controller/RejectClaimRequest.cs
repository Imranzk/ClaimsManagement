
namespace Claims.Api.Controllers;

public class RejectClaimRequest
{
    public string DecisionReason { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}
