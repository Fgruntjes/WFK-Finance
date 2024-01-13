using NodaTime;

namespace App.Lib.Data;

public static class NodaTimeExtension
{
    public static Instant ToUtc(this LocalDate localDate)
    {
        return localDate.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant();
    }

    public static Instant ToStartOfTheDay(this Instant instant)
    {
        return instant.InUtc().Date.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant();
    }

    public static Instant ToEndOfTheDay(this Instant instant)
    {
        return instant.InUtc().Date.PlusDays(1).AtStartOfDayInZone(DateTimeZone.Utc).ToInstant().Minus(Duration.FromTicks(1));
    }
}