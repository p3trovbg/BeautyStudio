using AppointmentSystem.Features.Appointments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointmentSystem.Features.Appointments.Persistence;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        // Value object mapping for DateTimeRange
        builder.OwnsOne(x => x.TimeRange, range =>
        {
            range.Property(r => r.Start).HasColumnName("StartTime").IsRequired();
            range.Property(r => r.End).HasColumnName("EndTime").IsRequired();
        });

        // PostgreSQL concurrency token using xmin
        builder.Property(x => x.Version)
            .IsRowVersion()
            .HasColumnName("xmin")
            .HasColumnType("xid");

        // Relationships
        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Composite Index for overlap queries
        builder.HasIndex("OwnerId", "Status");
    }
}
