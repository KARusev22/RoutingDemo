using Microsoft.AspNetCore.Mvc;
using RoutingDemo.Data;
using RoutingDemo.Models;

namespace RoutingDemo.Controllers;

/// <summary>
/// Демонстрира РОУТИНГ чрез REST заявки.
/// Атрибутът [Route("api/[controller]")] определя базовия път: /api/tasks
/// (името на контролера минус наставката "Controller").
/// Всеки метод се селектира според HTTP глагола: [HttpGet], [HttpPost], [HttpDelete].
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskStore _store;
    private readonly ILogger<TasksController> _logger;

    public TasksController(TaskStore store, ILogger<TasksController> logger)
    {
        _store = store;
        _logger = logger;
    }

    // GET /api/tasks  ->  връща всички задачи (200 OK)
    [HttpGet]
    public ActionResult<IEnumerable<TaskItem>> GetAll()
    {
        _logger.LogInformation("ROUTING: GET /api/tasks -> GetAll()");
        return Ok(_store.GetAll());
    }

    // GET /api/tasks/5  ->  връща една задача по Id (200 OK или 404 Not Found)
    // "{id}" е шаблонен параметър: стойността от URL се подава като аргумент.
    [HttpGet("{id}")]
    public ActionResult<TaskItem> GetById(int id)
    {
        var task = _store.GetById(id);

        if (task is null)
        {
            _logger.LogWarning("ROUTING: GET /api/tasks/{Id} -> 404 Not Found", id);
            return NotFound();
        }

        _logger.LogInformation("ROUTING: GET /api/tasks/{Id} -> GetById()", id);
        return Ok(task);
    }

    // POST /api/tasks  ->  създава нова задача (201 Created или 400 Bad Request)
    // Тялото на заявката (JSON) се десериализира автоматично в параметъра input.
    [HttpPost]
    public ActionResult<TaskItem> Create([FromBody] TaskItem input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
        {
            _logger.LogWarning("ROUTING: POST /api/tasks -> 400 (празно Title)");
            return BadRequest(new { error = "Полето 'title' е задължително." });
        }

        var created = _store.Add(input.Title.Trim());
        _logger.LogInformation("ROUTING: POST /api/tasks -> Create() създаде задача {Id}", created.Id);

        // 201 Created + Location: /api/tasks/{id}
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // DELETE /api/tasks/5  ->  изтрива задача по Id (204 No Content или 404 Not Found)
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (_store.GetById(id) is null)
        {
            _logger.LogWarning("ROUTING: DELETE /api/tasks/{Id} -> 404 Not Found", id);
            return NotFound();
        }

        _store.Remove(id);
        _logger.LogInformation("ROUTING: DELETE /api/tasks/{Id} -> Delete() (204 No Content)", id);
        return NoContent();
    }
}
