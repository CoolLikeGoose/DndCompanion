using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.Resources.SetCharacterResourceMax;

public sealed class SetCharacterResourceMaxService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;
    
    public SetCharacterResourceMaxService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }

    public async Task<SetCharacterResourceMaxResult> ExecuteAsync(
        SetCharacterResourceMaxCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new SetCharacterResourceMaxResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new SetCharacterResourceMaxResult(false, "Participant has no character assigned");
        
        var character = await _characterRepository.FindByIdWithResourcesAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new SetCharacterResourceMaxResult(false, "Character not found");
        
        try
        {
            var updated = character.SetResourceMaximum(command.ResourceType, command.Variant, command.Value);
            await _characterRepository.SaveChangesAsync(cancellationToken);
            
            return new SetCharacterResourceMaxResult(true, null, updated.Type, updated.Variant, updated.CurrentValue, updated.MaxValue);
        }
        catch (ArgumentException e)
        {
            return new SetCharacterResourceMaxResult(false, e.Message);
        }
    }
}