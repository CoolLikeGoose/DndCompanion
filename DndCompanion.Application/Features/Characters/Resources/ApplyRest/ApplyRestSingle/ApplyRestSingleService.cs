using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.Resources.ApplyRest.ApplyRestSingle;

public class ApplyRestSingleService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;

    public ApplyRestSingleService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }

    public async Task<ApplyRestSingleResult> ExecuteAsync(
        ApplyRestSingleCommand singleCommand, CancellationToken cancellationToken = default)
    {
        var participant =
            await _sessionRepository.FindParticipantByIdAsync(singleCommand.ParticipantId, cancellationToken);
        if (participant is null)
            return new ApplyRestSingleResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new ApplyRestSingleResult(false, "Participant has no character assigned");

        var character =
            await _characterRepository.FindByIdWithResourcesAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new ApplyRestSingleResult(false, "Character not found");

        try
        {
            var updated = character.ApplyRest(singleCommand.RecoveryType, singleCommand.IncludeShortOnLongRest);
            await _characterRepository.SaveChangesAsync(cancellationToken);

            return new ApplyRestSingleResult(true, null, updated);
        }
        catch (ArgumentException e)
        {
            return new ApplyRestSingleResult(false, e.Message);
        }
    }
}