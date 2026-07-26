using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Characters.Items.UpdateItem;

public sealed class UpdateItemService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICharacterRepository _characterRepository;
    
    public UpdateItemService(
        ISessionRepository sessionRepository,
        ICharacterRepository characterRepository)
    {
        _sessionRepository = sessionRepository;
        _characterRepository = characterRepository;
    }
    
    public async Task<UpdateItemResult> ExecuteAsync(
        UpdateItemCommand command, CancellationToken cancellationToken = default)
    {
        var participant = await _sessionRepository.FindParticipantByIdAsync(command.ParticipantId, cancellationToken);
        if (participant is null)
            return new UpdateItemResult(false, "Participant not found");
        if (participant.CharacterId is null)
            return new UpdateItemResult(false, "Participant has no character assigned");
        
        var character = await _characterRepository.FindByIdWithItemsAsync(participant.CharacterId.Value, cancellationToken);
        if (character is null)
            return new UpdateItemResult(false, "Character not found");

        try
        {
            character.UpdateItem(
                command.ItemId,
                command.Name,
                command.Description,
                command.SourceUrl,
                command.Quantity);
            await _characterRepository.SaveChangesAsync(cancellationToken);
            
            return new UpdateItemResult(true);
        }
        catch (Exception e)
        {
            return new UpdateItemResult(false, e.Message);
        }
    }
}