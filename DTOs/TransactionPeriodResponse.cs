using FinanceAPI.Models;

namespace FinanceAPI.DTOs
{
    public class TransactionPeriodResponse
    {
        public List<Transaction> Transactions { get; set; } = new();
        public decimal PeriodBalance { get; set; }    // Saldo do período
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}