using Discord;
using Discord.Commands;
using System;
using System.Text;
using System.Threading.Tasks;

namespace TheDialgaTeam.DiscordBot.Modules
{
    public sealed class HelpModule : ModuleBase<SocketCommandContext>
    {
        private CommandService CommandService { get; }

        public HelpModule(CommandService commandService)
        {
            CommandService = commandService;
        }

        [Command("Help")]
        public async Task Help()
        {
            var helpMessage = new EmbedBuilder()
                              .WithTitle("Available Command:")
                              .WithColor(Color.Orange)
                              .WithDescription($"To find out more about each command, use {Context.Client.CurrentUser.Mention} help \"CommandName\"");

            foreach (var commandServiceModule in CommandService.Modules)
            {
                var moduleName = commandServiceModule.Name;
                var commandInfo = new StringBuilder();

                if (moduleName == nameof(HelpModule))
                    continue;

                foreach (var command in commandServiceModule.Commands)
                {
                    var permission = await command.CheckPreconditionsAsync(Context);

                    if (permission.IsSuccess)
                        commandInfo.AppendLine($"`{command.Name}`: {command.Summary}");
                }

                if (commandInfo.Length > 0)
                    helpMessage = helpMessage.AddField(moduleName, commandInfo.ToString());
            }

            var dmChannel = await Context.Message.Author.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync("", false, helpMessage);
        }

        [Command("Help")]
        public async Task Help([Remainder] string commandName)
        {
            foreach (var commandServiceModule in CommandService.Modules)
            {
                foreach (var command in commandServiceModule.Commands)
                {
                    if (CheckCommandEquals(command, commandName))
                    {
                        var helpMessage = new EmbedBuilder()
                                          .WithTitle("Command Info:")
                                          .WithColor(Color.Orange)
                                          .WithDescription($"To find out more about each command, use {Context.Client.CurrentUser.Mention} help \"CommandName\"");

                        if (command.Parameters.Count == 0)
                        {
                            helpMessage = helpMessage.AddField($"{command.Name} command:", $"Usage: {Context.Client.CurrentUser.Mention} {command.Name}\nDescription: {command.Summary}");
                        }
                        else
                        {
                            var commandInfo = new StringBuilder($"Usage: {Context.Client.CurrentUser.Mention} {command.Name}");
                            var argsInfo = new StringBuilder();

                            foreach (var commandParameter in command.Parameters)
                            {
                                commandInfo.Append(!commandParameter.IsOptional ? $" `{commandParameter.Type.Name} {commandParameter.Name}`" : $" `[{commandParameter.Type.Name} {commandParameter.Name} = {commandParameter.DefaultValue}]`");
                                argsInfo.AppendLine($"{commandParameter.Type.Name} {commandParameter.Name}: {commandParameter.Summary}");
                            }

                            commandInfo.AppendLine($"\nDescription: {command.Summary}");
                            commandInfo.AppendLine("\nArguments Info:");
                            commandInfo.AppendLine(argsInfo.ToString());
                            commandInfo.AppendLine("Note:");
                            commandInfo.AppendLine("Char/String arguments requires to be double quoted except for the last string parameter \"This is a string\".");
                            commandInfo.AppendLine("Integer arguments can be -2147483648 to 2147483647.");
                            commandInfo.AppendLine("Boolean arguments can be true, false.");

                            helpMessage = helpMessage.AddField($"{command.Name} command:", commandInfo.ToString());
                        }

                        await ReplyAsync("", false, helpMessage);
                        return;
                    }
                }
            }
        }

        private bool CheckCommandEquals(CommandInfo command, string commandName)
        {
            if (command.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
                return true;

            foreach (var commandAliase in command.Aliases)
            {
                if (commandAliase.Equals(commandName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}