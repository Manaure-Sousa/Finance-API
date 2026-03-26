using Finance_API.Models;
using FinanceAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Finance_API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public TransactionType? Type { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}