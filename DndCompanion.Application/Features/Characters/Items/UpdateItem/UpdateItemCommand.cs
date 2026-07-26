namespace DndCompanion.Application.Features.Characters.Items.UpdateItem;

public sealed record UpdateItemCommand(
    Guid ParticipantId,
    Guid ItemId,
    string? Name = null,
    string? Description = null,
    string? SourceUrl = null,
    int? Quantity = null);