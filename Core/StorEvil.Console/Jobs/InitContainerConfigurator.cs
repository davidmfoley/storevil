using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.Console
{
    internal class InitContainerConfigurator : IContainerConfigurator
    {
        public string GetUsage()
        {
            return "initializes a storevil.config file";
        }

        public void SetupContainer(Container container, ConfigSettings settings, string[] args)
        {
            container.EasyRegister<IStorEvilJob, InitJob>();
            
        }
    }
}