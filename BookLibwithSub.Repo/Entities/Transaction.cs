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
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }

        public User User { get; set; }
        public Subscription Subscription { get; set; }
    }
}

