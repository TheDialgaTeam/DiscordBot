using System.Threading.Tasks;

namespace TheDialgaTeam.DependencyInjection
{
    public interface IInitializableAsync
    {
        Task InitializeAsync();
    }
}