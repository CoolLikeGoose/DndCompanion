using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Session
{
    
    private Session()
    {
        
    }
    
    public Guid Id { get; private set; }
    public InviteCode InviteCode { get; private set; } = null!;
    public PinCode? PinCode { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<SessionParticipant> Participants { get; private set; } = new List<SessionParticipant>();
    
    public static Session Create(Guid? masterUserId, string masterDisplayName, PinCode? pinCode)
    {
        if (string.IsNullOrWhiteSpace(masterDisplayName))
            throw new ArgumentException("Master display name is required", nameof(masterDisplayName));

        var session = new Session
        {
            Id = Guid.NewGuid(),
            InviteCode = InviteCode.Generate(),
            PinCode = pinCode,
            CreatedAt = DateTime.UtcNow
        };

        var master = SessionParticipant.Create(
            session.Id,
            masterUserId,
            masterDisplayName,
            SessionRole.Master);
        
        session.Participants.Add(master);
        return session;
    }

    public SessionParticipant Join(string displayName, Guid? userId = null, PinCode? pinCode = null)
    {
        if (PinCode is not null)
        {
            if (pinCode is null || !pinCode.Equals(PinCode))
                throw new ArgumentException("Invalid pin code", nameof(pinCode));
        }
        
        if (userId.HasValue && Participants.Any(p => p.UserId == userId.Value))
            throw new ArgumentException("User already joined", nameof(userId));

        var participant = SessionParticipant.Create(Id, userId, displayName, SessionRole.Player);
        Participants.Add(participant);
        
        return participant;
    }
}