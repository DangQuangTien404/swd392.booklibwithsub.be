using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookLibwithSub.Repo
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost;Database=BookLibDb;User Id=sa;Password=12345;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True")
                .Options;

            return new AppDbContext(options);
        }
    }
}
