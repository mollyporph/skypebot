using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace skypebot.model
{
    public class User
    {
        [Key]
        public string Handle { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; } 
    }

    public class Permission
    {
        [Key]
        public int Id { get; set; }
        public string Uri { get; set; }
    }
}
