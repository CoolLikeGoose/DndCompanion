namespace DndCompanion.Application.Features.Characters.Resources.AddDeathSave;

public sealed record AddDeathSaveCommand(
    Guid ParticipantId,
    bool IsSuccess);