using Entities;
using Microsoft.EntityFrameworkCore;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Routes
{
    public class UserEndpoints : ControllerBase
    {
        public static void Map(WebApplication app)
        {
            var Configuration = app.Configuration;
            var AuthGroup = app.MapGroup("/api/auth");
            var PasswordHasher = new PasswordHasher<User>();

            //Register endpoint 
            AuthGroup.MapPost("/register", async ([FromBody] User newUser, DataContext db) =>
            {
                try
                {
                    //Check all the data is provided
                    if (newUser == null)
                    {
                        return Results.Conflict("Provide all the data for the registration");
                    }

                    //Check if it already exists
                    User? UserExists = await db.users.FirstOrDefaultAsync(u => u.Email.ToLower() == newUser.Email.ToLower());

                    if (UserExists != null)
                    {
                        return Results.Conflict("User already exists");
                    }

                    newUser.Register_date = DateOnly.FromDateTime(DateTime.UtcNow);

                    // Ensure the plain-text password is not null or empty before hashing
                    if (string.IsNullOrEmpty(newUser.passwordHash))
                    {
                        return Results.BadRequest("Password cannot be empty.");
                    }

                    newUser.passwordHash = PasswordHasher.HashPassword(newUser, newUser.passwordHash);

                    db.users.Add(newUser);
                    await db.SaveChangesAsync();

                    return Results.Created($"/api/users/{newUser.Id}", newUser);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"An error occurred creating the user: {ex.Message}");
                }
            });

            //Login endpoint
            AuthGroup.MapPost("/login", async ([FromBody] User user, DataContext db) =>
            {
                try
                {
                    User? UserFromDb = await db.users.FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower());

                    if (UserFromDb == null)
                    {
                        return Results.NotFound("User not found.");
                    }

                    var isPasswordCorrect = PasswordHasher.VerifyHashedPassword(UserFromDb, UserFromDb.passwordHash, user.passwordHash);
                    if (isPasswordCorrect == PasswordVerificationResult.Failed)
                    {
                        return Results.Unauthorized();
                    }

                    // Generate JSON Web Token
                    var issuer = Configuration["Jwt:Issuer"];
                    var audience = Configuration["Jwt:Audience"];
                    var keyString = Configuration["Jwt:Key"];

                    if (string.IsNullOrEmpty(keyString))
                    {
                        return Results.Problem("JWT Key is not configured.", statusCode: 500);
                    }

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, UserFromDb.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, UserFromDb.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    var token = new JwtSecurityToken(
                       issuer: issuer,
                       audience: audience,
                       claims: claims,
                       expires: DateTime.Now.AddMinutes(60),
                       signingCredentials: credentials);

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    return Results.Ok(new { token = tokenString });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"An error occurred during login: {ex.Message}");
                }
            });
        }
    }
}
