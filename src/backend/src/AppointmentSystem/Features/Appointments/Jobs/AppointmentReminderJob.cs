using AppointmentSystem.Common.Email;
using AppointmentSystem.Common.Persistence;
using AppointmentSystem.Features.Appointments.Domain;
using AppointmentSystem.Features.Appointments.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppointmentSystem.Features.Appointments.Jobs;

/// <summary>
/// Hangfire background job to send 24-hour appointment reminders.
/// </summary>
public class AppointmentReminderJob
{
    private readonly AppDbContext _context;
    private readonly MailKitEmailService _emailService; // Using directly since hangfire jobs are often DI transient
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentReminderJob> _logger;

    public AppointmentReminderJob(AppDbContext context, MailKitEmailService emailService, IMapper mapper, ILogger<AppointmentReminderJob> logger)
    {
        _context = context;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>Finds appointments starting in ~24 hours and sends reminders.</summary>
    public async Task SendRemindersAsync(CancellationToken ct)
    {
        var targetStart = DateTime.UtcNow.Date.AddDays(1);
        var targetEnd = targetStart.AddDays(1);

        var upcomingAppointments = await _context.Appointments
            .Include(a => a.Owner)
            .Include(a => a.Customer)
            .Where(a => 
                a.Status == AppointmentStatus.Confirmed &&
                a.TimeRange.Start >= targetStart &&
                a.TimeRange.Start < targetEnd)
            .ToListAsync(ct);

        _logger.LogInformation("Found {Count} upcoming appointments for reminder to send", upcomingAppointments.Count);

        foreach (var appointment in upcomingAppointments)
        {
            try
            {
                var dto = _mapper.Map<AppointmentDto>(appointment);
                await _emailService.SendReminderAsync(dto, ct);
                _logger.LogInformation("Sent reminder for appointment {Id}", appointment.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reminder for appointment {Id}", appointment.Id);
            }
        }
    }
}
