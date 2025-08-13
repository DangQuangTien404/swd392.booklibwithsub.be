using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class AccountLedger
{
    public int Id { get; set; }

    public int MemberId { get; set; }
    public DateTime EntryDate { get; set; }
    public string EntryType { get; set; } = default!; // Payment/Fine/Adjustment
    public decimal Amount { get; set; }               // precision set in modelBuilder
    public string Currency { get; set; } = "USD";
    public int? RefLoanItemId { get; set; }
    public string? Note { get; set; }

    // Nav
    public Member Member { get; set; } = default!;
    public LoanItem? RefLoanItem { get; set; }
}

