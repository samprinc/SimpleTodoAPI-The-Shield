using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleTodoAPI.Data;
using SimpleTodoAPI.Models;

namespace SimpleTodoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // 🔒 ALL endpoints below require a valid JWT token
            // Without a token → 401 Unauthorized
            // With wrong/expired token → 401 Unauthorized
public class TodosController : ControllerBase
{
    private readonly AppDbContext _context;

    public TodosController(AppDbContext context)
    {
        _context = context;
    }

    // GET ALL — any logged in user (User or Admin)
    [HttpGet]
    public async Task<IActionResult> GetAllTodos()
    {
        var todos = await _context.Todos.ToListAsync();
        return Ok(todos);
    }

    // GET ONE — any logged in user (User or Admin)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodo(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound("Task not found.");
        return Ok(todo);
    }

    // POST — any logged in user (User or Admin)
    [HttpPost]
    public async Task<IActionResult> AddTodo(TodoItem newItem)
    {
        _context.Todos.Add(newItem);
        await _context.SaveChangesAsync();
        return Ok(newItem);
    }

    // PUT — any logged in user (User or Admin)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, TodoItem updatedItem)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound("Task not found.");

        todo.Title = updatedItem.Title;
        todo.Description = updatedItem.Description;
        todo.IsCompleted = updatedItem.IsCompleted;

        await _context.SaveChangesAsync();
        return Ok(todo);
    }

    // DELETE — ONLY Admin role can delete
    // [Authorize(Roles = "Admin")] overrides the controller-level [Authorize]
    // A "User" token hitting this endpoint → 403 Forbidden
    // An "Admin" token hitting this endpoint → 204 No Content ✅
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound();

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
