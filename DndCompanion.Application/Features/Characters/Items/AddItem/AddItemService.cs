using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.Items.AddItem;

public sealed class AddItemService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;

    public AddItemService(
        ISessionRepository sessionRepository, 
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }
    
    public async Task<AddItemResult> ExecuteAsync(
        AddItemCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new AddItemResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new AddItemResult(false, "Participant has no character assigned");
        
        var character = await _characterRepository.FindByIdWithItemsAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new AddItemResult(false, "Character not found");

        try
        {
            var item = character.AddItem(
                command.Name, 
                command.Description,
                command.SourceUrl,
                command.Quantity);
            await _characterRepository.SaveChangesAsync(cancellationToken);
            
            return new AddItemResult(true, ItemId: item.Id);
        }
        catch (Exception e)
        {
            return new AddItemResult(false, e.Message);
        }
    }
}