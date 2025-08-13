using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class BookAuthor
{
    public int BookId { get; set; }
    public int AuthorId { get; set; }

    // Nav
    public Book Book { get; set; } = default!;
    public Author Author { get; set; } = default!;
}

