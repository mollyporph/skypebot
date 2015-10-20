
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using skypebot.Data;
using skypebot.model;
using skypebot.Services;

namespace skypebot.Utility
{
    public interface IAuthorizationManager
    {
        bool AddPermission(string handle, string permission);
        bool RemovePermission(string handle, string permission);
        IEnumerable<string> GetPermissions(string handle);
        bool HasPermission(string handle, string permission);
    }

    public class AuthorizationManager : IAuthorizationManager
    {
        List<User> _inMemoryUsers = new List<User>(); 
        public AuthorizationManager()
        {

            
        }

       

        public bool RemovePermission(string handle, string permission)
        {
            using (var ctx = new UserContext())
            {
                var user = ctx.Users.FirstOrDefault(x => x.Handle == handle);
                if (user == null) return false;
                if (!user.Permissions.ToPermissionStrings().Contains(permission)) return false;
                user.Permissions.Remove(user.Permissions.FirstOrDefault(x => x.Uri == permission));
                ctx.SaveChanges();
                return true;
            }
        }

        public IEnumerable<string> GetPermissions(string handle)
        {
            using (var ctx = new UserContext())
            {
                var user = ctx.Users.FirstOrDefault(x => x.Handle == handle);
                return user?.Permissions?.Select(x => x.Uri).ToList();
            }
        }

        public bool HasPermission(string handle, string permission)
        {
            using (var ctx = new UserContext())
            {
                var firstOrDefault = ctx.Users.Include("Permissions").FirstOrDefault(user => user.Handle == handle);
                return
                    firstOrDefault != null && firstOrDefault
                        .Permissions.Select(p => p.Uri)
                        .Contains(permission.ToLower());
            }
        }

        public bool AddPermission(string handle, string permission)
        {
            using (var ctx = new UserContext())
            {
                var user = ctx.Users.Include("Permissions").FirstOrDefault(x => x.Handle == handle);
                if (user == null)
                {
                    user = new User
                    {
                        Handle = handle,
                        Permissions = ctx.Permissions.Where(x => x.Uri == permission).ToList()
                    };
                    ctx.Users.Add(user);
                }
                else
                {
                    //User already has permission
                    if (user.Permissions.ToPermissionStrings().Contains(permission)) return false;
                    var permissionToAdd = ctx.Permissions.FirstOrDefault(x => x.Uri == permission);
                    user.Permissions.Add(permissionToAdd);
                }
                
                ctx.SaveChanges();
                return true;
            }
        }
    }
}
