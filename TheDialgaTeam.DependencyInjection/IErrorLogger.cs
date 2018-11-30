using System;
using System.Threading.Tasks;

namespace TheDialgaTeam.DependencyInjection
{
    public interface IErrorLogger
    {
        Task LogErrorMessageAsync(Exception ex);
    }
}