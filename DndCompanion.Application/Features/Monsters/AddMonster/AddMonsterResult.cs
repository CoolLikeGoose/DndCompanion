using Domain.Entities;

namespace DndCompanion.Application.Features.Monsters.AddMonster;

public record AddMonsterResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    Monster? Monster = null);