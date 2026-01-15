using Ardalis.GuardClauses;

namespace EasyDoc.Domain.Entities;

public record PhoneNumber // value object
{
    public string Value { get; } = default!;

    private PhoneNumber() { }
    public PhoneNumber(string value)
    {
        Guard.Against.NullOrWhiteSpace(value);
        value = Normalize(value);

        Value = value;
    }
    private static string Normalize(string value) => value.Trim(); // TODO: better normlization

    public override string ToString() => Value;
    

}
