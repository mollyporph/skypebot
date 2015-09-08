using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using skypebot.Properties;
using skypebot.Services.repostpolice.model;
using SKYPE4COMLib;
using Application = System.Windows.Forms.Application;
using Timer = System.Timers.Timer;

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
        private static Timer timer;
        private static Skype skype;


        public SysTrayApp()
        {
            InitializeTrayApp();
            ChatBot.JoinChat("tbd");
            skype = new Skype();
            skype.Attach();
            skype.MessageStatus += skype_MessageStatus;
           
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

        private void skype_MessageStatus(ChatMessage msg, TChatMessageStatus status)
        {
            if (TChatMessageStatus.cmsRead == status)
            {
                return;
            }
            ChatBot.ProcessCommand(msg);
        

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
