
namespace Claims.Application.DTOs;

public class DashboardDto
{
    public int TotalClaims { get; set; }

    public int SubmittedClaims { get; set; }

    public int UnderReviewClaims { get; set; }

    public int InformationRequestedClaims { get; set; }

    public int InformationProvidedClaims { get; set; }

    public int ApprovedClaims { get; set; }

    public int RejectedClaims { get; set; }

    public int SettledClaims { get; set; }

    public decimal OutstandingLiability { get; set; }

    public List<OfficerWorkloadDto> OfficerWorkloads { get; set; } = new();

    public List<ClaimDto> RecentClaims { get; set; } = new();
}

public class OfficerWorkloadDto
{
    public string OfficerName { get; set; } = string.Empty;

    public int AssignedClaims { get; set; }
}
