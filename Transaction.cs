
namespace ZarządzanieFinansami;

public class Transaction
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        public Transaction(string category, decimal amount, DateTime date, string description)
        {
            Category = category;
            Amount = amount;
            Date = date;
            Description = description;
        }
    }

