using System.ComponentModel.DataAnnotations;
using FinanceAPI.Models;
using FinanceAPI.Models.Enums;

namespace FinanceAPI.DTOs
{
    public class TransactionDTO
    {
        public int? Id { get; set; }
        [Required]
        [Range(typeof(decimal), "0.01", "9999999999.99",
            ErrorMessage = "Amount must be between 0.01 and 9,999,999,999.99")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [MaxLength(200)]
        public string? Name { get; set; }
        [MaxLength(300)]
        public string? Description { get; set; }
        [EnumDataType(typeof(TransactionType))]
        public TransactionType Type { get; set; } = TransactionType.Expense;
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public TransactionDTO() { }
        public TransactionDTO(Transaction t) =>
        (Id, Amount, Name, Description, Type, CategoryId, Date) =
        (t.Id, t.Amount, t.Name, t.Description, t.Type, t.CategoryId, t.Date);

    }
}