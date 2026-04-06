

using FinanceAPI.Models;

namespace FinanceAPI.DTOs
{
    public class TransactionsResponse
    {
        public List<Transaction> Data { get; set; } = new();
        public decimal TotalBalance { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}