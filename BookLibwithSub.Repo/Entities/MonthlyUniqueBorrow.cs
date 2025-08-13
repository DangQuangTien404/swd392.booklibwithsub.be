using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class MonthlyUniqueBorrow
{
    public int MemberId { get; set; }
    public string YearMonth { get; set; } = default!; // "YYYY-MM" e.g., "2025-08"
    public int BookId { get; set; }

    public DateTime FirstBorrowedAt { get; set; }

    // Nav
    public Member Member { get; set; } = default!;
    public Book Book { get; set; } = default!;
}

