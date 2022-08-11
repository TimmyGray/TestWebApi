using Microsoft.EntityFrameworkCore;
using TestWebApi.Models;
namespace TestWebApi.Context
{
    public class DbFileContext : DbContext
    {
        public DbSet<DbFile> Files => Set<DbFile>();
        public DbSet<User> Users =>Set<User>();
        public DbSet<DataFile> DataFiles => Set<DataFile>();

        public DbFileContext(DbContextOptions<DbFileContext> options):base(options)
        {
          //  Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
