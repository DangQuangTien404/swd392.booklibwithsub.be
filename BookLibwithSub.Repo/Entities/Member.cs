using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibwithSub.Repo.Entities;

public class Member
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public string Status { get; set; } = "Active"; // Active/Blocked

    public DateTime? MembershipStartDate { get; set; }
    public DateTime? MembershipEndDate { get; set; }

    // Nav
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<AccountLedger> LedgerEntries { get; set; } = new List<AccountLedger>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

