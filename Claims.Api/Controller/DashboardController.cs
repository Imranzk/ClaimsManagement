
using Claims.Application.Features.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken)
    {
        var dashboard = await _sender.Send(
            new GetDashboardQuery(),
            cancellationToken);

        return Ok(dashboard);
    }
}
