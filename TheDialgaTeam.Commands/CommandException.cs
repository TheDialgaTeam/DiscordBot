using System;
using TheDialgaTeam.Commands.Info;

namespace TheDialgaTeam.Commands
{
    /// <summary>
    ///     The exception that is thrown if another exception occurs during a command execution.
    /// </summary>
    public class CommandException : Exception
    {
        /// <summary> Gets the command that caused the exception. </summary>
        public CommandInfo Command { get; }
        /// <summary> Gets the command context of the exception. </summary>
        public string CommandString { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandException" /> class using a
        ///     <paramref name="command"/> information, a <paramref name="command"/> context, and the exception that
        ///     interrupted the execution.
        /// </summary>
        /// <param name="command">The command information.</param>
        /// <param name="context">The context of the command.</param>
        /// <param name="ex">The exception that interrupted the command execution.</param>
        public CommandException(CommandInfo command, string commandString, Exception ex)
            : base($"Error occurred executing {command.GetLogText(commandString)}.", ex)
        {
            Command = command;
            CommandString = commandString;
        }
    }
}