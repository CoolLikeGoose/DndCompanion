using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.Resources.AddDeathSave;

public class AddDeathSaveService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;

    public AddDeathSaveService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }

    public async Task<AddDeathSaveResult> ExecuteAsync(
        AddDeathSaveCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new AddDeathSaveResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new AddDeathSaveResult(false, "Participant has no character assigned");

        var character =
            await _characterRepository.FindByIdAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new AddDeathSaveResult(false, "Character not found");

        try
        {
            character.AddDeathSave(command.IsSuccess);
            await _characterRepository.SaveChangesAsync(cancellationToken);

            return new AddDeathSaveResult(true, 
                Successes: character.DeathSavesSuccesses,
                Failures: character.DeathSavesFailures);
        }
        catch (ArgumentException e)
        {
            return new AddDeathSaveResult(false, e.Message);
        }
    }
}