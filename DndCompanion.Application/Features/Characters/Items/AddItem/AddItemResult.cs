namespace DndCompanion.Application.Features.Characters.Items.AddItem;

public sealed record AddItemResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    Guid? ItemId = null);