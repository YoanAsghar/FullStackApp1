using Entities;
using Microsoft.EntityFrameworkCore;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Routes
{
    public class UserEndpoints : ControllerBase
    {

        public static void Map(WebApplication app)
        {
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
                    User UserExists = await db.users.FirstOrDefaultAsync(u => u.Email.ToLower() == newUser.Email.ToLower());

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
                    return Results.Problem($"An error ocurred creating the user {ex}");
                }
            });


            //Login endpoint
            AuthGroup.MapPost("/login", async ([FromBody] User user, DataContext db) =>
            {
                try
                {
                    if (user.Email == null || user.passwordHash == null || user.Username == null)
                    {
                        return Results.Unauthorized();
                    }
                    User UserFromDb = await db.users.FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower());

                    if (UserFromDb == null)
                    {
                        return Results.Unauthorized();
                    }

                    var isPasswordCorrect = PasswordHasher.VerifyHashedPassword(UserFromDb, UserFromDb.passwordHash, user.passwordHash);
                    if (isPasswordCorrect != PasswordVerificationResult.Success)
                    {
                        return Results.Unauthorized();
                    }

                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    return Results.Conflict($"Error ocurred login {ex}");
                }

            }).RequireAuthorization();
        }
    }
}
