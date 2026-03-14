using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.SelectCharacter;

public sealed class SelectCharacterService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ICurrentUser _currentUser;
    
    public SelectCharacterService(
        ICharacterRepository characterRepository, 
        ISessionRepository sessionRepository, 
        ICurrentUser currentUser)
    {
        _characterRepository = characterRepository;
        _sessionRepository = sessionRepository;
        _currentUser = currentUser;
    }

    public async Task<SelectCharacterResult> ExecuteAsync(
        SelectCharacterCommand command, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null) 
            return new SelectCharacterResult(false, "Only authenticated users can select existing characters");

        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new SelectCharacterResult(false, "Participant not found");

        var characters = await _characterRepository.GetByUserIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (!characters.Any(c => c.Id == command.CharacterId))
            return new SelectCharacterResult(false, "Characters not found");

        participant.AssignCharacter(command.CharacterId);
        await _sessionRepository.SaveChangesAsync(cancellationToken);
        
        return new SelectCharacterResult(true, null, command.SessionId, command.ParticipantId);
    }
}