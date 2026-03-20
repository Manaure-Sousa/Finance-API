using Microsoft.EntityFrameworkCore;
using FinanceAPI.Data;
using Finance_API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=finance.db"));

var app = builder.Build();

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

transactionsGroup.MapPost("/", async (AppDbContext db, Transaction transaction) =>
{
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
    transactionFind.Category = newTransaction.Category;
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

app.Run();
