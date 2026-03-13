namespace DndCompanion.Application.Features.Auth.Login;

public sealed record LoginUserResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    Guid? UserId = null,
    string? UserName = null,
    string? Email = null);