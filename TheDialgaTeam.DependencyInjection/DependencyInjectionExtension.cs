using System;
using Microsoft.Extensions.DependencyInjection;

namespace TheDialgaTeam.DependencyInjection
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection BindInterfacesAndSelfAsSingleton<TService>(this IServiceCollection serviceCollection) where TService : class
        {
            if (!typeof(TService).IsClass)
                throw new ArgumentException($"{nameof(TService)} expect a class.");

            serviceCollection.AddSingleton<TService>();

            foreach (var type in typeof(TService).GetInterfaces())
                serviceCollection.AddSingleton(type, a => a.GetService<TService>());

            return serviceCollection;
        }
    }
}