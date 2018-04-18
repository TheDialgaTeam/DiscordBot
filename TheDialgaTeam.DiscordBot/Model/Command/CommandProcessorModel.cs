using System;
using System.Text.RegularExpressions;

namespace TheDialgaTeam.DiscordBot.Model.Command
{
    internal interface ICommandProcessorModel
    {
        string Command { get; }

        object[] GetCommandParamenterTypeObjects(params CommandProcessorModel.ParamenterType[] paramenterTypes);
    }

    internal sealed class CommandProcessorModel : ICommandProcessorModel
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

        public CommandProcessorModel(string fullCommand)
        {
            var command = fullCommand.Trim();

            Command = command.IndexOf(' ') == -1 ? command : command.Remove(command.IndexOf(' '));
            CommandArgs = command.IndexOf(' ') == -1 ? string.Empty : command.Remove(0, command.IndexOf(' ') + 1);
        }

        public object[] GetCommandParamenterTypeObjects(params ParamenterType[] paramenterTypes)
        {
            if (paramenterTypes.Length == 0)
                return null;

            var regexExp = "^";

            foreach (var paramenterType in paramenterTypes)
            {
                switch (paramenterType)
                {
                    case ParamenterType.Boolean:
                        regexExp += "(true|false|0|1)";
                        break;

                    case ParamenterType.SByte:
                        regexExp += @"(0x[0-9A-F]{1,}|0b_?[0-1]{1,}(?:_?[0-1]{1,})*|-*\d+)";
                        break;

                    case ParamenterType.Byte:
                        regexExp += @"(0x[0-9A-F]{1,}|0b_?[0-1]{1,}(?:_?[0-1]{1,})*|\d+)";
                        break;

                    case ParamenterType.Short:
                        regexExp += @"(0x[0-9A-F]{1,}|0b_?[0-1]{1,}(?:_?[0-1]{1,})*|-*\d+)";
                        break;

                    case ParamenterType.UShort:
                        regexExp += @"(0x[0-9A-F]{1,}|0b_?[0-1]{1,}(?:_?[0-1]{1,})*|\d+)";
                        break;

                    case ParamenterType.Int:
                        regexExp += @"(0x[0-9A-F]{1,}|0b_?[0-1]{1,}(?:_?[0-1]{1,})*|-*\d+)";
                        break;

                    case ParamenterType.UInt:
                        regexExp += @"(0x[0-9A-F]{1,}|0b_?[0-1]{1,}(?:_?[0-1]{1,})*|\d+)";
                        break;

                    case ParamenterType.Long:
                        regexExp += @"(0x[0-9A-F]{1,}|0b_?[0-1]{1,}(?:_?[0-1]{1,})*|-*\d+)";
                        break;

                    case ParamenterType.ULong:
                        regexExp += @"(0x[0-9A-F]{1,}|0b_?[0-1]{1,}(?:_?[0-1]{1,})*|\d+)";
                        break;

                    case ParamenterType.Float:
                        regexExp += @"(-?\d+\.\d+f|-?\d+)";
                        break;

                    case ParamenterType.Double:
                        regexExp += @"(-?\d+\.\d+|-?\d+)";
                        break;

                    case ParamenterType.Decimal:
                        regexExp += @"(-?\d+\.\d+m|-?\d+)";
                        break;

                    case ParamenterType.Char:
                        regexExp += @"([\s\S])";
                        break;

                    case ParamenterType.String:
                        regexExp += @"""([\s\S]+?)""";
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                regexExp += @"\s+";
            }

            regexExp = regexExp.Remove(regexExp.Length - 3);
            regexExp += "$";

            if (!Regex.IsMatch(CommandArgs, regexExp, RegexOptions.Multiline))
                return null;

            var match = Regex.Match(CommandArgs, regexExp, RegexOptions.Multiline);
            var commandObjects = new object[paramenterTypes.Length];

            for (var i = 0; i < paramenterTypes.Length; i++)
                switch (paramenterTypes[i])
                {
                    case ParamenterType.Boolean:
                        if (match.Groups[i + 1].Value.Equals("true", StringComparison.OrdinalIgnoreCase) || match.Groups[i + 1].Value.Equals("1"))
                            commandObjects[i] = true;
                        else if (match.Groups[i + 1].Value.Equals("false", StringComparison.OrdinalIgnoreCase) || match.Groups[i + 1].Value.Equals("0"))
                            commandObjects[i] = false;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.SByte:
                        if (sbyte.TryParse(match.Groups[i + 1].Value, out var sbyteResult))
                            commandObjects[i] = sbyteResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.Byte:
                        if (byte.TryParse(match.Groups[i + 1].Value, out var byteResult))
                            commandObjects[i] = byteResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.Short:
                        if (short.TryParse(match.Groups[i + 1].Value, out var shortResult))
                            commandObjects[i] = shortResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.UShort:
                        if (ushort.TryParse(match.Groups[i + 1].Value, out var ushortResult))
                            commandObjects[i] = ushortResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.Int:
                        if (int.TryParse(match.Groups[i + 1].Value, out var intResult))
                            commandObjects[i] = intResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.UInt:
                        if (uint.TryParse(match.Groups[i + 1].Value, out var uintResult))
                            commandObjects[i] = uintResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.Long:
                        if (long.TryParse(match.Groups[i + 1].Value, out var longResult))
                            commandObjects[i] = longResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.ULong:
                        if (ulong.TryParse(match.Groups[i + 1].Value, out var ulongResult))
                            commandObjects[i] = ulongResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.Float:
                        if (float.TryParse(match.Groups[i + 1].Value, out var floatResult))
                            commandObjects[i] = floatResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.Double:
                        if (double.TryParse(match.Groups[i + 1].Value, out var doubleResult))
                            commandObjects[i] = doubleResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.Decimal:
                        if (decimal.TryParse(match.Groups[i + 1].Value, out var decimalResult))
                            commandObjects[i] = decimalResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.Char:
                        if (char.TryParse(match.Groups[i + 1].Value, out var charResult))
                            commandObjects[i] = charResult;
                        else
                            commandObjects[i] = null;
                        break;

                    case ParamenterType.String:
                        commandObjects[i] = match.Groups[i + 1].Value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

            return commandObjects;
        }
    }
}