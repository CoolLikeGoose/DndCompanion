namespace Web.Services;

public sealed class SessionNotificationService
{
    private readonly Dictionary<Guid, List<Func<Task>>> _handlers = new();
    
    public void Subscribe(Guid sessionId, Func<Task> handler)
    {
        if (!_handlers.ContainsKey(sessionId))
        {
            _handlers[sessionId] = new List<Func<Task>>();
        }

        _handlers[sessionId].Add(handler);
    }
    
    public void Unsubscribe(Guid sessionId, Func<Task> handler)
    {
        if (_handlers.TryGetValue(sessionId, out var handlers))
        {
            handlers.Remove(handler);
        }
    }
    
    public async Task NotifyAsync(Guid sessionId)
    {
        if (_handlers.TryGetValue(sessionId, out var handlers))
        {
            foreach (var handler in handlers.ToList())
            {
                await handler();
            }
        }
    }
}