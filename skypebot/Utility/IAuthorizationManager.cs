using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repostpolice.Utility
{
    public interface IAuthorizationManager
    {
        void AddPermission(string handle, string permission);
        void RemovePermission(string handle, string permission);
        IEnumerable<string> GetPermissions(string handle);
        bool HasPermission(string handle, string permission);
    }
}
