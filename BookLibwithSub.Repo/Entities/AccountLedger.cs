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
    public string EntryType { get; set; } = string.Empty; 
    public decimal Amount { get; set; } 
    public string Currency { get; set; } = "VND";
    public int? RefLoanItemId { get; set; } 
    public string? Note { get; set; }

    public Member? Member { get; set; }
    public LoanItem? RefLoanItem { get; set; }
}