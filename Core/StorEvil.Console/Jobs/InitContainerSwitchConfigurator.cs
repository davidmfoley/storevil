using System;
using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.NUnit;
using StorEvil.Utility;

namespace StorEvil.Console
{
    internal class InitContainerSwitchConfigurator : ContainerSwitchConfigurator<InitSettings>
    {
        public InitContainerSwitchConfigurator()
            : base(new InitContainerConfigurator())
        {
        }

        protected override void SetupSwitches(SwitchParser<InitSettings> parser)
        {
           
        }

    }

    internal class InitContainerConfigurator : ContainerConfigurator<InitSettings>
    {
        protected override void SetupCustomComponents(Container container, ConfigSettings configSettings, InitSettings customSettings)
        {
            container.EasyRegister<IStorEvilJob, InitJob>();
        }
    }

    internal class InitSettings
    {
    }
}