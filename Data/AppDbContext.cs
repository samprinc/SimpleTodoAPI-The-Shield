using Microsoft.EntityFrameworkCore;
using SimpleTodoAPI.Models;

namespace SimpleTodoAPI.Data;

// AppDbContext = the bridge between C# code and the database
// Every DbSet here becomes a TABLE in the database
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Creates the "Todos" table — same as The Vault
    public DbSet<TodoItem> Todos { get; set; }

    // NEW: Creates the "Users" table for authentication
    public DbSet<User> Users { get; set; }
}
