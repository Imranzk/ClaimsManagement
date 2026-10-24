
using Claims.Domain.Enums;

namespace Claims.Application.DTOs;

public class CreateClaimDto
{
    public string ClaimantName { get; set; } = string.Empty;
    public string ClaimantEmail { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
    public ClaimType ClaimType { get; set; }
    public DateTime IncidentDate { get; set; }
    public string IncidentDescription { get; set; } = string.Empty;
    public decimal EstimatedAmount { get; set; }
}
