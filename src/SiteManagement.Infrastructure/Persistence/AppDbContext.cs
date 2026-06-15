using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SiteManagement.Domain.Entities;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Domain.Entities;

namespace SiteManagement.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<ApartmentUnit> ApartmentUnits => Set<ApartmentUnit>();
    public DbSet<BuildingUser> BuildingUsers => Set<BuildingUser>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<Fee> Fees => Set<Fee>();
    public DbSet<FeePayment> FeePayments => Set<FeePayment>();
    public DbSet<FaultReport> FaultReports => Set<FaultReport>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Building>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.JoinCode).HasMaxLength(20).IsRequired();
            b.HasIndex(x => x.JoinCode).IsUnique();
        });
        builder.Entity<BuildingUser>(b =>
        {
            b.Property(x => x.MemberType).HasMaxLength(20).IsRequired();
            b.HasIndex(x => new { x.BuildingId, x.UserId }).IsUnique();
        });
        builder.Entity<Announcement>(b =>
        {
            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.Content).HasMaxLength(4000).IsRequired();
            b.HasIndex(x => x.BuildingId);
        });

        builder.Entity<Fee>(b =>
        {
            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.BaseAmount).HasColumnType("TEXT");
            b.HasIndex(x => x.BuildingId);

            // TPH discriminator (FeeType kelimesi yok)
            b.HasDiscriminator<string>("FeeDiscriminator")
                .HasValue<NormalFee>("Normal")
                .HasValue<LateFee>("Late")
                .HasValue<SpecialFee>("Special");
        });

        builder.Entity<LateFee>(b =>
        {
            b.Property(x => x.PenaltyRate).HasColumnType("TEXT");
        });

        builder.Entity<SpecialFee>(b =>
        {
            b.Property(x => x.Note).HasMaxLength(300);
        });

        builder.Entity<FeePayment>(b =>
        {
            b.HasIndex(x => new { x.FeeId, x.UserId }).IsUnique();
        });
        builder.Entity<FaultReport>(b =>
        {
            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            b.HasIndex(x => x.BuildingId);
        });
    }
}