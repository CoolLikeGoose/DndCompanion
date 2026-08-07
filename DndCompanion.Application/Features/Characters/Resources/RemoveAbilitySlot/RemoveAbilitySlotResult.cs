namespace DndCompanion.Application.Features.Characters.Resources.RemoveAbilitySlot;

public sealed record RemoveAbilitySlotResult (
    bool IsSuccess,
    string? ErrorMessage = null);