namespace DndCompanion.Application.Features.Characters.Items.AddItem;

public sealed record AddItemCommand(
    Guid ParticipantId,
    string Name,
    string? Description = null,
    string? SourceUrl = null,
    int Quantity = 1);