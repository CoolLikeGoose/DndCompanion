using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.Items.RemoveItem;

public sealed class RemoveItemService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;

    public RemoveItemService(
        ISessionRepository sessionRepository, 
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }
    
    public async Task<RemoveItemResult> ExecuteAsync(
        RemoveItemCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new RemoveItemResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new RemoveItemResult(false, "Participant has no character assigned");
        
        var character = await _characterRepository.FindByIdWithItemsAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new RemoveItemResult(false, "Character not found");

        try
        {
            character.RemoveItem(command.ItemId);
            await _characterRepository.SaveChangesAsync(cancellationToken);
            
            return new RemoveItemResult(true);
        }
        catch (Exception e)
        {
            return new RemoveItemResult(false, e.Message);
        }
    }
}