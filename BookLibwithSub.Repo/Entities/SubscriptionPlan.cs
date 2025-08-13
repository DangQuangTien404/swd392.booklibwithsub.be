using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class SubscriptionPlan
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;
    public decimal Price { get; set; }              // precision set in modelBuilder
    public int MonthlyUniqueTitles { get; set; }    // e.g., 10
    public int DailyMaxLoans { get; set; }          // e.g., 2
    public bool AllowRepeatSameTitleInMonth { get; set; } = true;

    // Nav
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}

