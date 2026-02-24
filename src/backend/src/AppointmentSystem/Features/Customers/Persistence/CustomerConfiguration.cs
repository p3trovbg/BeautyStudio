using AppointmentSystem.Common.Domain;
using AppointmentSystem.Features.Customers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointmentSystem.Features.Customers.Persistence;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
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
