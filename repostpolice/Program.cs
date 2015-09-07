using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using SKYPE4COMLib;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Application = System.Windows.Forms.Application;
using Timer = System.Timers.Timer;

namespace Mambot
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
        private Regex urlRegex = new Regex(@"((https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?)(?:\s)?", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static List<UrlHistoryItem> UrlHistory;
        private static Queue<string> Insults = new Queue<string>();

        public SysTrayApp()
        {
            UrlHistory = new List<UrlHistoryItem>();
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Skype Mambot";
            trayIcon.Icon = new Icon("skypednd.ico", 40, 40);
            trayIcon.BalloonTipTitle =
                "Mambot is running minimized!";
            trayIcon.BalloonTipText = "Remember to accept the app in skype! :)";
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
            trayIcon.ShowBalloonTip(3000);
            skype = new Skype();
            skype.Attach();
            DoCleanUpAndRefill();
            timer = new Timer(60000);
            timer.Elapsed += (sender, eventArgs) => DoCleanUpAndRefill();
            timer.Enabled = true;

            skype.MessageStatus += new _ISkypeEvents_MessageStatusEventHandler(skype_MessageStatus);
        }
        private void skype_MessageStatus(ChatMessage msg, TChatMessageStatus status)
        {
            if (TChatMessageStatus.cmsRead == status)
            {
                return;
            }
            var results = urlRegex.Matches(msg.Body);
            if(msg.Body.Contains("\r\n\r\n<<<"))
            {
                return;
            }
            foreach (var res in results)
            {
                var truncRgx = new Regex(@"(\s.*)*");
                var trunctedRes = truncRgx.Replace(res.ToString(), "");
                Uri uriResult;
                if (!Uri.TryCreate(res.ToString(), UriKind.Absolute, out uriResult) ||
                    uriResult.Scheme != Uri.UriSchemeHttp)
                    continue;
                if (UrlHistory.Any(x => uriResult == x.Url))
                {
                    WarnOfRepost(msg, UrlHistory.FirstOrDefault(x => uriResult == x.Url));
                }
                else
                {
                    SavePost(uriResult, msg.FromDisplayName);
                }
            }

        }

        private void SavePost(Uri uriResult, string fromDisplayName)
        {
            UrlHistory.Add(new UrlHistoryItem
            {
                PostedAt = DateTime.Now,
                Url = uriResult,
                User = fromDisplayName
            });
        }

        private void WarnOfRepost(ChatMessage msg, UrlHistoryItem item)
        {

            msg.Chat.SendMessage(
                $"{msg.FromDisplayName}: That link is a repost from {item.User} at {item.PostedAt}, {Insults.Dequeue()}");
        }


        private static void DoCleanUpAndRefill()
        {
            if (Insults.Count < 5)
            {
                DownLoadMoreInsults();
            }
            UrlHistory.RemoveAll(x => x.PostedAt < DateTime.Now.AddHours(-6));
        }

        private static void DownLoadMoreInsults()
        {
            for (var i = 0; i < 20; i++)
            {
                using (var wc = new WebClient())
                {
                    var content = JsonConvert.DeserializeObject<Insult>(wc.DownloadString("Http://quandyfactory.com/insult/json"));
                    Insults.Enqueue(content.insult);
                }
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
