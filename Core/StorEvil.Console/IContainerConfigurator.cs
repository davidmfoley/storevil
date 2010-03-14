using Funq;
using StorEvil.Configuration;

namespace StorEvil.Console
{
    public interface IContainerConfigurator
    {
        string GetUsage();
        void SetupContainer(Container container, ConfigSettings settings, string[] args);
    }
}