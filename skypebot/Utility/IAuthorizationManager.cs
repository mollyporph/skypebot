using System.Collections.Generic;
using System.Linq;
using skypebot.model;

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
        List<User> _inMemoryUsers = new List<User>(); 
        public AuthorizationManager()
        {
          _inMemoryUsers.Add(new User
          {
              Handle = "nattregnet",
              Permissions = new List<Permission>()
              {
                  new Permission
                  {
                      Id = 0,
                      Uri = "couchpotatoservice"
                  },
                  new Permission
                  {
                      Id = 1,
                      Uri = "authorizationservice"
                  }
              }
          });
            _inMemoryUsers.Add(new User
            {
                Handle = "johda155",
                Permissions = new List<Permission>()
              {
                  new Permission
                  {
                      Id = 0,
                      Uri = "couchpotatoservice"
                  }
              }
            });
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
            var authorized = _inMemoryUsers?.FirstOrDefault(x => x.Handle == handle)?.Permissions.Select(p => p.Uri).Contains(permission);
            return authorized.HasValue && authorized.Value;
            //Todo: remove dummy
        }

        public void AddPermission(string handle, string permission)
        {
        }
    }
}
