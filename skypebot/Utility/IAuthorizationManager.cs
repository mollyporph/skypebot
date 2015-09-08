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

    public class AuthorizationManager : IAuthorizationManager
    {
        public AuthorizationManager()
        {
            
        }

        public void RemovePermission(string handle, string permission)
        {
        }

        public IEnumerable<string> GetPermissions(string handle)
        {
            return new List<string>();
        }

        public bool HasPermission(string handle, string permission)
        {
            //Todo: remove dummy
            return true;
        }

        public void AddPermission(string handle, string permission)
        {
        }
    }
}
