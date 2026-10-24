
using Claims.Application.DTOs;
using MediatR;

namespace Claims.Application.Features.Dashboard;

public record GetDashboardQuery : IRequest<DashboardDto>;
