using Entities;
using Microsoft.EntityFrameworkCore;
using Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Routes
{
    public static class ProductEndpoints
    {
        public static void Map(WebApplication app)
        {
            var Configuration = app.Configuration;
            var ProductsGroup = app.MapGroup("/api/products");

            //Returns all the products
            ProductsGroup.MapGet("/", async (DataContext db) =>
            {
                var products = await db.products.ToListAsync();

                return Results.Ok(products);
            });
            //Returns a product by id
            ProductsGroup.MapGet("/{id:int}", async (int id, DataContext db) =>
            {
                var product = await db.products.FindAsync(id);

                if (product == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(product);
            });

            //Creates a product
            ProductsGroup.MapPost("/", async ([FromBody] Product newProduct, DataContext db, ClaimsPrincipal User) =>
            {
                //TextProduct to send

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Results.Unauthorized();
                }

                db.products.Add(newProduct);
                await db.SaveChangesAsync();

                return Results.Created($"/api/products/{newProduct.Id}", newProduct);
            }).RequireAuthorization();

            ProductsGroup.MapPut("/{id:int}", async (int id, DataContext db, [FromBody] Product ProductReplace) =>
            {
                var ProductToUpdate = await db.products.FindAsync(id);

                if (ProductToUpdate == null)
                {
                    return Results.NotFound();
                }

                ProductToUpdate.Name = ProductReplace.Name;
                ProductToUpdate.Description = ProductReplace.Description;
                ProductToUpdate.Price = ProductReplace.Price;
                ProductToUpdate.Stock = ProductReplace.Stock;

                await db.SaveChangesAsync();

                return Results.Ok(ProductToUpdate);
            }).RequireAuthorization();

            ProductsGroup.MapDelete("/{id:int}", async (int id, DataContext db) =>
            {
                var ItemToRemove = await db.products.FindAsync(id);

                if (ItemToRemove == null)
                {
                    return Results.NotFound();
                }

                db.products.Remove(ItemToRemove);
                await db.SaveChangesAsync();
                return Results.Ok();
            }).RequireAuthorization();
        }
    }
}
