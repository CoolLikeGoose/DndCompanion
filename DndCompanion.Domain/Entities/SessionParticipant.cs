using Domain.Enums;

namespace Domain.Entities;

public class SessionParticipant
{
    private SessionParticipant()
    {
        
    }

    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid? UserId { get; private set; }
    public string DisplayName { get; private set; } = null!;
    public SessionRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }
    
    // Current character
    public Guid? CharacterId { get; private set; }
    public Character? Character { get; private set; }

    public static SessionParticipant Create(Guid sessionId, Guid? userId, string displayName, SessionRole role)
    {
        if (sessionId == Guid.Empty)
            throw new ArgumentException("Session id is required", nameof(sessionId));
        
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name is required", nameof(displayName));
        
        var normalizedDisplayName = displayName.Trim();
        if (normalizedDisplayName.Length > 50)
            throw new ArgumentException("Display name must be less than 50 chars", nameof(displayName));

        return new SessionParticipant
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            UserId = userId,
            DisplayName = normalizedDisplayName,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };
    }

    public void AssignCharacter(Guid characterId)
    {
        if (characterId == Guid.Empty)
            throw new ArgumentException("Character id is required", nameof(characterId));
        
        CharacterId = characterId;
    }
}