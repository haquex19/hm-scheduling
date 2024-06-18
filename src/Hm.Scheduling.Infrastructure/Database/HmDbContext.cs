using Hm.Scheduling.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hm.Scheduling.Infrastructure.Database;

public class HmDbContext(DbContextOptions<HmDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<AppointmentAvailability> Availabilities { get; set; }

    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HmDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateDates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void UpdateDates()
    {
        var entries = ChangeTracker.Entries<IEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedOn = DateTimeOffset.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedOn = DateTimeOffset.UtcNow;
            }
        }
    }
}
