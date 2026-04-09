using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<ReservationWave> ReservationWaves => Set<ReservationWave>();
    public DbSet<Theatre> Theatres => Set<Theatre>();
    public DbSet<Play> Plays => Set<Play>();
    public DbSet<Performance> Performances => Set<Performance>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Subscription: composite key
        builder.Entity<Subscription>(e =>
        {
            e.HasKey(s => new { s.UserId, s.SeasonId });

            e.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(s => s.Season)
                .WithMany(s => s.Subscriptions)
                .HasForeignKey(s => s.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Season -> ReservationWaves
        builder.Entity<ReservationWave>(e =>
        {
            e.HasOne(rw => rw.Season)
                .WithMany(s => s.ReservationWaves)
                .HasForeignKey(rw => rw.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Theatre -> Plays
        builder.Entity<Play>(e =>
        {
            e.HasOne(p => p.Theatre)
                .WithMany(t => t.Plays)
                .HasForeignKey(p => p.TheatreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Performance relationships
        builder.Entity<Performance>(e =>
        {
            e.HasOne(p => p.ReservationWave)
                .WithMany(rw => rw.Performances)
                .HasForeignKey(p => p.ReservationWaveId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(p => p.Play)
                .WithMany(pl => pl.Performances)
                .HasForeignKey(p => p.PlayId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(p => p.Theatre)
                .WithMany()
                .HasForeignKey(p => p.TheatreId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(p => p.ReservationWaveId);
            e.HasIndex(p => p.PlayId);
            e.HasIndex(p => p.TheatreId);
        });

        // Reservation relationships and indexes
        builder.Entity<Reservation>(e =>
        {
            e.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(r => r.Performance)
                .WithMany(p => p.Reservations)
                .HasForeignKey(r => r.PerformanceId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(r => new { r.UserId, r.PerformanceId }).IsUnique();
            e.HasIndex(r => r.PerformanceId);
        });

        // Store enum as string
        builder.Entity<Reservation>()
            .Property(r => r.Status)
            .HasConversion<string>();

        // NotificationLog
        builder.Entity<NotificationLog>(e =>
        {
            e.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(n => n.Performance)
                .WithMany()
                .HasForeignKey(n => n.PerformanceId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(n => new { n.UserId, n.PerformanceId, n.NotificationType });
        });
    }
}
