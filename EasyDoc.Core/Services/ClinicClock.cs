using System;

namespace EasyDoc.Application.Services;

/// <summary>
/// EasyDoc stores appointment dates and times as clinic-local values (DateOnly/TimeOnly).
/// All "now" comparisons must use the clinic's time zone rather than the server's UTC clock.
/// </summary>
public static class ClinicClock
{
    private static readonly TimeZoneInfo Zone = ResolveZone();

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);

    public static DateOnly Today => DateOnly.FromDateTime(Now);

    public static bool IsInTheFuture(DateOnly date, TimeOnly time) =>
        IsInTheFuture(date, time, Now);

    public static bool IsInTheFuture(DateOnly date, TimeOnly time, DateTime clinicNow) =>
        new DateTime(date, time) > clinicNow;

    private static TimeZoneInfo ResolveZone()
    {
        foreach (var id in new[] { "Africa/Cairo", "Egypt Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
                // Try the next platform-specific identifier.
            }
            catch (InvalidTimeZoneException)
            {
                // Try the next platform-specific identifier.
            }
        }

        // The demo must stay deterministic even on a host with no tzdata.
        return TimeZoneInfo.Utc;
    }
}
