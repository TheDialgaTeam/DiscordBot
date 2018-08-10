using Nancy;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Nancy
{
    public sealed class IndexController : NancyModule
    {
        public IndexController(SQLiteService sqliteService)
        {
            Get("/", args => Response.AsText("This is running :)"));
        }
    }
}