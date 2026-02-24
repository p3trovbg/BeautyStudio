namespace AppointmentSystem.Common.Domain;

/// <summary>
/// Base entity with common properties for all domain entities.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Unique identifier for the entity.</summary>
    public Guid Id { get; set; }

    /// <summary>Timestamp when the entity was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Indicates whether the entity has been soft-deleted.</summary>
    public bool IsDeleted { get; set; }
}
