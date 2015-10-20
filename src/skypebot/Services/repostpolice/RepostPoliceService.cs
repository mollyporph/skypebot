using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skypebot.Services.repostpolice
{

    /*
            private Regex urlRegex = new Regex(@"((https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?)(?:\s)?", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static List<UrlHistoryItem> _urlHistory;
        private static Queue<string> Insults = new Queue<string>();
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
                if (_urlHistory.Any(x => uriResult == x.Url))
                {
                    WarnOfRepost(msg, _urlHistory.FirstOrDefault(x => uriResult == x.Url));
                }
                else
                {
                    SavePost(uriResult, msg.FromDisplayName);
                }
            }
private void SavePost(Uri uriResult, string fromDisplayName)
        {
            _urlHistory.Add(new UrlHistoryItem
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
            _urlHistory.RemoveAll(x => x.PostedAt < DateTime.Now.AddHours(-6));
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

    */
    class RepostPoliceService : IChatBotService
    {
        public int Priority { get; }
        public bool CanHandleCommand(string command)
        {
            return false;
        }

        public void HandleCommand(string fromHandle, string fromDisplayName, string command, string parameters)
        {
        }
    }
}
