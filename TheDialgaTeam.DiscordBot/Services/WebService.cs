using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using Nancy.Owin;
using Nancy.TinyIoc;
using System;
using System.Threading.Tasks;

namespace TheDialgaTeam.DiscordBot.Services
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
                      .ConfigureServices(a => { a.AddSingleton(program); })
                      .UseStartup<Startup>()
                      .UseUrls("http://*:80")
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
        private IServiceProvider ServiceProvider { get; }

        public Startup(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy(a => a.Bootstrapper = new Bootstrapper(ServiceProvider)));
        }
    }

    internal sealed class Bootstrapper : DefaultNancyBootstrapper
    {
        private IServiceProvider ServiceProvider { get; }

        public Bootstrapper(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register(ServiceProvider);
        }
    }
}