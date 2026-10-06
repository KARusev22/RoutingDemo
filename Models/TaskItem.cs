namespace RoutingDemo.Models;

/// <summary>
/// Модел на задача. ASP.NET Core го сериализира автоматично като JSON.
/// </summary>
public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsDone { get; set; }
}
