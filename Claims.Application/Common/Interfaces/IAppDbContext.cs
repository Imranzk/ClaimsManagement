
using Claims.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Claim> Claims { get; }
    DbSet<ClaimAudit> ClaimAudits { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
