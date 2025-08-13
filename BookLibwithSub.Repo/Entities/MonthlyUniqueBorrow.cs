using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class MonthlyUniqueBorrow
{
    public int MemberId { get; set; }
    public string YearMonth { get; set; } = string.Empty; 
    public int BookId { get; set; }
    public DateTime FirstBorrowedAt { get; set; }

    public Member? Member { get; set; }
    public Book? Book { get; set; }
}