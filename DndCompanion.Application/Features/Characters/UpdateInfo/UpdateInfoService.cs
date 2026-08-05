using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.UpdateInfo;

public class UpdateInfoService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;

    public UpdateInfoService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }

    public async Task<UpdateInfoResult> ExecuteAsync(
        UpdateInfoCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new UpdateInfoResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new UpdateInfoResult(false, "Participant has no character assigned");

        var character =
            await _characterRepository.FindByIdWithInfoAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new UpdateInfoResult(false, "Character not found");

        try
        {
            character.UpdateInfo(
                characterClass: command.CharacterClass,
                level: command.Level,
                race: command.Race,
                age: command.Age,
                background: command.Background,
                alignment: command.Alignment,
                experiencePoints: command.ExperiencePoints,
                personalityTraits: command.PersonalityTraits,
                ideals: command.Ideals,
                bonds: command.Bonds,
                flaws: command.Flaws,
                languageProficiencies: command.LanguageProficiencies,
                toolProficiencies: command.ToolProficiencies
            );

            await _characterRepository.SaveChangesAsync(cancellationToken);

            return new UpdateInfoResult(true);
        }
        catch (Exception e)
        {
            return new UpdateInfoResult(false, e.Message);
        }
    }
}