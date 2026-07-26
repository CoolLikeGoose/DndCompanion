namespace DndCompanion.Application.Features.Characters.Items.RemoveItem;

public sealed record RemoveItemCommand(
    Guid ParticipantId,
    Guid ItemId);