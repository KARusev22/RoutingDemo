using System.Collections.Concurrent;
using RoutingDemo.Models;

namespace RoutingDemo.Data;

/// <summary>
/// In-memory хранилище. Данните се пазят в паметта докато приложението работи
/// (подходящо за демо; при рестарт се нулират).
/// </summary>
public class TaskStore
{
    // ConcurrentDictionary е thread-safe – безопасен при едновременни заявки.
    private readonly ConcurrentDictionary<int, TaskItem> _items = new();

    // Следващото свободно Id. Interlocked.Increment е атомарна операция.
    private int _nextId = 1;

    public IEnumerable<TaskItem> GetAll()
        => _items.Values.OrderBy(task => task.Id);

    public TaskItem? GetById(int id)
        => _items.TryGetValue(id, out var item) ? item : null;

    public TaskItem Add(string title)
    {
        var item = new TaskItem
        {
            Id = Interlocked.Increment(ref _nextId) - 1,
            Title = title,
            IsDone = false
        };

        _items[item.Id] = item;
        return item;
    }

    public bool Remove(int id)
        => _items.TryRemove(id, out _);
}
