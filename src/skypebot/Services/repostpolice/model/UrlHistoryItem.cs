using System;

namespace skypebot.Services.repostpolice.model
{
    public class UrlHistoryItem
    {
        public Uri Url { get; set; }
        public string User { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
