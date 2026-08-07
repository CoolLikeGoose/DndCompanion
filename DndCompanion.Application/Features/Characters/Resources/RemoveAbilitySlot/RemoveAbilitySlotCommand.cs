namespace DndCompanion.Application.Features.Characters.Resources.RemoveAbilitySlot;

public sealed record RemoveAbilitySlotCommand(
    Guid ParticipantId,
    string Name);