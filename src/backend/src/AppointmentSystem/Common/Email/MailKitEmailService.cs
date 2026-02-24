using AppointmentSystem.Common.Application.Interfaces;
using AppointmentSystem.Features.Appointments.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AppointmentSystem.Common.Email;

/// <summary>
/// Sends transactional emails via SMTP using MailKit.
/// </summary>
public class MailKitEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MailKitEmailService> _logger;

    /// <summary>Initializes a new instance of <see cref="MailKitEmailService"/>.</summary>
    public MailKitEmailService(IConfiguration configuration, ILogger<MailKitEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendBookingConfirmationAsync(AppointmentDto appointment, CancellationToken ct)
    {
        var subject = $"Booking Confirmed: {appointment.Title}";
        var body = $@"<h2>Appointment Confirmed</h2>
            <p><strong>Title:</strong> {appointment.Title}</p>
            <p><strong>Date:</strong> {appointment.StartTime:f} — {appointment.EndTime:t}</p>
            <p><strong>Owner:</strong> {appointment.OwnerName}</p>
            <p><strong>Customer:</strong> {appointment.CustomerName}</p>
            {(string.IsNullOrEmpty(appointment.Notes) ? "" : $"<p><strong>Notes:</strong> {appointment.Notes}</p>")}";

        await SendEmailAsync(appointment.CustomerEmail, subject, body, ct);
        await SendEmailAsync(appointment.OwnerEmail, subject, body, ct);
    }

    /// <inheritdoc />
    public async Task SendCancellationNoticeAsync(AppointmentDto appointment, CancellationToken ct)
    {
        var subject = $"Appointment Cancelled: {appointment.Title}";
        var body = $@"<h2>Appointment Cancelled</h2>
            <p><strong>Title:</strong> {appointment.Title}</p>
            <p><strong>Date:</strong> {appointment.StartTime:f} — {appointment.EndTime:t}</p>";

        await SendEmailAsync(appointment.CustomerEmail, subject, body, ct);
        await SendEmailAsync(appointment.OwnerEmail, subject, body, ct);
    }

    /// <inheritdoc />
    public async Task SendReminderAsync(AppointmentDto appointment, CancellationToken ct)
    {
        var subject = $"Reminder: {appointment.Title} tomorrow";
        var body = $@"<h2>Appointment Reminder</h2>
            <p><strong>Title:</strong> {appointment.Title}</p>
            <p><strong>Date:</strong> {appointment.StartTime:f} — {appointment.EndTime:t}</p>
            <p><strong>With:</strong> {appointment.OwnerName}</p>";

        await SendEmailAsync(appointment.CustomerEmail, subject, body, ct);
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct)
    {
        var smtpHost = _configuration["Email:SmtpHost"]!;
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"]!);
        var smtpUser = _configuration["Email:SmtpUser"]!;
        var smtpPassword = _configuration["Email:SmtpPassword"]!;
        var fromAddress = _configuration["Email:FromAddress"]!;

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(fromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(smtpUser, smtpPassword, ct);
            await client.SendAsync(message, ct);
            _logger.LogInformation("Email sent to {Recipient}: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}: {Subject}", to, subject);
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
        }
    }
}
