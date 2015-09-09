using System;
using System.Drawing;
using System.Windows.Forms;
using Ninject;
using skypebot.Properties;
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
        private static Skype skype;
        private ChatBot chatBot;
        private Timer _timer;

        public SysTrayApp()
        {
            InitializeTrayApp();

            InitializeChatBotModule();

            InitializeSkypeHook();


        }

        private void InitializeSkypeHook()
        {
            skype = new Skype();
            skype.Attach();
            skype.MessageStatus += chatBot.ProcessCommand;
        }

        private void InitializeChatBotModule()
        {
            IKernel kernel = new StandardKernel(new ChatBotModule());
            chatBot = kernel.Get<ChatBot>();
            chatBot.JoinChat(@"#jonar90/$nattregnet;f00327a27dd370f5");
            _timer = new Timer(2000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            chatBot.PrintMessages(skype.Chat[@"#jonar90/$nattregnet;f00327a27dd370f5"]);
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
