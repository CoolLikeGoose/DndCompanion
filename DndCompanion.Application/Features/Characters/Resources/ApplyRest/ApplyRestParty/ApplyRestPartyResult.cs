namespace DndCompanion.Application.Features.Characters.Resources.ApplyRest.ApplyRestParty;

public sealed record ApplyRestPartyResult(
    bool IsSuccess,
    string? ErrorMessage = null);