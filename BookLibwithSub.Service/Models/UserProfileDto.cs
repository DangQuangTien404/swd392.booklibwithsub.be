using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibwithSub.Service.Models
{
    public sealed class UserProfileDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public CurrentSubscriptionDto? CurrentSubscription { get; set; }
    }

    public sealed class CurrentSubscriptionDto
    {
        public int SubscriptionId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string Status { get; set; } = "Inactive"; // Active/Inactive
        public DateTime? EndDate { get; set; }
    }
}

