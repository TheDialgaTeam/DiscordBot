using System.Threading.Tasks;

namespace TheDialgaTeam.DependencyInjection.ProgramLoop
{
    public interface ITickable
    {
        void Tick();
    }

    public interface ITickableAsync
    {
        Task TickAsync();
    }
}