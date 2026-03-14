namespace Domain.ValueObjects;

public class PinCode
{
    public string Value { get; }
    
    private PinCode(string value) => Value = value;

    public static PinCode From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Pin code is required.", nameof(value));

        var normalized = value.Trim();
        if (normalized.Length is < 4 or > 8)
            throw new ArgumentException("Pin code must contain 4-8 chars");

        return new PinCode(normalized);
    }
    
    public override string ToString() => Value;
}