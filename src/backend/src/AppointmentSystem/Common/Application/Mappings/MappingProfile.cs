using AppointmentSystem.Features.Appointments.Domain;
using AppointmentSystem.Features.Appointments.DTOs;
using AppointmentSystem.Features.Customers.Domain;
using AppointmentSystem.Features.Customers.DTOs;
using AppointmentSystem.Features.Owners.Domain;
using AppointmentSystem.Features.Owners.DTOs;
using AutoMapper;

namespace AppointmentSystem.Common.Application.Mappings;

/// <summary>
/// AutoMapper profile defining Entity ↔ DTO mappings.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>Configures all entity-to-DTO and DTO-to-entity maps.</summary>
    public MappingProfile()
    {
        // Appointments
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.Owner.FullName))
            .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Customer.FullName))
            .ForMember(d => d.OwnerEmail, opt => opt.MapFrom(s => s.Owner.Email.Value))
            .ForMember(d => d.CustomerEmail, opt => opt.MapFrom(s => s.Customer.Email.Value));
        CreateMap<CreateAppointmentDto, Appointment>();

        // Owners
        CreateMap<Owner, OwnerDto>()
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value));

        // Customers
        CreateMap<Customer, CustomerDto>()
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value));
    }
}
