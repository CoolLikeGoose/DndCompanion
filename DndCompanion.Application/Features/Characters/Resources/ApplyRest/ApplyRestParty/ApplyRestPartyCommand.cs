using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.ApplyRest.ApplyRestParty;

public sealed record ApplyRestPartyCommand(
    Guid SessionId,
    RecoveryType RecoveryType,
    bool IncludeShortOnLongRest = false);