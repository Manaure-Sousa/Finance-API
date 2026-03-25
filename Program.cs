using Microsoft.EntityFrameworkCore;
using FinanceAPI.Data;
using FinanceAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=finance.db"));

var app = builder.Build();

app.MapTransactionsEndpoints();

app.Run();
