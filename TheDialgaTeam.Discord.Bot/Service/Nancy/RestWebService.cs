using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using Nancy.Owin;
using Nancy.TinyIoc;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Service.Nancy
{
    public sealed class RestWebService
    {
        private Program Program { get; }

        private IWebHost WebHost { get; set; }

        public RestWebService(Program program)
        {
            Program = program;

            WebHost = new WebHostBuilder()
                      .UseContentRoot(Environment.CurrentDirectory)
                      .UseKestrel()
                      .ConfigureServices(a => { a.AddSingleton(program.ServiceProvider.GetRequiredService<SQLiteService>()); })
                      .UseStartup<Startup>()
                      .UseUrls("http://*:5000")
                      .Build();
        }

        public void RebuildWebHost(ushort port)
        {
            WebHost = new WebHostBuilder()
                      .UseContentRoot(Environment.CurrentDirectory)
                      .UseKestrel()
                      .ConfigureServices(a => { a.AddSingleton(Program.ServiceProvider.GetRequiredService<SQLiteService>()); })
                      .UseStartup<Startup>()
                      .UseUrls($"http://*:{port}")
                      .Build();
        }

        public async Task StartAsync()
        {
            await WebHost.StartAsync();
        }

        public async Task StopAsync()
        {
            await WebHost.StopAsync();
        }
    }

    internal sealed class Startup
    {
        private SQLiteService SQLiteService { get; }

        public Startup(SQLiteService sqliteService)
        {
            SQLiteService = sqliteService;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy(a => a.Bootstrapper = new Bootstrapper(SQLiteService)));
        }
    }

    internal sealed class Bootstrapper : DefaultNancyBootstrapper
    {
        private SQLiteService SQLiteService { get; }

        public Bootstrapper(SQLiteService sqliteService)
        {
            SQLiteService = sqliteService;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register(SQLiteService);
        }
    }
}