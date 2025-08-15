using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace BookLibwithSub.Repo.Entities
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public int UserID { get; set; }
        public int? SubscriptionID { get; set; }
        public string TransactionType { get; set; } = null!; 
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        public User User { get; set; } = null!;
        public Subscription? Subscription { get; set; }
    }
}

