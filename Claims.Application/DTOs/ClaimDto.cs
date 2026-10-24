
using Claims.Domain.Enums;

namespace Claims.Application.DTOs;

public class ClaimDto
{
    public Guid Id { get; set; }
    public string ClaimNumber { get; set; } = string.Empty;
    public string ClaimantName { get; set; } = string.Empty;
    public string ClaimantEmail { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
    public ClaimType ClaimType { get; set; }
    public DateTime IncidentDate { get; set; }
    public string IncidentDescription { get; set; } = string.Empty;
    public decimal EstimatedAmount { get; set; }
    public ClaimStatus Status { get; set; }
    public string? AssignedOfficer { get; set; }
    public string? AdditionalInformation { get; set; }
    public string? DecisionReason { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
