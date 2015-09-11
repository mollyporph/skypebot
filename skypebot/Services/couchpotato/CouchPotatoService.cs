using System;
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
        private readonly IChatBot _chatBot;

        public CouchPotatoService(IAuthorizationManager authorizationManager,IChatBot chatBot)
        {
            _couchpotatoApiKey = ConfigurationManager.AppSettings["couchpotato_apikey"];
            _couchpotatoUrl = ConfigurationManager.AppSettings["couchpotato_url"];
            _userAllowedMovieIds = new Dictionary<string, List<string>>();
            _authorizationManager = authorizationManager;
            _chatBot = chatBot;
        }

        public bool CanHandleCommand(string command)
        {
            return Commands.Contains(command);
        }

        public void HandleCommand(string fromHandle, string fromDisplayName, string command, string parameters)
        {
            var minimumCapLength = Math.Min(parameters.Length, 150);
            parameters = parameters.Substring(0, minimumCapLength);
            var hasPermission = _authorizationManager.HasPermission(fromHandle, this.GetType().Name.ToLower());
            if (!hasPermission) return;
            if (string.IsNullOrWhiteSpace(parameters)) return;
            switch (command)
            {
                case "!addmovie":
                    TryAddMovie(fromHandle, fromDisplayName, parameters.Split(' ')[0]);
                    break;
                case "!getmovie":
                    TrySearchMovie(fromHandle, fromDisplayName, parameters);
                    break;
                default:
                    return;

            }
        }

        private async void TrySearchMovie(string fromHandle, string fromDisplayName, string parameters)
        {
            await Task.Run(() => SearchMovie(fromHandle, fromDisplayName, parameters));
        }

        private async void TryAddMovie(string fromHandle, string fromDisplayName, string movie)
        {
            
            var movieId = movie.ToLower();

            if (movieId.StartsWith("tt"))
            {
                await Task.Run(() => AddMovie(fromHandle, fromDisplayName, movieId));
            }

        }

        private async void SearchMovie(string fromHandle,string fromDisplayName, string movie)
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
                if (content.Movies == null)
                {

                    _chatBot.EnqueueMessage($"Could not find movie {movie} :(");

                    return;
                };
                //Cap movies
                var cappedMovies = content.Movies.OrderByDescending(x => x.year).Take(4).ToList();
                _userAllowedMovieIds[fromHandle] = new List<string>(cappedMovies.Select(x => x.imdb).ToList());

                _chatBot.EnqueueMessage(
                    $"{fromDisplayName}: Found the following movies, please add one using \"!addmovie tt0000000\" from imdb-id below\n" +
                    $"{string.Join("\n", cappedMovies.Select(x => $"ImdbId: {x.imdb} Title: {x.original_title} Year: {x.year} ImdbUrl: {x.ImdbUrl}").ToList())}");
            }
        }

        private async void AddMovie(string fromHandle, string fromDisplayName, string movieIdentifier, string quality = BestQuality)
        {

            if (!Regex.IsMatch(movieIdentifier, @"tt\d{7}")) return;
            if (!_userAllowedMovieIds[fromHandle].Contains(movieIdentifier)) return;
            using (var httpClient = new HttpClient())
            {
                const string command = "/movie.add?";

                var res = await
                    httpClient.GetStringAsync(
                        $"{_couchpotatoUrl}{_couchpotatoApiKey}{command}identifier={movieIdentifier}&profile_id={quality}");
                dynamic content = JsonConvert.DeserializeObject<dynamic>(res);
                if ((bool)content["success"])
                {

                    _chatBot.EnqueueMessage($"{fromDisplayName}: Your movie was successfully added!");
                }
            }
        }
    }
}
