using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skypebot.Utility
{
    public class DummyAuthorizationManager : IAuthorizationManager
    {
        public bool AddPermission(string handle, string permission)
        {
            return true;
        }

        public bool RemovePermission(string handle, string permission)
        {
            return true;
        }

        public IEnumerable<string> GetPermissions(string handle)
        {
            return new List<string>();
        }

        public bool HasPermission(string handle, string permission)
        {
            return true;
        }
    }
}
