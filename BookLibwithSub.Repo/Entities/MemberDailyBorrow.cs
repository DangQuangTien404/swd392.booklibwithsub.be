using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class MemberDailyBorrow
{
    public int MemberId { get; set; } 
    public DateOnly LocalDate { get; set; } 
    public int BorrowCount { get; set; }

    public Member? Member { get; set; }
}


