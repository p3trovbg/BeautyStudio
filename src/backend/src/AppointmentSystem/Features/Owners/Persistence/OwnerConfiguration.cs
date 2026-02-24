using AppointmentSystem.Common.Domain;
using AppointmentSystem.Features.Owners.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointmentSystem.Features.Owners.Persistence;

public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(254)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value)
            );

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);
    }
}
