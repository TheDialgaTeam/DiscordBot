using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace TheDialgaTeam.DependencyInjection.ProgramLoop
{
    [SuppressMessage("ReSharper", "RedundantNameQualifier")]
    public sealed class ProgramLoopManager
    {
        private IEnumerable<IInitializable> InitializableServices { get; }

        private IEnumerable<IInitializableAsync> InitializableAsyncServices { get; }

        private IEnumerable<ITickable> TickableServices { get; }

        private IEnumerable<ITickableAsync> TickableAsyncServices { get; }

        private IEnumerable<IDisposable> DisposableService { get; }

        private IEnumerable<IDisposableAsync> DisposableAsyncServices { get; }

        private IErrorLogger ErrorLogger { get; }

        private bool Initialized { get; set; }

        private System.Timers.Timer UpdateTimer { get; set; }

        private bool ContinueNextFrame { get; set; } = true;

        public ProgramLoopManager(IServiceProvider serviceProvider)
        {
            InitializableServices = serviceProvider.GetService(typeof(IEnumerable<IInitializable>)) as IEnumerable<IInitializable>;
            InitializableAsyncServices = serviceProvider.GetService(typeof(IEnumerable<IInitializableAsync>)) as IEnumerable<IInitializableAsync>;
            TickableServices = serviceProvider.GetService(typeof(IEnumerable<ITickable>)) as IEnumerable<ITickable>;
            TickableAsyncServices = serviceProvider.GetService(typeof(IEnumerable<ITickableAsync>)) as IEnumerable<ITickableAsync>;
            DisposableService = serviceProvider.GetService(typeof(IEnumerable<IDisposable>)) as IEnumerable<IDisposable>;
            DisposableAsyncServices = serviceProvider.GetService(typeof(IEnumerable<IDisposableAsync>)) as IEnumerable<IDisposableAsync>;
            ErrorLogger = serviceProvider.GetService(typeof(IErrorLogger)) as IErrorLogger;
        }

        public async Task StartProgramLoopAsync(int targetFPS = 60)
        {
            if (Initialized)
                return;

            Initialized = true;

            if (InitializableServices != null)
            {
                foreach (var initializableService in InitializableServices)
                {
                    try
                    {
                        initializableService.Initialize();
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger?.LogErrorMessage(ex);
                    }
                }
            }

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
                        ErrorLogger?.LogErrorMessage(ex);
                    }
                }
            }

            UpdateTimer = new System.Timers.Timer(1000d / targetFPS) { AutoReset = true, Enabled = false };
            UpdateTimer.Elapsed += UpdateTimerOnElapsed;
            UpdateTimer.Start();
        }

        public async Task StopProgramLoopAsync()
        {
            if (!Initialized)
                return;

            Initialized = false;

            UpdateTimer.Elapsed -= UpdateTimerOnElapsed;
            UpdateTimer.Stop();

            if (DisposableService != null)
            {
                foreach (var disposable in DisposableService.Reverse())
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger?.LogErrorMessage(ex);
                    }
                }
            }

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
                        ErrorLogger?.LogErrorMessage(ex);
                    }
                }
            }
        }

        private async void UpdateTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (!ContinueNextFrame)
                return;

            ContinueNextFrame = false;

            if (TickableServices != null)
            {
                foreach (var tickableService in TickableServices)
                {
                    try
                    {
                        tickableService.Tick();
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger?.LogErrorMessage(ex);
                    }
                }
            }

            if (TickableAsyncServices != null)
            {
                foreach (var tickableAsyncService in TickableAsyncServices)
                {
                    try
                    {
                        await tickableAsyncService.TickAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger?.LogErrorMessage(ex);
                    }
                }
            }

            ContinueNextFrame = true;
        }
    }
}