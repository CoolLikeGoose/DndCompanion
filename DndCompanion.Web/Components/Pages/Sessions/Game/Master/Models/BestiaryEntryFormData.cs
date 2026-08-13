namespace Web.Components.Pages.Sessions.Game.Master.Models;

public sealed record BestiaryEntryFormData(
    string Name,
    int MaxHp,
    string? Description,
    bool IsEdit,
    Guid? BestiaryEntryId = null);