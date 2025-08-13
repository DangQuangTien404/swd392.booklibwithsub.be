using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    // Nav
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}

