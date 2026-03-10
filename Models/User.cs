namespace SimpleTodoAPI.Models;

// Blueprint for a user in our system
// Every person who wants to use the API must be a User
public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    // RULE: We NEVER store the actual password
    // We always store the HASH (a scrambled version)
    // BCrypt turns "mypassword123" into "$2a$11$xyz..." 
    // Even if the database is stolen, passwords are safe
    public string PasswordHash { get; set; } = string.Empty;

    // Role decides what the user can do
    // "User"  → can read, add, update todos
    // "Admin" → can also DELETE todos
    public string Role { get; set; } = "User";
}
