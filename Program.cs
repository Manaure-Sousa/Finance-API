using Microsoft.EntityFrameworkCore;
using FinanceAPI.Data;
using Finance_API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=finance.db"));

var app = builder.Build();

app.Run();
