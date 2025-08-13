using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class LoanItem
{
    public int Id { get; set; }

    public int LoanId { get; set; }
    public int BookId { get; set; }

    public DateTime? ReturnDate { get; set; }
    public decimal FineAmount { get; set; } // precision set in modelBuilder
    public bool IsLost { get; set; }

    // Nav
    public Loan Loan { get; set; } = default!;
    public Book Book { get; set; } = default!;
    public ICollection<AccountLedger> LedgerLinks { get; set; } = new List<AccountLedger>();
}

