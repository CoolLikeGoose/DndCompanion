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

    private readonly List<Monster> _monsters = new();
    public IReadOnlyCollection<Monster> Monsters => _monsters.AsReadOnly();

    public static Session Create(Guid? masterUserId, string masterDisplayName, PinCode? pinCode = null)
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

        var defaultBattle = Battle.Create(session.Id, "Other monsters", 0);
        session._battles.Add(defaultBattle);
        session.DefaultBattleId = defaultBattle.BattleId;

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

    // Monsters
    public Monster AddMonster(string name, int maxHp, string? description = null, Guid? bestiaryEntryId = null,
        Guid? battleId = null)
    {
        var resolvedBattleId = battleId ?? DefaultBattleId;
        if (_battles.All(b => b.BattleId != resolvedBattleId))
            throw new ArgumentException($"Battle {resolvedBattleId} not found in this session.", nameof(battleId));
        
        var maxOrder = _monsters
            .Where(m => m.BattleId == resolvedBattleId)
            .Select(m => (double?)m.Order)
            .Max() ?? 0;

        var monster = Monster.Create(Id, name, maxHp, resolvedBattleId, description, bestiaryEntryId, maxOrder + 1000);
        _monsters.Add(monster);
        return monster;
    }

    public void RemoveMonster(Guid monsterId)
    {
        var monster = _monsters.FirstOrDefault(x => x.Id == monsterId);
        if (monster is null)
            throw new ArgumentException($"Monster with id {monsterId} not found for this session.", nameof(monsterId));
        _monsters.Remove(monster);
    }

    public void UpdateMonster(
        Guid monsterId,
        string? name = null,
        int? maxHp = null,
        int? currentHp = null,
        string? description = null)
    {
        var monster = _monsters.FirstOrDefault(x => x.Id == monsterId);
        if (monster is null)
            throw new ArgumentException($"Monster with id {monsterId} not found for this session.", nameof(monsterId));

        monster.Update(name, maxHp, currentHp, description);
    }
    
    public void ReorderMonster(Guid monsterId, double newOrder)
    {
        var monster = _monsters.FirstOrDefault(m => m.Id == monsterId);
        if (monster is null)
            throw new ArgumentException($"Monster with id {monsterId} not found for this session.", nameof(monsterId));

        monster.SetOrder(newOrder);
    }

    // Battles
    public Guid DefaultBattleId { get; private set; }
    private readonly List<Battle> _battles = [];
    public IReadOnlyCollection<Battle> Battles => _battles.AsReadOnly();

    public Battle AddBattle(string name)
    {
        var maxOrder = _battles
            .Select(b => (double?)b.Order)
            .Max() ?? 0;
        
        var battle = Battle.Create(Id, name, maxOrder + 1000);
        _battles.Add(battle);
        return battle;
    }

    public void RemoveBattle(Guid battleId)
    {
        if (battleId == DefaultBattleId)
            throw new InvalidOperationException("Cannot remove the default battle.");

        var battle = _battles.FirstOrDefault(b => b.BattleId == battleId);
        if (battle is null)
            throw new ArgumentException($"Battle {battleId} not found.", nameof(battleId));

        _battles.Remove(battle);
    }
    
    public void ReorderBattle(Guid battleId, double newOrder)
    {
        var battle = _battles.FirstOrDefault(b => b.BattleId == battleId);
        if (battle is null)
            throw new ArgumentException($"Battle {battleId} not found.", nameof(battleId));

        battle.SetOrder(newOrder);
    }
    
    public void MoveMonsterToBattle(Guid monsterId, Guid targetBattleId, double newOrder)
    {
        if (_battles.All(b => b.BattleId != targetBattleId))
            throw new ArgumentException($"Battle {targetBattleId} not found in this session.", nameof(targetBattleId));

        var monster = _monsters.FirstOrDefault(m => m.Id == monsterId);
        if (monster is null)
            throw new ArgumentException($"Monster with id {monsterId} not found for this session.", nameof(monsterId));

        monster.MoveToBattle(targetBattleId, newOrder);
    }
}