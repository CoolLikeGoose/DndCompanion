namespace Web.Components.Pages.Sessions.Game.Master.Models;

public sealed record MonsterFormData(
    string Name,
    int MaxHp,
    int CurrentHp,
    string? Description,
    bool IsEdit,
    Guid? MonsterId = null,
    Guid? BestiaryEntryId = null,
    bool SaveToBestiary = false);