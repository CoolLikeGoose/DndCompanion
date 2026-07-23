using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.AddAbilitySlot;

public sealed record AddAbilitySlotCommand(
    Guid ParticipantId,
    string? Name,
    string? Group,
    int MaxValue,
    RecoveryType RecoveryType);