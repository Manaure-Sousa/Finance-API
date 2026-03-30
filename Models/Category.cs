using FinanceAPI.DTOs;
using FinanceAPI.Models.Enums;

namespace FinanceAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public TransactionType? Type { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public Category() { }
        public Category(CreateCategoryDTO c) => (Name, Type) = (c.Name, c.Type);
    }
}