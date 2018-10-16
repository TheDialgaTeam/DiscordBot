using System;
using TheDialgaTeam.DependencyInjection.ProgramLoop;

namespace TheDialgaTeam.Discord.Bot.Services.Console
{
    public sealed class FramesPerSecondService : IInitializable, ITickable
    {
        private DateTime LastCheck { get; set; }

        private int FrameCounted { get; set; }

        public void Initialize()
        {
            LastCheck = DateTime.Now;
        }

        public void Tick()
        {
            FrameCounted++;

            var timeDiff = DateTime.Now - LastCheck;

            if (timeDiff >= TimeSpan.FromSeconds(1))
            {
                System.Console.Title = $"The Dialga Team Discord Bot (.Net Core) | FPS: {FrameCounted / (timeDiff.TotalMilliseconds / timeDiff.TotalMilliseconds):F}/60";
                FrameCounted = 0;
                LastCheck = DateTime.Now;
            }
        }
    }
}