namespace Web.Services;

public sealed class SessionNotificationService
{
    private readonly Dictionary<Guid, List<Func<SessionEvent, Task>>> _handlers = new();
    private readonly object _lock = new();

    private readonly Dictionary<Guid, Queue<BattleLogEntry>> _logs = new();
    private const int MaxLogEntries = 100;

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
        if (sessionEvent.LogEntry is not null)
        {
            lock (_lock)
            {
                if (!_logs.ContainsKey(sessionEvent.SessionId))
                {
                    _logs[sessionEvent.SessionId] = new Queue<BattleLogEntry>();
                }

                var queue = _logs[sessionEvent.SessionId];
                queue.Enqueue(sessionEvent.LogEntry);

                while (queue.Count > MaxLogEntries)
                    queue.Dequeue();
            }
        }

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

    public IReadOnlyList<BattleLogEntry> GetLog(Guid sessionId)
    {
        lock (_lock)
        {
            return _logs.TryGetValue(sessionId, out var queue)
                ? queue.ToList().AsReadOnly()
                : Array.Empty<BattleLogEntry>();
        }
    }
}