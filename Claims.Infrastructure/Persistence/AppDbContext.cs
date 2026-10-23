
using Claims.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Claim> Claims => Set<Claim>();

    public DbSet<ClaimAudit> ClaimAudits => Set<ClaimAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.HasIndex(c => c.ClaimNumber)
                .IsUnique();

            entity.Property(c => c.ClaimNumber)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(c => c.ClaimantName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(c => c.ClaimantEmail)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(c => c.Market)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(c => c.IncidentDescription)
                .HasMaxLength(2000)
                .IsRequired();

            entity.Property(c => c.EstimatedAmount)
                .HasPrecision(18, 2);

            entity.Property(c => c.Status)
                .HasConversion<string>()
                .HasMaxLength(40);

            entity.Property(c => c.ClaimType)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.Property(c => c.AssignedOfficer)
                .HasMaxLength(150);

            entity.Property(c => c.AdditionalInformation)
                .HasMaxLength(4000);

            entity.Property(c => c.DecisionReason)
                .HasMaxLength(2000);

            entity.HasMany(c => c.AuditHistory)
                .WithOne(a => a.Claim)
                .HasForeignKey(a => a.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ClaimAudit>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Action)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(a => a.Details)
                .HasMaxLength(2000);

            entity.Property(a => a.PerformedBy)
                .HasMaxLength(150)
                .IsRequired();
        });
    }
}
