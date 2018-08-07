using Discord;

namespace TheDialgaTeam.Discord.Bot.Model.Discord.Command
{
    internal sealed class CommandExecuteResult
    {
        public string Message { get; }

        public EmbedBuilder EmbedBuilder { get; }

        public bool IsSuccess { get; }

        private CommandExecuteResult(string message, EmbedBuilder embedBuilder, bool isSuccess)
        {
            Message = message;
            EmbedBuilder = embedBuilder;
            IsSuccess = isSuccess;
        }

        public static CommandExecuteResult FromSuccess(string message)
        {
            return new CommandExecuteResult(message, null, true);
        }

        public static CommandExecuteResult FromSuccess(EmbedBuilder embedBuilder)
        {
            return new CommandExecuteResult(null, embedBuilder, true);
        }

        public static CommandExecuteResult FromError(string message)
        {
            return new CommandExecuteResult(message, null, false);
        }

        public static CommandExecuteResult FromError(EmbedBuilder embedBuilder)
        {
            return new CommandExecuteResult(null, embedBuilder, false);
        }

        public string BuildDiscordTextResponse()
        {
            if (Message == null)
                return null;

            return IsSuccess ? $":white_check_mark: {Message}" : $":x: {Message}";
        }

        public Embed BuildDiscordEmbedResponse()
        {
            if (EmbedBuilder == null)
                return null;

            return IsSuccess ? EmbedBuilder.WithColor(Color.Green).Build() : EmbedBuilder.WithColor(Color.Red).Build();
        }
    }
}