using System.Data.Entity;
using repostpolice.model;

namespace skypebot.Data
{
    public class UserContext : DbContext 
    {
        public DbSet<User> Users { get; set; }
    }
}
