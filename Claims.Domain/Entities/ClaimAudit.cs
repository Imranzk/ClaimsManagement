using System;
using Claims.Domain.Enums;
namespace Claims.Domain.Entities;

public class ClaimAudit
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ClaimId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? Details { get; set; }

    public string PerformedBy { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Claim? Claim { get; set; }
}
