using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class Payment
{
    public int Id { get; set; }

    public int MemberId { get; set; }
    public int? SubscriptionId { get; set; } // usually set for sub payments

    public decimal Amount { get; set; } // precision set in modelBuilder
    public string Status { get; set; } = "Pending"; // Pending/Success/Failed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Nav
    public Member Member { get; set; } = default!;
    public Subscription? Subscription { get; set; }
}

