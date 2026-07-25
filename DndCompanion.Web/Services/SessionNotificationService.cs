namespace Web.Services;

public sealed class SessionNotificationService
{
    private readonly Dictionary<Guid, List<Func<Task>>> _handlers = new();
    private readonly object _lock = new();
    
    public void Subscribe(Guid sessionId, Func<Task> handler)
    {
        lock (_lock)
        {
            if (!_handlers.ContainsKey(sessionId))
            {
                _handlers[sessionId] = new List<Func<Task>>();
            }

            _handlers[sessionId].Add(handler);
        }
    }
    
    public void Unsubscribe(Guid sessionId, Func<Task> handler)
    {
        lock (_lock)
        {
            if (_handlers.TryGetValue(sessionId, out var handlers))
            {
                handlers.Remove(handler);
            }
        }
    }
    
    public async Task NotifyAsync(Guid sessionId)
    {
        List<Func<Task>> snapshot;
        lock (_lock)
        {
            if (!_handlers.TryGetValue(sessionId, out var handlers))
            {
                return;
            }   
            snapshot = handlers.ToList();
        }

        foreach (var handler in snapshot)
        {
            try
            {
                await handler();
            }
            catch
            {
                // ignored
            }
        }
    }
}