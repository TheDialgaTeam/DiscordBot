using TheDialgaTeam.Commands.Builders;
using TheDialgaTeam.Commands.Info;

namespace TheDialgaTeam.Commands
{
    internal interface IModuleBase
    {
        void SetContext(string commandString);

        void BeforeExecute(CommandInfo command);
        
        void AfterExecute(CommandInfo command);

        void OnModuleBuilding(CommandService commandService, ModuleBuilder builder);
    }
}
