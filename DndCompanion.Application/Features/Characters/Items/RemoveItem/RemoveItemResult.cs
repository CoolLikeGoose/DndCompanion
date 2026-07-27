namespace DndCompanion.Application.Features.Characters.Items.RemoveItem;

public sealed record RemoveItemResult (
    bool IsSuccess,
    string? ErrorMessage = null);