
using Claims.Domain.Enums;

namespace Claims.Domain.Entities;

public class Claim
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ClaimNumber { get; set; } = string.Empty;

    public string ClaimantName { get; set; } = string.Empty;

    public string ClaimantEmail { get; set; } = string.Empty;

    public string Market { get; set; } = string.Empty;

    public ClaimType ClaimType { get; set; }

    public DateTime IncidentDate { get; set; }

    public string IncidentDescription { get; set; } = string.Empty;

    public decimal EstimatedAmount { get; set; }

    public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;

    public string? AssignedOfficer { get; set; }

    public string? AdditionalInformation { get; set; }

    public string? DecisionReason { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<ClaimAudit> AuditHistory { get; set; } = new();
}
