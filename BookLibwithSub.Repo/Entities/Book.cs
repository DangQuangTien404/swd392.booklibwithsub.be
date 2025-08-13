using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;
    public int? PublicationYear { get; set; }
    public string? Isbn { get; set; }

    // Nav
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public Inventory? Inventory { get; set; }
    public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();
}

