namespace DndCompanion.Application.Features.Characters.Items.UpdateItem;

public sealed record UpdateItemResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    Guid? ItemId = null);