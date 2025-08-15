using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace BookLibwithSub.Repo.Entities
{
    public class LoanItem
    {
        public int LoanItemID { get; set; }
        public int LoanID { get; set; }
        public int BookID { get; set; }
        public DateTime? ReturnedDate { get; set; }

        public Loan Loan { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}
