using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;
using Domain.Entities;

namespace DndCompanion.Application.Features.Characters.CreateCharacter;

public sealed class CreateCharacterService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ICurrentUser _currentUser;
    
    public CreateCharacterService(
        ICharacterRepository characterRepository, 
        ISessionRepository sessionRepository, 
        ICurrentUser currentUser)
    {
        _characterRepository = characterRepository;
        _sessionRepository = sessionRepository;
        _currentUser = currentUser;
    }

    public async Task<CreateCharacterResult> ExecuteAsync(
        CreateCharacterCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new CreateCharacterResult(false, "Participant not found");

        Character character;
        try
        {
            character = Character.Create(command.Name, _currentUser.UserId);
        }
        catch (ArgumentException e)
        {
            return new CreateCharacterResult(false, e.Message);
        }
        
        participant.AssignCharacter(character.Id);
        
        await _characterRepository.AddAsync(character, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);
        
        return new CreateCharacterResult(
            true, 
            null, 
            character.Id, 
            command.SessionId, 
            command.ParticipantId);
    }
}