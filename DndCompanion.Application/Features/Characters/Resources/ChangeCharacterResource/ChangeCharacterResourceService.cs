using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.Resources.ChangeCharacterResource;

public sealed class ChangeCharacterResourceService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;
    
    public ChangeCharacterResourceService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }

    public async Task<ChangeCharacterResourceResult> ExecuteAsync(
        ChangeCharacterResourceCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new ChangeCharacterResourceResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new ChangeCharacterResourceResult(false, "Participant has no character assigned");
        
        var character = await _characterRepository.FindByIdWithResourcesAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new ChangeCharacterResourceResult(false, "Character not found");
        
        try
        {
            var updated = character.ChangeResource(command.ResourceType, command.Variant, command.Delta);
            await _characterRepository.SaveChangesAsync(cancellationToken);
            
            return new ChangeCharacterResourceResult(true, null, updated.Type, updated.Variant, updated.CurrentValue, updated.MaxValue);
        }
        catch (ArgumentException e)
        {
            return new ChangeCharacterResourceResult(false, e.Message);
        }
    }
}