namespace TheDialgaTeam.DiscordBot.Model.Command
{
    internal sealed class CommandParser
    {
        public enum ParamenterType
        {
            Boolean,

            SByte,

            Byte,

            Short,

            UShort,

            Int,

            UInt,

            Long,

            ULong,

            Float,

            Double,

            Decimal,

            Char,

            String
        }

        public string Command { get; }

        private string CommandArgs { get; }

        public CommandParser(string command, bool caseSensitive = false)
        {
            Command = command.IndexOf(' ') == -1 ? command : command.Remove(command.IndexOf(' '));
            CommandArgs = command.IndexOf(' ') == -1 ? string.Empty : command.Remove(0, command.IndexOf(' ') + 1);

            if (!caseSensitive)
                Command = Command.ToLower();
        }

        public object[] GetCommandParamenterTypeObjects(params ParamenterType[] paramenterTypes)
        {
            if (paramenterTypes.Length == 0)
                return new object[0];
        }
    }
}