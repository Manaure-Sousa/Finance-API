using System.ComponentModel.DataAnnotations;
using Finance_API.Models;
using FinanceAPI.Models.Enums;

namespace FinanceAPI.DTOs
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TransactionType Type { get; set; } = TransactionType.Expense;

        public int CategoryId { get; set; }

        public TransactionDTO() { }
        public TransactionDTO(Transaction t) =>
        (Id, Amount, Name, Description, Type, CategoryId) =
        (t.Id, t.Amount, t.Name, t.Description, t.Type, t.CategoryId);

    }
}