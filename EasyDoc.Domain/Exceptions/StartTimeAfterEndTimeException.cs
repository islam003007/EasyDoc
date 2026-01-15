namespace EasyDoc.Domain.Exceptions;

public class StartTimeAfterEndTimeException: DomainRuleException
{
    public StartTimeAfterEndTimeException(TimeOnly startTime, TimeOnly endTime)
       : base("TimeRange.Invalid", $"Start time '{startTime}' cannot be later than end time '{endTime}'.", new {StartTime = startTime, EndTime = endTime})
    {
    }
}
