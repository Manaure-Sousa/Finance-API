using Finance_API.Models;
using FinanceAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceAPI.Endpoints
{
    public static class CategoriesEndpoints
    {
        public static void MapCategoriesEndpoints(this WebApplication app)
        {
            app.MapPost("/categories", async (AppDbContext db, Category category) =>
            {
                db.Categories.Add(category);
                await db.SaveChangesAsync();
                return Results.Created($"/categories/{category.Id}", category);
            });

            app.MapGet("/categories", async (AppDbContext db) =>
            {
                var categories = await db.Categories.ToListAsync();
                return Results.Ok(categories);
            });

            app.MapGet("/categories/{id}", async (AppDbContext db, int id) =>
            {
                var category = await db.Categories.FindAsync(id);
                return category != null ? Results.Ok(category) : Results.NotFound();
            });

            app.MapPut("/categories/{id}", async (AppDbContext db, int id, Category updatedCategory) =>
            {
                var category = await db.Categories.FindAsync(id);
                if (category == null) return Results.NotFound();

                category.Name = updatedCategory.Name;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            app.MapDelete("/categories/{id}", async (AppDbContext db, int id) =>
            {
                var category = await db.Categories.FindAsync(id);
                if (category == null) return Results.NotFound();

                db.Categories.Remove(category);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}