using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ninject;
using skypebot.Data;
using skypebot.model;
using skypebot.Properties;
using skypebot.Services;
using SKYPE4COMLib;
using Application = System.Windows.Forms.Application;
using Timer = System.Timers.Timer;
using User = skypebot.model.User;

namespace skypebot
{
    public class SysTrayApp : Form
    {


        [STAThread]
        public static void Main()
        {
            Application.Run(new SysTrayApp());
        }
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private static Skype skype;
        private Timer _timer;
        private IChatBot _chatBot;

        public SysTrayApp()
        {
            InitializeTrayApp();

            RecreatePermissionTable();
            RecreateAdminUserIfMissing();

            InitializeChatBotModule();

            InitializeSkypeHook();


        }

        private void InitializeSkypeHook()
        {
            skype = new Skype();
            skype.Attach();
            skype.MessageStatus += _chatBot.ProcessCommand;
        }

        private void InitializeChatBotModule()
        {
            IKernel kernel = new StandardKernel(new ChatBotModule());
            _chatBot = kernel.Get<IChatBot>();
            _chatBot.JoinChat(@"#jonar90/$nattregnet;f00327a27dd370f5");
            _timer = new Timer(2000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _chatBot.PrintMessages(skype.Chat[@"#jonar90/$nattregnet;f00327a27dd370f5"]);
        }

        private void InitializeTrayApp()
        {
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayIcon = new NotifyIcon
            {
                Text = Resources.SysTrayApp_SysTrayApp_Skype_Mambot,
                Icon = new Icon("skypednd.ico", 40, 40),
                BalloonTipTitle = Resources.SysTrayApp_SysTrayApp_Mambot_is_running_minimized_,
                BalloonTipText = Resources.SysTrayApp_SysTrayApp_Remember_to_accept_the_app_in_skype____,
                BalloonTipIcon = ToolTipIcon.Info,
                ContextMenu = trayMenu,
                Visible = true
            };

            trayIcon.ShowBalloonTip(3000);
        }
        private static void RecreateAdminUserIfMissing()
        {
            //Wohoo, EF6-foo 10/10
            using (var ctx = new UserContext())
            {
                var adminUser = ctx.Users.FirstOrDefault(x => x.Handle == "nattregnet");
                var permissions = ctx.Permissions.ToList();
                if (adminUser == null)
                {
                    adminUser = new User()
                    {
                        Handle = "nattregnet",
                        Permissions = permissions
                    };

                    adminUser.Permissions = permissions;
                    ctx.Users.Add(adminUser);
                }
                else
                {
                    if (adminUser.Permissions == null || !adminUser.Permissions.Equals(permissions))
                    {
                        adminUser.Permissions = adminUser.Permissions == null ? permissions : new List<Permission>(adminUser.Permissions.Union(permissions));

                    }
                    //Coherence is wierd :/

                    ctx.Entry(adminUser).State = EntityState.Modified;
                }
                ctx.SaveChanges();

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
                objCtx.ExecuteStoreCommand("DELETE FROM Permissions " +
                                           "DBCC CHECKIDENT ('Permissions', RESEED, 0)");

                serviceTypes.ForEach(x => ctx.Permissions.Add(new Permission() { Uri = x }));
                ctx.SaveChanges();
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }


    }
}
