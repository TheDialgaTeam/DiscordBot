using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheDialgaTeam.DependencyInjection
{
    public sealed class DependencyInjectionManager
    {
        private IEnumerable<IInitializableAsync> InitializableAsyncServices { get; }

        private IEnumerable<IDisposableAsync> DisposableAsyncServices { get; }

        private IErrorLogger ErrorLogger { get; }

        private bool Initialized { get; set; }

        public DependencyInjectionManager(IServiceProvider serviceProvider)
        {
            InitializableAsyncServices = serviceProvider.GetService(typeof(IEnumerable<IInitializableAsync>)) as IEnumerable<IInitializableAsync>;
            DisposableAsyncServices = serviceProvider.GetService(typeof(IEnumerable<IDisposableAsync>)) as IEnumerable<IDisposableAsync>;
            ErrorLogger = serviceProvider.GetService(typeof(IErrorLogger)) as IErrorLogger;
        }

        public async Task StartProgramLoopAsync()
        {
            if (Initialized)
                return;

            Initialized = true;

            if (InitializableAsyncServices != null)
            {
                foreach (var initializableAsyncService in InitializableAsyncServices)
                {
                    try
                    {
                        await initializableAsyncService.InitializeAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        if (ErrorLogger != null)
                            await ErrorLogger.LogErrorMessageAsync(ex).ConfigureAwait(false);

                        throw;
                    }
                }
            }
        }

        public async Task StopProgramLoopAsync()
        {
            if (!Initialized)
                return;

            Initialized = false;

            if (DisposableAsyncServices != null)
            {
                foreach (var disposableAsync in DisposableAsyncServices.Reverse())
                {
                    try
                    {
                        await disposableAsync.DisposeAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        if (ErrorLogger != null)
                            await ErrorLogger.LogErrorMessageAsync(ex).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}