using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookLibwithSub.Repo.Entities;

public class SystemAccount
{
    public int Id { get; set; }

    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!; // keep simple as requested
    public string Role { get; set; } = "Librarian";  // Admin/Librarian
    public string Status { get; set; } = "Active";   // Active/Disabled
}

