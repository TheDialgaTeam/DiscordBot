using System.Threading.Tasks;

namespace TheDialgaTeam.DependencyInjection
{
    public interface IDisposableAsync
    {
        Task DisposeAsync();
    }
}