using FinanceAPI.Models.Enums;

namespace FinanceAPI.DTOs
{
    public class CreateCategoryDTO
    {
        public string Name { get; set; } = null!;
        public TransactionType? Type { get; set; }
    }
}