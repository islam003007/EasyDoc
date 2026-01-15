namespace EasyDoc.SharedKernel;

public struct Optional<TValue>
{
    public bool IsProvided { get; }
    public TValue? Value { get; }
    public Optional(TValue? value)
    {
        Value = value;
        IsProvided = true;
    }

    public static Optional<TValue> NotProvided() => default;

    public static Optional<TValue> NullValue = new Optional<TValue>(default);
    public void IfProvided(Action<TValue> action)
    {
        if (IsProvided)
            action(Value!); 
    }
}
