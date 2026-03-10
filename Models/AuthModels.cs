namespace SimpleTodoAPI.Models;

// DTO = Data Transfer Object
// These are NOT database models — they are just shapes for incoming/outgoing data

// -------------------------------------------------
// What the client sends when REGISTERING
// POST /api/auth/register
// -------------------------------------------------
public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Optional: if not provided, defaults to "User"
    public string Role { get; set; } = "User";
}

// -------------------------------------------------
// What the client sends when LOGGING IN
// POST /api/auth/login
// -------------------------------------------------
public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// -------------------------------------------------
// What we send BACK after successful login
// The client must save this token and send it with every future request
// -------------------------------------------------
public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Message { get; set; } = "Login successful!";
}
