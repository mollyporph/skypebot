using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using skypebot.Services.couchpotato.model;
using skypebot.Utility;

namespace skypebot.Services.couchpotato
{
    public class CouchPotatoService : IChatBotService
    {
        private readonly Dictionary<string, List<string>> _userAllowedMovieIds;
        public string[] Commands { get; } = { "!addmovie", "!getmovie" };
        private const string BestQuality = "6a1d1912dc7a48c6819ac95053892932";
        private readonly string _couchpotatoApiKey;
        private readonly string _couchpotatoUrl;
        private readonly IAuthorizationManager _authorizationManager;

        public CouchPotatoService(IAuthorizationManager authorizationManager1)
        {
            _couchpotatoApiKey = ConfigurationManager.AppSettings["couchpotato_apikey"];
            _couchpotatoUrl = ConfigurationManager.AppSettings["couchpotato_url"];
            _userAllowedMovieIds = new Dictionary<string, List<string>>();
            Priority = 1;
            _authorizationManager = authorizationManager1;
        }

        public int Priority { get; private set; }
        public bool CanHandleCommand(string command)
        {
            return Commands.Contains(command);
        }

        public void HandleCommand(string fromHandle, string fromDisplayName, string command, string parameters)
        {
            if (!_authorizationManager.HasPermission(fromHandle, this.GetType().Name.ToLower())) return;
            switch (command)
            {
                case "addmovie":
                    TryAddMovie(fromHandle, fromDisplayName, parameters.Split(' ')[0]);
                    break;
                case "getmovie":
                    break;
                default:
                    return;

            }
        }

        private async void TryAddMovie(string fromHandle, string fromDisplayName, string movie)
        {
            
            var movieId = movie.Substring(0, 100).Trim(' ').ToLower();

            if (movieId.StartsWith("tt"))
            {
                await Task.Run(() => AddMovie(fromHandle, fromDisplayName, movieId));
            }
            else
            {
                await Task.Run(() => SearchMovie(fromDisplayName,fromHandle, movie));
            }
        }

        private async void SearchMovie(string fromDisplayName,string fromHandle, string movie)
        {
            using (var httpClient = new HttpClient())
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                const string command = "/search?";
                var res =
                    await
                        httpClient.GetStringAsync($"{_couchpotatoUrl}{_couchpotatoApiKey}{command}q={movie}&type=movie");
                var content = JsonConvert.DeserializeObject<MoviesModel>(res, jsonSerializerSettings);
                if (content.Movies == null) return;
                _userAllowedMovieIds[fromHandle] = new List<string>(content.Movies.Select(x => x.imdb).ToList());
                ChatBot.EnqueueMessage(
                    $"{fromDisplayName}: Found the following movies, please add one using \"!addmovie tt0000000\" from imdb-id below\n" +
                    $"{string.Join("\n", content.Movies.Select(x => $"Title: {x.original_title} ImdbId: {x.imdb} Year: {x.year} ImdbUrl: {x.ImdbUrl}").ToList())}");
            }
        }

        private async void AddMovie(string fromHandle, string fromDisplayName, string movieIdentifier, string quality = BestQuality)
        {

            if (!Regex.IsMatch(movieIdentifier, @"tt\d{7}")) return;
            if (!_userAllowedMovieIds[fromHandle].Contains(movieIdentifier)) return;
            using (HttpClient httpClient = new HttpClient())
            {
                const string command = "/movie.add?";

                var res = await
                    httpClient.GetStringAsync(
                        $"{_couchpotatoUrl}{_couchpotatoApiKey}{command}identifier={movieIdentifier}&profile_id={quality}");
                dynamic content = JsonConvert.DeserializeObject<dynamic>(res);
                if (content.success)
                {

                    ChatBot.EnqueueMessage($"{fromDisplayName}: Your movie was successfully added!");
                }
            }
        }
    }
}
