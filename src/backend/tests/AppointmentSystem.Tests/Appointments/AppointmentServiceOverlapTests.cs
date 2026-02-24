using AppointmentSystem.Common.Application.Interfaces;
using AppointmentSystem.Common.Application.Mappings;
using AppointmentSystem.Common.Domain;
using AppointmentSystem.Common.Persistence;
using AppointmentSystem.Features.Appointments.Domain;
using AppointmentSystem.Features.Appointments.DTOs;
using AppointmentSystem.Features.Appointments.Services;
using AppointmentSystem.Features.Appointments.Validators;
using AppointmentSystem.Features.Customers.Domain;
using AppointmentSystem.Features.Owners.Domain;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppointmentSystem.Tests.Appointments;

/// <summary>
/// Unit tests for the AppointmentService overlap detection logic.
/// </summary>
public class AppointmentServiceOverlapTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AppointmentService _service;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();

    public AppointmentServiceOverlapTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        var unitOfWork = new TestUnitOfWork(_context);
        _emailServiceMock = new Mock<IEmailService>();

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = mapperConfig.CreateMapper();

        var createValidator = new CreateAppointmentValidator();
        var updateValidator = new UpdateAppointmentValidator();
        var logger = new Mock<ILogger<AppointmentService>>();

        _service = new AppointmentService(
            _context, unitOfWork, _emailServiceMock.Object, mapper, createValidator, updateValidator, logger.Object);

        SeedData();
    }

    private void SeedData()
    {
        var owner = Owner.Create("Test Owner", Email.Create("owner@test.com"), null);
        owner.Id = _ownerId;
        _context.Owners.Add(owner);

        var customer = Customer.Create("Test Customer", Email.Create("customer@test.com"), null);
        customer.Id = _customerId;
        _context.Customers.Add(customer);

        // Existing appointment: 10:00 — 11:00 today
        var appointment = Appointment.Create(
            _ownerId,
            _customerId,
            "Existing Appointment",
            DateTimeRange.Create(DateTime.UtcNow.Date.AddDays(1).AddHours(10), DateTime.UtcNow.Date.AddDays(1).AddHours(11)),
            null
        );
        appointment.Status = AppointmentStatus.Confirmed;
        _context.Appointments.Add(appointment);

        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenNewAppointmentOverlapsExisting()
    {
        var dto = new CreateAppointmentDto(
            _ownerId, _customerId, "Overlap Test", 
            DateTime.UtcNow.Date.AddDays(1).AddHours(10).AddMinutes(30), 
            DateTime.UtcNow.Date.AddDays(1).AddHours(11).AddMinutes(30), null);

        var result = await _service.CreateAsync(dto, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("APPOINTMENT_OVERLAP");
    }

    [Fact]
    public async Task CreateAsync_ShouldSucceed_WhenNoOverlap()
    {
        var dto = new CreateAppointmentDto(
            _ownerId, _customerId, "No Overlap", 
            DateTime.UtcNow.Date.AddDays(1).AddHours(12), 
            DateTime.UtcNow.Date.AddDays(1).AddHours(13), null);

        var result = await _service.CreateAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("No Overlap");
    }

    public void Dispose() => _context.Dispose();
}

internal class TestUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public TestUnitOfWork(AppDbContext context) => _context = context;
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
    public Task BeginTransactionAsync(System.Data.IsolationLevel isolationLevel, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
