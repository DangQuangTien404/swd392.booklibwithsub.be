using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class MemberDailyBorrow
{
    public int MemberId { get; set; }
    public DateTime LocalDate { get; set; } // date-only semantics

    public int BorrowCount { get; set; }

    // Nav
    public Member Member { get; set; } = default!;
}

