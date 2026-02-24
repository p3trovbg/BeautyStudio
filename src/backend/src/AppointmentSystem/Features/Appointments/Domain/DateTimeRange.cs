using AppointmentSystem.Common.Domain;

namespace AppointmentSystem.Features.Appointments.Domain;

/// <summary>
/// A value object representing a contiguous range of time.
/// </summary>
public sealed record DateTimeRange
{
    /// <summary>The start of the range.</summary>
    public DateTime Start { get; }
    /// <summary>The end of the range.</summary>
    public DateTime End { get; }

    private DateTimeRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    /// <summary>Creates a new validated <see cref="DateTimeRange"/>.</summary>
    public static DateTimeRange Create(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new DomainException("End time must be after start time.", "INVALID_TIME_RANGE");

        return new DateTimeRange(start.ToUniversalTime(), end.ToUniversalTime());
    }

    /// <summary>Checks if this range overlaps with another range.</summary>
    public bool OverlapsWith(DateTimeRange other)
    {
        // Two ranges overlap if the first starts before the second ends,
        // and the first ends after the second starts.
        return Start < other.End && End > other.Start;
    }
}
