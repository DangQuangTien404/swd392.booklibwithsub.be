using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace BookLibwithSub.Repo.Entities
{
    public class SubscriptionPlan
    {
        public int SubscriptionPlanID { get; set; }
        public string PlanName { get; set; }
        public int DurationDays { get; set; }
        public int MaxPerDay { get; set; }
        public int MaxPerMonth { get; set; }
        public decimal Price { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}

