
namespace Claims.Application.DTOs;

public class ClaimAuditDto
{
    public Guid Id { get; set; }
    public Guid ClaimId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
