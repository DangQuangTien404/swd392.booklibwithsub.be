using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace BookLibwithSub.Repo.Entities
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public int PublishedYear { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        // Navigation
        public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();
    }
}
