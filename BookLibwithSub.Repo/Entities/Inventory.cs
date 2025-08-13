using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class Inventory
{
    // One-to-one with Book: PK == FK
    public int BookId { get; set; }

    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public int ReservedCopies { get; set; }

    public byte[]? RowVersion { get; set; } // concurrency

    // Nav
    public Book Book { get; set; } = default!;
}

