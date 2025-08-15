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
        public string Title { get; set; } = null!;
        public string AuthorName { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();
    }
}
