using System.Threading.Tasks;

namespace TheDialgaTeam.DependencyInjection.ProgramLoop
{
    public interface IDisposable : System.IDisposable
    {
    }

    public interface IDisposableAsync
    {
        Task DisposeAsync();
    }
}