using System;

namespace TheDialgaTeam.Commands.Attributes
{
    /// <summary>
    /// Instructs the command system to treat command paramters of this type
    /// as a collection of named arguments matching to its properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class NamedArgumentTypeAttribute : Attribute { }
}
