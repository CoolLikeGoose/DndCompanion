using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.ApplyRest;

public sealed record ApplyRestCommand(
    Guid ParticipantId,
    RecoveryType RecoveryType,
    bool IncludeShortOnLongRest = false);