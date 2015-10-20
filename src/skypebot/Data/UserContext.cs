using System.Collections.Generic;
using System.Data.Entity;
using skypebot.model;

namespace skypebot.Data
{
    public class UserContext : DbContext
    {
#if DEBUG
#else
        public UserContext() : base("UserContext")
        {
            
        }
#endif
        public DbSet<User> Users { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(user => user.Permissions)
                .WithMany(permission => permission.Users)
                .Map(m =>
                {
                    m.MapLeftKey("Handle");
                    m.MapRightKey("Id");
                    m.ToTable("UserPermissions");

                });
        }

    }
}
