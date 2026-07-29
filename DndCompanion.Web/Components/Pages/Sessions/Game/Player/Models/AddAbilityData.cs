using Domain.Enums;

namespace Web.Components.Pages.Sessions.Game.Player.Models;

public record AddAbilityData(
    string Name, 
    string? Group,
    int MaxValue,
    RecoveryType RecoveryType);