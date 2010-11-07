using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.NUnit;
using StorEvil.Utility;

namespace StorEvil.Console
{
    public class NUnitContainerSwitchConfigurator : ContainerSwitchConfigurator<TestFixtureGenerationSettings>
    {
        public NUnitContainerSwitchConfigurator() : base(new NUnitContainerConfigurator())
        {
        }

        protected override void SetupSwitches(SwitchParser<TestFixtureGenerationSettings> parser)
        {
            parser
                .AddSwitch("--destination", "-d")
                .SetsField(x => x.TargetFilePath)
                .WithDescription("path to the file that will hold the generated test fixture code");
        }
    }

  
}