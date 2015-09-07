using System.Collections.Generic;

namespace skypebot.Utility
{
    public interface IAuthorizationManager
    {
        void AddPermission(string handle, string permission);
        void RemovePermission(string handle, string permission);
        IEnumerable<string> GetPermissions(string handle);
        bool HasPermission(string handle, string permission);
    }
}
