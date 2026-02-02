using Microsoft.EntityFrameworkCore;
using Entities;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Routes
{
    public static class UserEndpoints
    {

        public static void Map(WebApplication app)
        {

            var PasswordHasher = new PasswordHasher<User>();

            //Register endpoint 
            app.MapPost("/api/users/register", async ([FromBody] User newUser, DataContext db) =>
            {
                try
                {
                    //Check all the data is provided
                    if (newUser.Username == null || newUser.Email == null || newUser.Password == null)
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
                    newUser.Password = PasswordHasher.HashPassword(newUser, newUser.Password);

                    db.users.Add(newUser);
                    await db.SaveChangesAsync();

                    var userResponse = new
                    {
                        Id = newUser.Id,
                        Username = newUser.Username,
                        Email = newUser.Email
                    };

                    return Results.Created($"/api/users/{newUser.Id}", userResponse);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"An error ocurred creating the user {ex}");
                }
            });


            //Login endpoint
            app.MapPost("/api/users/login", async ([FromBody] User user, DataContext db) =>
            {
                try
                {
                    if (user.Email == null || user.Password == null || user.Username == null)
                    {
                        return Results.Unauthorized();
                    }
                    User UserFromDb = await db.users.FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower());

                    if (UserFromDb == null)
                    {
                        return Results.Unauthorized();
                    }

                    var isPasswordCorrect = PasswordHasher.VerifyHashedPassword(UserFromDb, UserFromDb.Password, user.Password);
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

            });
        }
    }
}
