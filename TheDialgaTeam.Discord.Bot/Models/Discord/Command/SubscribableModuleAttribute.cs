using System;

namespace TheDialgaTeam.Discord.Bot.Models.Discord.Command
{
    public sealed class SubscribableModuleAttribute : Attribute
    {
        public string ModuleName { get; set; }

        public SubscribableModuleAttribute(string name)
        {
            ModuleName = name;
        }
    }
}