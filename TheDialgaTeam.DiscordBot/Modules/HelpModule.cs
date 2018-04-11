using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Text;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;

namespace TheDialgaTeam.DiscordBot.Modules
{
    [Name("Help")]
    public sealed class HelpModule : ModuleHelper
    {
        private CommandService CommandService { get; }

        public HelpModule(IProgram program)
        {
            CommandService = program.CommandService;
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
                var moduleName = $"{commandServiceModule.Name} Module";
                var commandInfo = new StringBuilder();

                if (moduleName == "Help Module")
                    continue;

                foreach (var preconditionAttribute in commandServiceModule.Preconditions)
                {
                    if (!(preconditionAttribute is RequireActiveModuleAttribute))
                        continue;

                    moduleName += " (Subscribers only)";
                }

                foreach (var command in commandServiceModule.Commands)
                    commandInfo.AppendLine($"`{command.Name}`: {command.Summary}");

                if (commandInfo.Length > 0)
                    helpMessage = helpMessage.AddField(moduleName, commandInfo.ToString());
            }

            if (Context.Message.Channel is SocketDMChannel || Context.Message.Channel is SocketGroupChannel)
                await ReplyAsync("", false, helpMessage.Build());
            else
            {
                var dmChannel = await Context.Message.Author.GetOrCreateDMChannelAsync();
                await dmChannel.SendMessageAsync("", false, helpMessage.Build());
            }
        }

        [Command("Help")]
        public async Task Help([Remainder] string commandName)
        {
            foreach (var commandServiceModule in CommandService.Modules)
            {
                var moduleName = $"{commandServiceModule.Name} Module";

                if (moduleName == "Help Module")
                    continue;

                foreach (var command in commandServiceModule.Commands)
                {
                    if (!CheckCommandEquals(command, commandName))
                        continue;

                    var helpMessage = new EmbedBuilder()
                                      .WithTitle("Command Info:")
                                      .WithColor(Color.Orange)
                                      .WithDescription($"To find out more about each command, use {Context.Client.CurrentUser.Mention} help \"CommandName\"");

                    var requiredPermission = RequiredPermissions.GuildMember;
                    var requiredContext = ContextType.Guild | ContextType.DM | ContextType.Group;
                    var requiredContextString = string.Empty;

                    foreach (var commandAttribute in command.Preconditions)
                    {
                        switch (commandAttribute)
                        {
                            case RequirePermissionAttribute requirePermissionAttribute:
                                requiredPermission = requirePermissionAttribute.RequiredPermission;
                                break;

                            case RequireContextAttribute requireContextAttribute:
                                requiredContext = requireContextAttribute.Contexts;
                                break;
                        }
                    }

                    if (requiredContext == (ContextType.Guild | ContextType.DM | ContextType.Group))
                        requiredContextString = $"{ContextType.Guild}, {ContextType.DM}, {ContextType.Group}";
                    else if (requiredContext == (ContextType.Guild | ContextType.DM))
                        requiredContextString = $"{ContextType.Guild}, {ContextType.DM}";
                    else if (requiredContext == (ContextType.Guild | ContextType.Group))
                        requiredContextString = $"{ContextType.Guild}, {ContextType.Group}";
                    else if (requiredContext == (ContextType.DM | ContextType.Group))
                        requiredContextString = $"{ContextType.DM}, {ContextType.Group}";
                    else if (requiredContext == ContextType.Guild)
                        requiredContextString = $"{ContextType.Guild}";
                    else if (requiredContext == ContextType.DM)
                        requiredContextString = $"{ContextType.DM}";
                    else if (requiredContext == ContextType.Group)
                        requiredContextString = $"{ContextType.Group}";

                    if (command.Parameters.Count == 0)
                        helpMessage = helpMessage.AddField($"{command.Name} command:", $"Usage: {Context.Client.CurrentUser.Mention} {command.Name}\nDescription: {command.Summary}\nRequired Permission: {requiredPermission.ToString()}\nRequired Context: {requiredContext}");
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
                        commandInfo.AppendLine($"Required Permission: {requiredPermission.ToString()}");
                        commandInfo.Append("Required Context: ");
                        commandInfo.AppendLine(requiredContextString);
                        commandInfo.AppendLine("\nArguments Info:");
                        commandInfo.AppendLine(argsInfo.ToString());
                        commandInfo.AppendLine("Note:");
                        commandInfo.AppendLine("Char/String arguments requires to be double quoted except for the last string parameter \"This is a string\".");
                        commandInfo.AppendLine("Integer arguments can be -2147483648 to 2147483647.");
                        commandInfo.AppendLine("Boolean arguments can be true, false.");

                        helpMessage = helpMessage.AddField($"{command.Name} command:", commandInfo.ToString());
                    }

                    await ReplyAsync("", false, helpMessage.Build());
                    return;
                }
            }
        }

        private static bool CheckCommandEquals(CommandInfo command, string commandName)
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