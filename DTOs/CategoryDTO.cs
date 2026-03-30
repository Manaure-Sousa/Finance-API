using FinanceAPI.Models.Enums;

namespace FinanceAPI.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public TransactionType? Type { get; set; }
    }
}