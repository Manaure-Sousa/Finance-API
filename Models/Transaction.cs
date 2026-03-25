using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance_API.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}