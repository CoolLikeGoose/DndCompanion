using DndCompanion.Application.Abstractions.Persistence;
using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.RemoveAbilitySlot;

public sealed class RemoveAbilitySlotService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;

    public RemoveAbilitySlotService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }

    public async Task<RemoveAbilitySlotResult> ExecuteAsync(
        RemoveAbilitySlotCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new RemoveAbilitySlotResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new RemoveAbilitySlotResult(false, "Participant has no character assigned");

        var character =
            await _characterRepository.FindByIdWithResourcesAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new RemoveAbilitySlotResult(false, "Character not found");

        try
        {
            character.RemoveResource(ResourceType.AbilitySlot, command.Name);
            await _characterRepository.SaveChangesAsync(cancellationToken);

            return new RemoveAbilitySlotResult(true);
        }
        catch (Exception e)
        {
            return new RemoveAbilitySlotResult(false, e.Message);
        }
    }
}