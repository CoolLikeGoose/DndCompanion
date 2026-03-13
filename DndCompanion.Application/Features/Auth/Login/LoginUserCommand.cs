namespace DndCompanion.Application.Features.Auth.Login;

public sealed record LoginUserCommand(
    string Email,
    string Password);