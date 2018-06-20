namespace TheDialgaTeam.Discord.Bot.Model.Discord.Command
{
    public interface ICommandExecuteResult
    {
        string Message { get; }

        bool IsSuccess { get; }

        string BuildDiscordResponse();
    }

    internal sealed class CommandExecuteResult : ICommandExecuteResult
    {
        public string Message { get; }

        public bool IsSuccess { get; }

        private CommandExecuteResult(string message, bool isSuccess)
        {
            Message = message;
            IsSuccess = isSuccess;
        }

        public static CommandExecuteResult FromSuccess(string message)
        {
            return new CommandExecuteResult(message, true);
        }

        public static CommandExecuteResult FromError(string message)
        {
            return new CommandExecuteResult(message, false);
        }

        public string BuildDiscordResponse()
        {
            return IsSuccess ? $":white_check_mark: {Message}" : $":x: {Message}";
        }
    }
}