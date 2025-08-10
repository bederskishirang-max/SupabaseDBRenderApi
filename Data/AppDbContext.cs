using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;
using PostSQLgreAPI.Model;

namespace PostSQLgreAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
    }

}
