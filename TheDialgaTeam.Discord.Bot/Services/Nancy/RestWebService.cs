using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using Nancy.Owin;
using Nancy.TinyIoc;
using TheDialgaTeam.DependencyInjection;
using TheDialgaTeam.Discord.Bot.Services.Discord;
using TheDialgaTeam.Discord.Bot.Services.EntityFramework;

namespace TheDialgaTeam.Discord.Bot.Services.Nancy
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
            WebHost?.Dispose();
            WebHost = new WebHostBuilder()
                      .UseContentRoot(Environment.CurrentDirectory)
                      .UseKestrel()
                      .ConfigureServices(a =>
                      {
                          a.AddSingleton(Program.ServiceProvider.GetService<SqliteDatabaseService>());
                          a.AddSingleton(Program.ServiceProvider.GetService<DiscordAppService>());
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
        private SqliteDatabaseService SqliteDatabaseService { get; }

        private DiscordAppService DiscordAppService { get; }

        public Startup(SqliteDatabaseService sqliteDatabaseService, DiscordAppService discordAppService)
        {
            SqliteDatabaseService = sqliteDatabaseService;
            DiscordAppService = discordAppService;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy(a => a.Bootstrapper = new Bootstrapper(SqliteDatabaseService, DiscordAppService)));
        }
    }

    internal sealed class Bootstrapper : DefaultNancyBootstrapper
    {
        private SqliteDatabaseService SqliteDatabaseService { get; }

        private DiscordAppService DiscordAppService { get; }

        public Bootstrapper(SqliteDatabaseService sqliteDatabaseService, DiscordAppService discordAppService)
        {
            SqliteDatabaseService = sqliteDatabaseService;
            DiscordAppService = discordAppService;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register(SqliteDatabaseService);
            container.Register(DiscordAppService);
        }
    }
}