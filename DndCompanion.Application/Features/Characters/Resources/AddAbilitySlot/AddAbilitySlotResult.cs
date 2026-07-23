namespace DndCompanion.Application.Features.Characters.Resources.AddAbilitySlot;

public sealed record AddAbilitySlotResult(
    bool IsSuccess,
    string? ErrorMessage = null);