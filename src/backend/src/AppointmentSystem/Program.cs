using System.Text;
using AppointmentSystem.Common.Application.Interfaces;
using AppointmentSystem.Common.Application.Mappings;
using AppointmentSystem.Common.Email;
using AppointmentSystem.Common.Middleware;
using AppointmentSystem.Common.Persistence;
using AppointmentSystem.Features.Appointments.Jobs;
using AppointmentSystem.Features.Appointments.Services;
using AppointmentSystem.Features.Customers.Services;
using AppointmentSystem.Features.Owners.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog configuration
builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration));

// 2. Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Add Hangfire
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString)));
builder.Services.AddHangfireServer();

// 4. Register Layer Dependencies
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IEmailService, MailKitEmailService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// 5. Add AutoMapper and FluentValidation
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(typeof(Program).Assembly);

// 6. Controllers and Exception Handling
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 7. JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key missing");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// 8. Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Appointment System API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }});
});

// 9. CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? Array.Empty<string>();
        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});

// 10. Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Auto-migrate in development
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Hangfire Dashboard (restrict in production!)
app.UseHangfireDashboard("/hangfire");

// Schedule the daily reminder job
RecurringJob.AddOrUpdate<AppointmentReminderJob>(
    "send-appointment-reminders",
    job => job.SendRemindersAsync(CancellationToken.None),
    Cron.Daily(8) // Runs daily at 08:00 UTC
);

app.Run();
