using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using Nancy.Owin;
using Nancy.TinyIoc;
using TheDialgaTeam.DependencyInjection;
using TheDialgaTeam.DependencyInjection.ProgramLoop;
using TheDialgaTeam.Discord.Bot.Old.Service.Discord;
using TheDialgaTeam.Discord.Bot.Old.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Service.Nancy
{
    public sealed class RestWebService : IDisposableAsync
    {
        private Program Program { get; }

        private IWebHost WebHost { get; set; }

        public RestWebService(Program program)
        {
            Program = program;
        }

        public async Task StartAsync(ushort port = 5000)
        {
            WebHost = new WebHostBuilder()
                      .UseContentRoot(Environment.CurrentDirectory)
                      .UseKestrel()
                      .ConfigureServices(a =>
                      {
                          a.AddSingleton(Program.ServiceProvider.GetRequiredService<SQLiteService>());
                          a.AddSingleton(Program.ServiceProvider.GetRequiredService<DiscordAppService>());
                      })
                      .UseStartup<Startup>()
                      .UseUrls($"http://*:{port}")
                      .Build();

            await WebHost.StartAsync();
        }

        public async Task StopAsync()
        {
            if (WebHost != null)
                await WebHost.StopAsync().ConfigureAwait(false);
        }

        public async Task DisposeAsync()
        {
            await StopAsync().ConfigureAwait(false);
            WebHost?.Dispose();
        }
    }

    internal sealed class Startup
    {
        private SQLiteService SQLiteService { get; }

        private DiscordAppService DiscordAppService { get; }

        public Startup(SQLiteService sqliteService, DiscordAppService discordAppService)
        {
            SQLiteService = sqliteService;
            DiscordAppService = discordAppService;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy(a => a.Bootstrapper = new Bootstrapper(SQLiteService, DiscordAppService)));
        }
    }

    internal sealed class Bootstrapper : DefaultNancyBootstrapper
    {
        private SQLiteService SQLiteService { get; }

        private DiscordAppService DiscordAppService { get; }

        public Bootstrapper(SQLiteService sqliteService, DiscordAppService discordAppService)
        {
            SQLiteService = sqliteService;
            DiscordAppService = discordAppService;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register(SQLiteService);
            container.Register(DiscordAppService);
        }
    }
}