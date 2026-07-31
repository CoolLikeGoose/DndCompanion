using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.UpdateStats;

public class UpdateStatsService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;

    public UpdateStatsService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }
    
    public async Task<UpdateStatsResult> ExecuteAsync(
        UpdateStatsCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new UpdateStatsResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new UpdateStatsResult(false, "Participant has no character assigned");

        var character =
            await _characterRepository.FindByIdWithInfoAndStatsAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new UpdateStatsResult(false, "Character not found");

        try
        {
            character.UpdateStats(
                strength: command.Strength,
                dexterity: command.Dexterity,
                constitution: command.Constitution,
                intelligence: command.Intelligence,
                wisdom: command.Wisdom,
                charisma: command.Charisma
            );
            
            await _characterRepository.SaveChangesAsync(cancellationToken);
            
            return new UpdateStatsResult(true);
        }
        catch (Exception e)
        {
            return new UpdateStatsResult(false, e.Message);
        }
    }
}