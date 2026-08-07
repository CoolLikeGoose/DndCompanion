using DndCompanion.Application.Abstractions.Persistence;
using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.ApplyRest.ApplyRestParty;

public class ApplyRestPartyService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;

    public ApplyRestPartyService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }

    public async Task<ApplyRestPartyResult> ExecuteAsync(
        ApplyRestPartyCommand command, CancellationToken cancellationToken = default)
    {
        var session =
            await _sessionRepository.FindByIdWithParticipantsAndCharactersAsync(command.SessionId, cancellationToken);

        if (session is null)
            return new ApplyRestPartyResult(false, "Session not found");

        if (!session.Participants.Any())
            return new ApplyRestPartyResult(false, "No participants found");

        foreach (var participant in session.Participants.Where(p => p.CharacterId is not null))
        {
            var character =
                await _characterRepository.FindByIdWithResourcesAsync(participant.CharacterId!.Value,
                    cancellationToken);

            if (character is null)
                continue;

            try
            {
                character.ApplyRest(command.RecoveryType);
                Console.WriteLine(
                    $"After rest - HP: {character.Resources.FirstOrDefault(r => r.Type == ResourceType.HitPoints)?.CurrentValue}");
                await _characterRepository.SaveChangesAsync(cancellationToken);
                Console.WriteLine("Saved");
            }
            catch (ArgumentException e)
            {
                return new ApplyRestPartyResult(false, e.Message);
            }
        }

        // await _characterRepository.SaveChangesAsync(cancellationToken);

        return new ApplyRestPartyResult(true);
    }
}