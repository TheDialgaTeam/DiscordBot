using Nancy;

namespace TheDialgaTeam.DiscordBot.Nancy
{
    public sealed class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", args => View["Web/index.html"]);
        }
    }
}