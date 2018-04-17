using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using Nancy.Owin;
using Nancy.TinyIoc;
using System;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Services.SQLite;

namespace TheDialgaTeam.DiscordBot.Services.Nancy
{
    public interface IWebService
    {
        Task StartAsync();

        Task StopAsync();
    }

    internal sealed class WebService : IWebService
    {
        private IWebHost WebHost { get; }

        public WebService(IProgram program)
        {
            WebHost = new WebHostBuilder()
                      .UseContentRoot(Environment.CurrentDirectory)
                      .UseKestrel()
                      .ConfigureServices(a => { a.AddSingleton(program.ServiceProvider.GetRequiredService<ISQLiteService>()); })
                      .UseStartup<Startup>()
                      .UseUrls("http://*:5000")
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
        private ISQLiteService SQLiteService { get; }

        public Startup(ISQLiteService sqliteService)
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
        private ISQLiteService SQLiteService { get; }

        public Bootstrapper(ISQLiteService sqliteService)
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