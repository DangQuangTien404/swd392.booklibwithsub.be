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
    public int SubscriptionId { get; set; } 
    public decimal Amount { get; set; } 
    public string Status { get; set; } = "Pending"; 
    public DateTime CreatedAt { get; set; }  
    public Member? Member { get; set; }
    public Subscription? Subscription { get; set; }
}