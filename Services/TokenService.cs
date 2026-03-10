using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SimpleTodoAPI.Models;

namespace SimpleTodoAPI.Services;

// TokenService has ONE job:
// Take a User → Build a JWT token → Return the token string
//
// HOW JWT WORKS:
// ┌─────────────────────────────────────────────────────┐
// │  eyJhbGci...   .   eyJ1c2VyI...   .   abc123xyz    │
// │   HEADER            PAYLOAD          SIGNATURE      │
// │  (algorithm)    (user data)       (tamper-proof)    │
// └─────────────────────────────────────────────────────┘
//
// The PAYLOAD contains: user id, username, role
// The SIGNATURE proves nobody changed the payload
// The server checks the signature on every request

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user)
    {
        // STEP 1: Claims = the data we put INSIDE the token
        // Anyone can decode these (they are base64, not encrypted)
        // But nobody can FAKE them without the secret key
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User's ID
            new Claim(ClaimTypes.Name, user.Username),                // Username
            new Claim(ClaimTypes.Role, user.Role)                     // "Admin" or "User"
        };

        // STEP 2: The secret key — must match what is in appsettings.json
        // This key is what makes the signature tamper-proof
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        // STEP 3: The signing algorithm (HmacSha256 is the standard)
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // STEP 4: Build the token with all the pieces
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],       // Who created this token
            audience: _config["Jwt:Audience"],   // Who should use this token
            claims: claims,                       // The user data inside
            expires: DateTime.UtcNow.AddMinutes( // When it expires
                double.Parse(_config["Jwt:ExpiryInMinutes"]!)
            ),
            signingCredentials: credentials       // The signature
        );

        // STEP 5: Convert to the string format: xxxxx.yyyyy.zzzzz
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
