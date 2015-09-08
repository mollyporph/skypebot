
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

            RecreatePermissionTable();
            RecreateAdminUserIfMissing();
            
        }

        private static void RecreateAdminUserIfMissing()
        {
            using (var ctx = new UserContext())
            {
                if (ctx.Users.FirstOrDefault(x => x.Handle == "nattregnet") != null) return;
                var adminUser = new User()
                {
                    Handle = "nattregnet",
                    Permissions = new List<Permission>()
                };

                var permissions = ctx.Permissions.ToList();
                adminUser.Permissions = permissions;
                ctx.Users.Add(adminUser);
            }
        }

        private static void RecreatePermissionTable()
        {

            //Reflection : Get all coherent types from IChatBotService and add them as permission-objects in db
            var type = typeof(IChatBotService);
            var serviceTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface).Select(x => x.Name.ToLower()).ToList();


            using (var ctx = new UserContext())
            {
                var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)ctx).ObjectContext;
                objCtx.ExecuteStoreCommand("TRUNCATE TABLE [Permissions]");

                serviceTypes.ForEach(x => ctx.Permissions.Add(new Permission() {Uri = x}));
                ctx.SaveChanges();
            }

        }

        public void RemovePermission(string handle, string permission)
        {
            using (var ctx = new UserContext())
            {
                var user = ctx.Users.FirstOrDefault(x => x.Handle == handle);
                if (user == null) return;
                if (!user.Permissions.ToPermissionStrings().Contains(permission)) return;
                user.Permissions.Remove(user.Permissions.FirstOrDefault(x => x.Uri == permission));
                ctx.SaveChanges();
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
                var firstOrDefault = ctx.Users.FirstOrDefault(user => user.Handle == handle);
                return
                    firstOrDefault != null && firstOrDefault
                        .Permissions.Select(p => p.Uri)
                        .Contains(permission);
            }
        }

        public void AddPermission(string handle, string permission)
        {
            using (var ctx = new UserContext())
            {
                var user = ctx.Users.FirstOrDefault(x => x.Handle == handle);
                if (user == null) return;
                if (user.Permissions.ToPermissionStrings().Contains(permission)) return;
                user.Permissions.Add(new Permission {Uri = permission});
                ctx.SaveChanges();
            }
        }
    }
}
