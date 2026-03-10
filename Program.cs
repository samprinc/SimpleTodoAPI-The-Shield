using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleTodoAPI.Data;
using SimpleTodoAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════
// STEP 1: REGISTER SERVICES
// ═══════════════════════════════════════════════

// Tells ASP.NET to find and use our Controller classes
builder.Services.AddControllers();

// Database: SQLite file called "todos.db"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=todos.db"));

// Register TokenService for dependency injection
builder.Services.AddScoped<TokenService>();

// // Swagger with Authorize button
// builder.Services.AddEndpointsApiExplorer();

// builder.Services.AddSwaggerGen(options =>
// {
//     options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//     {
//         Name = "Authorization",
//         Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
//         Scheme = "Bearer",
//         BearerFormat = "JWT",
//         In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//         Description = "Paste your JWT token here. Example: eyJhbGci..."
//     });
//     options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
//     {
//         {
//             new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//             {
//                 Reference = new Microsoft.OpenApi.Models.OpenApiReference
//                 {
//                     Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 }
//             },
//             Array.Empty<string>()
//         }
//     });
// });

// ═══════════════════════════════════════════════
// STEP 2: CONFIGURE JWT AUTHENTICATION
// Reads and validates tokens on every request
// ═══════════════════════════════════════════════
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,           // Was this token made by us?
            ValidateAudience = true,         // Is this token meant for us?
            ValidateLifetime = true,         // Has this token expired?
            ValidateIssuerSigningKey = true, // Was this signed with our secret key?

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

// Enables [Authorize] and [Authorize(Roles = "Admin")] to work
builder.Services.AddAuthorization();

// ═══════════════════════════════════════════════
// STEP 3: BUILD + PIPELINE
// ORDER MATTERS — do not rearrange
// ═══════════════════════════════════════════════
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // WHO ARE YOU?   → validates the token
app.UseAuthorization();  // WHAT CAN YOU DO? → enforces [Authorize]

app.MapControllers();

app.Run();
