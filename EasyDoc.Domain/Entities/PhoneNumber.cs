using Ardalis.GuardClauses;

namespace EasyDoc.Domain.Entities;

public record PhoneNumber // value object. At this point might be overengineering but fine.
{
    public string Value { get; } = default!;

    private PhoneNumber() { }
    public PhoneNumber(string value)
    {
        Guard.Against.NullOrWhiteSpace(value);
        Value = value;
    }
    public override string ToString() => Value;
}
