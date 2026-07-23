using DndCompanion.Application.Abstractions.Persistence;
using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.AddAbilitySlot;

public sealed class AddAbilitySlotService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;
    
    public AddAbilitySlotService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }

    public async Task<AddAbilitySlotResult> ExecuteAsync(
        AddAbilitySlotCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new AddAbilitySlotResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new AddAbilitySlotResult(false, "Participant has no character assigned");
        
        var character = await _characterRepository.FindByIdWithResourcesAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new AddAbilitySlotResult(false, "Character not found");

        try
        {
            character.AddResource(
                ResourceType.AbilitySlot, 
                command.MaxValue, 
                command.RecoveryType, 
                command.Name, 
                command.Group);
            await _characterRepository.SaveChangesAsync(cancellationToken);
            
            return new AddAbilitySlotResult(true);
        }
        catch (Exception e)
        {
            return new AddAbilitySlotResult(false, e.Message);
        }
    }
}