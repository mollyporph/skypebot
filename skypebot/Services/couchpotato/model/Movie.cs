namespace skypebot.Services.couchpotato.model
{
    public class Movie
    {
        public string original_title { get; set; }
        public string imdb { get; set; }
        public string year { get; set; }
        public string ImdbUrl => $"http://www.imdb.com/title/{imdb}/";
    }
}
