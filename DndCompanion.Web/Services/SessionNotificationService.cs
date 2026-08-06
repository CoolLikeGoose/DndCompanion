namespace Web.Services;

public sealed class SessionNotificationService
{
    private readonly Dictionary<Guid, List<Func<SessionEvent, Task>>> _handlers = new();
    private readonly object _lock = new();
    
    public void Subscribe(Guid sessionId, Func<SessionEvent, Task> handler)
    {
        lock (_lock)
        {
            if (!_handlers.ContainsKey(sessionId))
            {
                _handlers[sessionId] = new List<Func<SessionEvent, Task>>();
            }

            _handlers[sessionId].Add(handler);
        }
    }
    
    public void Unsubscribe(Guid sessionId, Func<SessionEvent, Task> handler)
    {
        lock (_lock)
        {
            if (_handlers.TryGetValue(sessionId, out var handlers))
            {
                handlers.Remove(handler);
            }
        }
    }
    
    public async Task NotifyAsync(SessionEvent sessionEvent)
    {
        List<Func<SessionEvent, Task>> snapshot;
        lock (_lock)
        {
            if (!_handlers.TryGetValue(sessionEvent.SessionId, out var handlers))
            {
                return;
            }   
            snapshot = handlers.ToList();
        }

        foreach (var handler in snapshot)
        {
            try
            {
                await handler(sessionEvent);
            }
            catch
            {
                // ignored
            }
        }
    }
}