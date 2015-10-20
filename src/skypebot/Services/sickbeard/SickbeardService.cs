using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using skypebot.Services.sickbeard.model;
using skypebot.Utility;

namespace skypebot.Services.sickbeard
{
    class SickbeardService : IChatBotService
    {
        private string[] _commands = new[] {"!getseries", "!addseries"};
        private IChatBot _chatBot;
        private IAuthorizationManager _authorizationManager;
        private string sickBeardUrl;
        private string sickBeardApiKey;
        private static Dictionary<string, List<string>> _userAllowedSeriesIds; 
        public SickbeardService(IAuthorizationManager authorizationManager, IChatBot chatBot)
        {
            _authorizationManager = authorizationManager;
            _chatBot = chatBot;
            sickBeardUrl = ConfigurationManager.AppSettings["sickbeardurl"];
            sickBeardApiKey = ConfigurationManager.AppSettings["sickbeardapikey"];
            _userAllowedSeriesIds = new Dictionary<string, List<string>>();
        }

        public bool CanHandleCommand(string command)
        {
            return _commands.Contains(command);
        }

        public void HandleCommand(string fromHandle, string fromDisplayName, string command, string parameters)
        {
            //Cap input parameters to reduce overflow
            var minimumCapLength = Math.Min(parameters.Length, 150);
            parameters = parameters.Substring(0, minimumCapLength);
            if (!_authorizationManager.HasPermission(fromHandle, this.GetType().Name.ToLower())) return;
            if (string.IsNullOrWhiteSpace(parameters)) return;
            switch (command)
            {
                case "!getseries":
                    TrySearchSeries(fromHandle, fromDisplayName, parameters);
                    break;
                case "!addseries":
                    TryAddSeries(fromHandle, fromDisplayName, parameters);
                    break;
            }
        }

        private async void TrySearchSeries(string fromHandle, string fromDisplayName, string parameters)
        {
            await Task.Run(() => SearchSeries(fromHandle, fromDisplayName, parameters));
        }
        private async void TryAddSeries(string fromHandle, string fromDisplayName, string parameters)
        {
            await Task.Run(() => AddSeries(fromHandle, fromDisplayName, parameters));
        }
        private async void SearchSeries(string fromHandle, string fromDisplayName, string parameters)
        {

            var Query = $"/?cmd=sb.searchtvdb&name={parameters}";
            using (var httpClient = new HttpClient())
            {
                var res = await httpClient.GetStringAsync(sickBeardUrl+sickBeardApiKey + Query);
                var seriesResults = Newtonsoft.Json.JsonConvert.DeserializeObject<SickbeardModel>(res).data.results.OrderByDescending(x => x.first_aired).Take(4);
                var sickbeardSeries = seriesResults as SickbeardSeries[] ?? seriesResults.ToArray().Where(x => x.name != @"** 403: Series Not Permitted **");
                if (!sickbeardSeries.Any())
                {
                    _chatBot.EnqueueMessage($"{fromDisplayName}: Couldn't find any series for \"{parameters}\"");
                    return;
                }
                _userAllowedSeriesIds[fromHandle] = new List<string>(sickbeardSeries.Select(x => x.tvdbid).ToList());

                _chatBot.EnqueueMessage($"{fromDisplayName}: found the following series," +
                                        $" add one using !addseries <tvbdid>\n"+ 
                                        $"{string.Join("\n",sickbeardSeries.Select(x => $"{x.name}({x.first_aired.GetValueOrDefault().ToString("yyyy-MM-dd")}) ID: {x.tvdbid}") )}");

            }
        }

       
        private async void AddSeries(string fromHandle, string fromDisplayName, string parameters)
        {
            if (!Regex.IsMatch(parameters, @"\d{4,10}")) return;
            //Must have searched for it beforehand
            if (!_userAllowedSeriesIds.ContainsKey(fromHandle)) return;
            if (!_userAllowedSeriesIds[fromHandle].Contains(parameters)) return;
            var Query = $"/?cmd=show.addnew&tvdbid={parameters}&initial=hdtv|rawhdtv|fullhdtv|hdwebdl|fullhdwebdl|hdbluray|fullhdbluray&status=wanted";
            using (var httpClient = new HttpClient())
            {
                var res = await httpClient.GetStringAsync(sickBeardUrl + sickBeardApiKey + Query);
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SickbeardAddModel>(res);
                _chatBot.EnqueueMessage(result.result == "success"
                    ? $"{fromDisplayName}: Your series was successfully added!"
                    : $"{fromDisplayName}: Failed to add your series (it might already exist?)");
            }
           
        }
    }
}
