using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using TestWebApi.Models;
using System.Threading.Tasks;
namespace TestWebApi.Context
{
    public class DbFileContext : DbContext
    {
        public DbSet<DbFile> Files => Set<DbFile>();
        public DbSet<User> Users =>Set<User>();

        public DbFileContext(DbContextOptions<DbFileContext> options):base(options)
        {
            Database.EnsureCreated();
        }
    }
}
