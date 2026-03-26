using FinanceAPI.Models.Enums;

namespace Finance_API.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TransactionType Type { get; set; } = TransactionType.Expense;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public DateTime Date { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}