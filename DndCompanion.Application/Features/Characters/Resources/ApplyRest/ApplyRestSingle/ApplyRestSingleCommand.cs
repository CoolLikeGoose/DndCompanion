using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.ApplyRest.ApplyRestSingle;

public sealed record ApplyRestSingleCommand(
    Guid ParticipantId,
    RecoveryType RecoveryType,
    bool IncludeShortOnLongRest = false);