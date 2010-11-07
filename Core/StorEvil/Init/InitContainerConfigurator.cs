using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.Console
{
    public class InitContainerConfigurator : ContainerConfigurator<InitSettings>
    {
        protected override void SetupCustomComponents(Container container, ConfigSettings configSettings, InitSettings customSettings)
        {
            container.EasyRegister<IStorEvilJob, InitJob>();
        }
    }
}