
namespace Claims.Api.Controllers;

public class ProvideInformationRequest
{
    public string AdditionalInformation { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}
