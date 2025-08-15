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
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string Status { get; set; }

        // Navigation
        public Loan Loan { get; set; }
        public Book Book { get; set; }
    }
}
