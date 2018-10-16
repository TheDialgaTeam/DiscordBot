using System;

namespace TheDialgaTeam.DependencyInjection.ProgramLoop
{
    public interface IErrorLogger
    {
        void LogErrorMessage(Exception ex);
    }
}