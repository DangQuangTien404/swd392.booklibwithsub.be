using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace BookLibwithSub.Repo.Entities
{
    public class Loan
    {
        public int LoanID { get; set; }
        public int SubscriptionID { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime ReturnDueDate { get; set; }

        public Subscription Subscription { get; set; } = null!;
        public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();
    }
}
