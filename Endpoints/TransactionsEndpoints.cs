using Finance_API.Models;
using FinanceAPI.Data;
using FinanceAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FinanceAPI.Endpoints
{
    public static class TransactionsEndpoints
    {
        public static void MapTransactionsEndpoints(this WebApplication app)
        {
            var transactionsGroup = app.MapGroup("/transactions");

            transactionsGroup.MapGet("/", async (AppDbContext db) =>
            {
                return Results.Ok(await db.Transactions.ToListAsync());
            });

            transactionsGroup.MapGet("/{id}", async (AppDbContext db, int id) =>
                await db.Transactions.FindAsync(id) is Transaction transactionFind
                    ? Results.Ok(transactionFind)
                    : Results.NotFound("Transaction not found.")
            );

            transactionsGroup.MapPost("/", async (AppDbContext db, TransactionDTO dto) =>
            {
                var transaction = new Transaction
                {
                    Amount = dto.Amount,
                    Name = dto.Name,
                    Description = dto.Description,
                    Type = dto.Type,
                    CategoryId = dto.CategoryId,
                };
                db.Transactions.Add(transaction);
                await db.SaveChangesAsync();
                return Results.Created($"/transactions/{transaction.Id}", transaction);
            });

            transactionsGroup.MapPut("/{id}", async (AppDbContext db, int id, Transaction newTransaction) =>
            {
                var transactionFind = await db.Transactions.FindAsync(id);
                if (transactionFind is null)
                    return Results.NotFound("Transaction not found.");

                transactionFind.Amount = newTransaction.Amount;
                transactionFind.Name = newTransaction.Name;
                transactionFind.Description = newTransaction.Description;
                transactionFind.Type = newTransaction.Type;
                transactionFind.Category = newTransaction.Category; // modificar posteriormente => CategoryID
                transactionFind.Date = newTransaction.Date;
                transactionFind.UpdatedAt = DateTime.UtcNow;

                await db.SaveChangesAsync();
                return Results.NoContent();

            });

            transactionsGroup.MapDelete("/{id}", async (AppDbContext db, int id) =>
            {
                if (await db.Transactions.FindAsync(id) is Transaction transactionFind)
                {
                    db.Transactions.Remove(transactionFind);
                    await db.SaveChangesAsync();
                    return Results.NoContent();
                }
                return Results.NotFound("Transaction not found.");
            });
        }
    }
}