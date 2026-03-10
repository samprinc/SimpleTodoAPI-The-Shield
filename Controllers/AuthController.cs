using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleTodoAPI.Data;
using SimpleTodoAPI.Models;
using SimpleTodoAPI.Services;

namespace SimpleTodoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    // -----------------------------------------------
    // POST: api/auth/register
    // No token needed — anyone can register
    // -----------------------------------------------
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        // Check if this username is already taken
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (existingUser != null)
        {
            return BadRequest(new { message = "Username already taken. Choose another." });
        }

        // NEVER save plain text passwords
        // BCrypt.HashPassword turns "pass123" into a safe hash like "$2a$11$xyz..."
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            Role = request.Role // "User" by default, "Admin" if specified
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Registration successful! You can now login.",
            username = user.Username,
            role = user.Role
        });
    }

    // -----------------------------------------------
    // POST: api/auth/login
    // No token needed — this is where you GET the token
    // -----------------------------------------------
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        // STEP 1: Find the user by username
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        // STEP 2: If user not found, return Unauthorized
        // NOTE: We say "Invalid username or password" not "User not found"
        // This way attackers cannot tell which one is wrong (security best practice)
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        // STEP 3: Check the password against the stored hash
        // BCrypt.Verify compares "pass123" against "$2a$11$xyz..." and returns true/false
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        // STEP 4: Everything is correct — generate and return the JWT token
        var token = _tokenService.GenerateToken(user);

        return Ok(new TokenResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role,
            Message = $"Welcome back, {user.Username}!"
        });
    }
}
