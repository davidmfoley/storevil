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

    
}