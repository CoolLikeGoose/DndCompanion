using System.Security.Cryptography;

namespace Domain.ValueObjects;

public class InviteCode
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    public string Value { get; }
    
    private InviteCode(string value) => Value = value;

    public static InviteCode Generate(int length = 6)
    {
        if (length > 10)
            throw new ArgumentOutOfRangeException(nameof(length), "Invite code length must be less than 10");
        
        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        return new InviteCode(new string(chars));
    }

    public static InviteCode From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Invite code is required.", nameof(value));
        
        return new InviteCode(value.Trim().ToUpperInvariant());
    }
    
    public override string ToString() => Value;
}