using Microsoft.EntityFrameworkCore;
using FinanceAPI.Models;
using FinanceAPI.Models.Enums;

namespace FinanceAPI.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Alimentação", Type = TransactionType.Expense },
                new Category { Id = 2, Name = "Transporte", Type = TransactionType.Expense },
                new Category { Id = 3, Name = "Salário", Type = TransactionType.Income },
                new Category { Id = 4, Name = "Investimentos", Type = TransactionType.Investment },
                new Category { Id = 5, Name = "Lazer", Type = TransactionType.Expense },
                new Category { Id = 6, Name = "Moradia", Type = TransactionType.Expense },
                new Category { Id = 7, Name = "Saúde", Type = TransactionType.Expense },
                new Category { Id = 8, Name = "Educação", Type = TransactionType.Expense },
                new Category { Id = 9, Name = "Assinaturas", Type = TransactionType.Expense },
                new Category { Id = 10, Name = "Compras", Type = TransactionType.Expense },
                new Category { Id = 11, Name = "Freelance", Type = TransactionType.Income },
                new Category { Id = 12, Name = "Dividendos", Type = TransactionType.Investment },
                new Category { Id = 13, Name = "Rendimentos", Type = TransactionType.Investment },
                new Category { Id = 14, Name = "Outros", Type = TransactionType.Expense }
            );
        }
    }
}