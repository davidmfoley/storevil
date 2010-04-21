using System;
using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.NUnit;
using StorEvil.Utility;

namespace StorEvil.Console
{
    internal class InitContainerConfigurator : ContainerConfigurator<InitSettings>
    {
        protected override void SetupCustomComponents(Container container)
        {
            container.EasyRegister<IStorEvilJob, InitJob>();
        }        

        protected override void SetupSwitches(SwitchParser<InitSettings> parser)
        {
           
        }

    }

    internal class InitSettings
    {
    }
}