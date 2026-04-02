using FinanceAPI.Models;
using FinanceAPI.Data;
using FinanceAPI.DTOs;

using Microsoft.EntityFrameworkCore;
using FinanceAPI.Models.Enums;

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

            transactionsGroup.MapGet("/month", async (AppDbContext db, int? year = null, int? month = null) =>
            {
                var now = DateTime.UtcNow;
                var targetYear = year ?? now.Year;
                var targetMonth = month ?? now.Month;

                var monthStart = new DateTime(targetYear, targetMonth, 1);
                var monthEnd = monthStart.AddMonths(1);

                var transactions = await db.Transactions.Include(t => t.Category).Where(t => t.Date >= monthStart && t.Date < monthEnd).OrderByDescending(t => t.Date).ToListAsync();

                // Pode Melhorar com o tempo, mas por enquanto é o suficiente para calcular o saldo do período
                var income = transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                var expenses = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                var periodBalance = income - expenses;

                return Results.Ok(new TransactionPeriodResponse
                {
                    Transactions = transactions,
                    PeriodBalance = periodBalance,
                    PeriodStart = monthStart,
                    PeriodEnd = monthEnd
                });
            });

            transactionsGroup.MapGet("/week", async (AppDbContext db, int? year = null, int? week = null) =>
            {
                var now = DateTime.UtcNow;
                var weekStart = now.AddDays(-(int)now.DayOfWeek).Date;
                var weekEnd = weekStart.AddDays(7);

                var transactions = await db.Transactions.Include(t => t.Category).Where(t => t.Date >= weekStart && t.Date < weekEnd).OrderByDescending(t => t.Date).ToListAsync();

                // Pode Melhorar com o tempo, mas por enquanto é o suficiente para calcular o saldo do período
                var income = transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                var expenses = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                var periodBalance = income - expenses;

                return Results.Ok(new TransactionPeriodResponse
                {
                    Transactions = transactions,
                    PeriodBalance = periodBalance,
                    PeriodStart = weekStart,
                    PeriodEnd = weekEnd
                });
            });

            transactionsGroup.MapGet("/{id}", async (AppDbContext db, int id) =>
                await db.Transactions.FindAsync(id) is Transaction transactionFind
                    ? Results.Ok(transactionFind)
                    : Results.NotFound("Transaction not found.")
            );


            transactionsGroup.MapPost("/", async (AppDbContext db, TransactionDTO dto) =>
            {
                var category = await db.Categories.FindAsync(dto.CategoryId);
                if (category is null)
                {
                    return Results.BadRequest("Category not found.");
                }

                var transaction = new FinanceAPI.Models.Transaction
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

            transactionsGroup.MapPut("/{id}", async (AppDbContext db, int id, TransactionDTO newTransaction) =>
            {
                var transactionFind = await db.Transactions.FindAsync(id);
                if (transactionFind is null)
                    return Results.NotFound("Transaction not found.");

                var category = await db.Categories.FindAsync(newTransaction.CategoryId);
                if (category is null)
                    return Results.BadRequest("Category not found.");

                transactionFind.Amount = newTransaction.Amount;
                transactionFind.Name = newTransaction.Name;
                transactionFind.Description = newTransaction.Description;
                transactionFind.Type = newTransaction.Type;
                transactionFind.CategoryId = newTransaction.CategoryId;
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