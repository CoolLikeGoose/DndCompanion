namespace Web.Services;

public abstract record BattleLogEntry(DateTime Timestamp);

public sealed record DamageLogEntry(DateTime Timestamp, string TargetName, int Amount, bool IsHeal)
    : BattleLogEntry(Timestamp);

public sealed record ItemLogEntry(DateTime Timestamp, string TargetName, string? ItemName)
    : BattleLogEntry(Timestamp);

public sealed record DeathThrowLogEntry(DateTime Timestamp, string TargetName, bool IsSuccess)
    : BattleLogEntry(Timestamp);