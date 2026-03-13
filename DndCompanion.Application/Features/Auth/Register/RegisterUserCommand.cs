namespace DndCompanion.Application.Features.Auth.Register;

public sealed record RegisterUserCommand(
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword);