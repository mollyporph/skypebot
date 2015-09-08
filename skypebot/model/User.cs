using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace skypebot.model
{
    public class User
    {
        [Key]
        public string Handle { get; set; }
        public ICollection<Permission> Permissions { get; set; } 
    }

    public class Permission
    {
        [Key]
        public int Id { get; set; }
        public string Uri { get; set; }
    }

    public static class Extensions
    {
        public static string ToPermissionString(this Permission permission)
        {
            return permission.Uri;
        }

        public static IEnumerable<string> ToPermissionStrings(this IEnumerable<Permission> permissions)
        {
            return permissions.Select(x => x.ToPermissionString());
        } 
    }
}
