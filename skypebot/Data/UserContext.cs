using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using repostpolice.model;

namespace repostpolice.Data
{
    public class UserContext : DbContext 
    {
        public DbSet<User> Users { get; set; }
    }
}
