using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.Resources.ApplyRest;

public class ApplyRestService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;
    
    public ApplyRestService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }

    public async Task<ApplyRestResult> ExecuteAsync(
        ApplyRestCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new ApplyRestResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new ApplyRestResult(false, "Participant has no character assigned");
        
        var character = await _characterRepository.FindByIdWithResourcesAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new ApplyRestResult(false, "Character not found");
        
        try
        {
            var updated = character.ApplyRest(command.RecoveryType, command.IncludeShortOnLongRest);
            await _characterRepository.SaveChangesAsync(cancellationToken);
            
            return new ApplyRestResult(true, null, updated);
        }
        catch (ArgumentException e)
        {
            return new ApplyRestResult(false, e.Message);
        }
    }
}