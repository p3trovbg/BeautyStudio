using AppointmentSystem.Features.Appointments.Domain;
using AppointmentSystem.Features.Customers.Domain;
using AppointmentSystem.Features.Owners.Domain;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Common.Persistence;

/// <summary>
/// EF Core DbContext for the Appointment Management System.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>Initializes a new instance of <see cref="AppDbContext"/>.</summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>Owners table.</summary>
    public DbSet<Owner> Owners => Set<Owner>();
    /// <summary>Customers table.</summary>
    public DbSet<Customer> Customers => Set<Customer>();
    /// <summary>Appointments table.</summary>
    public DbSet<Appointment> Appointments => Set<Appointment>();

    /// <summary>Applies entity configurations and global query filters.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filters for soft delete
        modelBuilder.Entity<Owner>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Customer>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Appointment>().HasQueryFilter(e => !e.IsDeleted);
    }
}
