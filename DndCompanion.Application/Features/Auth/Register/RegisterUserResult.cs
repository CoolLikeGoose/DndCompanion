namespace DndCompanion.Application.Features.Auth.Register;

public sealed record RegisterUserResult(
    bool IsSuccess,
    string? ErrorMessage,
    Guid? UserId = null);