using System.ComponentModel.DataAnnotations;
using AppointmentSystem.Common.Domain;
using AppointmentSystem.Features.Customers.Domain;
using AppointmentSystem.Features.Owners.Domain;

namespace AppointmentSystem.Features.Appointments.Domain;

/// <summary>
/// Represents an appointment booked between a customer and an owner.
/// </summary>
public class Appointment : BaseEntity
{
    private Appointment(Guid ownerId, Guid customerId, string title, DateTimeRange timeRange, string? notes)
    {
        OwnerId = ownerId;
        CustomerId = customerId;
        Title = title;
        TimeRange = timeRange;
        Notes = notes;
        Status = AppointmentStatus.Pending;
    }

    public Guid OwnerId { get; private set; }
    public Owner Owner { get; private set; } = null!;
    
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    public string Title { get; private set; }
    public DateTimeRange TimeRange { get; private set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; private set; }

    /// <summary>Optimistic concurrency token (PostgreSQL xmin).</summary>
    [Timestamp]
    public uint Version { get; private set; }

    // Required by EF Core
    private Appointment() { Title = null!; TimeRange = null!; }

    /// <summary>Creates a new appointment booking.</summary>
    public static Appointment Create(Guid ownerId, Guid customerId, string title, DateTimeRange timeRange, string? notes)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty.", "INVALID_TITLE");

        return new Appointment(ownerId, customerId, title, timeRange, notes);
    }

    /// <summary>Updates appointment details.</summary>
    public void Update(string title, DateTimeRange timeRange, string? notes)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty.", "INVALID_TITLE");

        Title = title;
        TimeRange = timeRange;
        Notes = notes;
    }
}
