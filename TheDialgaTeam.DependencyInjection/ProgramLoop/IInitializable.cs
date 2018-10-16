using System.Threading.Tasks;

namespace TheDialgaTeam.DependencyInjection.ProgramLoop
{
    public interface IInitializable
    {
        void Initialize();
    }

    public interface IInitializableAsync
    {
        Task InitializeAsync();
    }
}