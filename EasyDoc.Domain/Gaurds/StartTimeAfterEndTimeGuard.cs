using Ardalis.GuardClauses;
using EasyDoc.Domain.Exceptions;

namespace EasyDoc.Domain.Gaurds;

public static class StartTimeAfterEndTimeGuard
{
    public static void StartTimeAfterEndTime(this IGuardClause clause, TimeOnly startTime, TimeOnly endTime)
    {
        if (startTime > endTime)
            throw new StartTimeAfterEndTimeException(startTime, endTime);
    }
}
