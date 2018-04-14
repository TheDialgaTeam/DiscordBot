﻿using Discord;
using Discord.Commands;
using System;
using System.Text;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;

namespace TheDialgaTeam.DiscordBot.Modules
{
    [Name("Help")]
    public sealed class HelpModule : ModuleHelper
    {
        private IServiceProvider ServiceProvider { get; }

        private CommandService CommandService { get; }

        public HelpModule(IProgram program)
        {
            ServiceProvider = program.ServiceProvider;
            CommandService = program.CommandService;
        }

        [Command("Help")]
        public async Task HelpAsync()
        {
            var helpMessage = new EmbedBuilder()
                              .WithTitle("Available Command:")
                              .WithColor(Color.Orange)
                              .WithDescription($"To find out more about each command, use `{Context.Client.CurrentUser.Mention} help <CommandName>`")
                              .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl());

            foreach (var module in CommandService.Modules)
            {
                var moduleName = $"{module.Name} Module";

                if (moduleName == "Help Module")
                    continue;

                var commandInfo = new StringBuilder();

                foreach (var command in module.Commands)
                {
                    var preconditionResult = await command.CheckPreconditionsAsync(Context, ServiceProvider);

                    if (!preconditionResult.IsSuccess)
                        continue;

                    commandInfo.Append($"`{command.Name}`");

                    if (command.Aliases.Count > 0)
                    {
                        foreach (var commandAliase in command.Aliases)
                        {
                            if (!commandAliase.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                                commandInfo.Append($" `{commandAliase}`");
                        }
                    }

                    commandInfo.AppendLine($": {command.Summary}");
                }

                if (commandInfo.Length > 0)
                    helpMessage = helpMessage.AddField(moduleName, commandInfo.ToString());
            }

            await ReplyDMAsync("", false, helpMessage.Build());
        }

        [Command("Help")]
        public async Task HelpAsync([Remainder] string commandName)
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
                                      .WithDescription($"To find out more about each command, use `{Context.Client.CurrentUser.Mention} help <CommandName>`");

                    var requiredPermission = RequiredPermissions.GuildMember;
                    var requiredContext = ContextType.Guild | ContextType.DM | ContextType.Group;
                    string requiredContextString;

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

                    switch (requiredContext)
                    {
                        case ContextType.Guild | ContextType.DM | ContextType.Group:
                            requiredContextString = $"{ContextType.Guild}, {ContextType.DM}, {ContextType.Group}";
                            break;

                        case ContextType.Guild | ContextType.DM:
                            requiredContextString = $"{ContextType.Guild}, {ContextType.DM}";
                            break;

                        case ContextType.Guild | ContextType.Group:
                            requiredContextString = $"{ContextType.Guild}, {ContextType.Group}";
                            break;

                        case ContextType.DM | ContextType.Group:
                            requiredContextString = $"{ContextType.DM}, {ContextType.Group}";
                            break;

                        case ContextType.Guild:
                            requiredContextString = $"{ContextType.Guild}";
                            break;

                        case ContextType.DM:
                            requiredContextString = $"{ContextType.DM}";
                            break;

                        case ContextType.Group:
                            requiredContextString = $"{ContextType.Group}";
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var commandInfo = new StringBuilder($"Usage: {Context.Client.CurrentUser.Mention} {command.Name}");
                    var argsInfo = new StringBuilder();

                    foreach (var commandParameter in command.Parameters)
                    {
                        commandInfo.Append(!commandParameter.IsOptional ? $" `{commandParameter.Type.Name} {commandParameter.Name}`" : $" `[{commandParameter.Type.Name} {commandParameter.Name} = {commandParameter.DefaultValue ?? "null"}]`");
                        argsInfo.AppendLine($"{commandParameter.Type.Name} {commandParameter.Name}: {commandParameter.Summary}");
                    }

                    commandInfo.AppendLine($"\nDescription: {command.Summary}");
                    commandInfo.AppendLine($"Required Permission: {requiredPermission.ToString()}");
                    commandInfo.AppendLine($"Required Context: {requiredContextString}");

                    if (argsInfo.Length > 0)
                    {
                        commandInfo.AppendLine("\nArguments Info:");
                        commandInfo.Append(argsInfo);
                    }

                    commandInfo.AppendLine("\nNote:");
                    commandInfo.AppendLine("Char/String arguments must be double quoted except for the last string parameter \"This is a string\".");
                    commandInfo.AppendLine("IRole arguments can be role name, role id or @role.");
                    commandInfo.AppendLine("IChannel arguments can be channel name, channel id or #channel.");

                    helpMessage = helpMessage.AddField($"{command.Name} command:", commandInfo.ToString());

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