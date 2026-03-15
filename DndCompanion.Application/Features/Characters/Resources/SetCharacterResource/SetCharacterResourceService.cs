using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.Resources.SetCharacterResource;

public sealed class SetCharacterResourceService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;
    
    public SetCharacterResourceService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }

    public async Task<SetCharacterResourceResult> ExecuteAsync(
        SetCharacterResourceCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new SetCharacterResourceResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new SetCharacterResourceResult(false, "Participant has no character assigned");
        
        var character = await _characterRepository.FindByIdWithResourcesAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new SetCharacterResourceResult(false, "Character not found");
        
        try
        {
            var updated = character.SetResource(command.ResourceType, command.Variant, command.Value);
            await _characterRepository.SaveChangesAsync(cancellationToken);
            
            return new SetCharacterResourceResult(true, null, updated.Type, updated.Variant, updated.CurrentValue, updated.MaxValue);
        }
        catch (ArgumentException e)
        {
            return new SetCharacterResourceResult(false, e.Message);
        }
    }
}