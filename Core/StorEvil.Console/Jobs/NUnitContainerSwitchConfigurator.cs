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

    public class NUnitContainerConfigurator : ContainerConfigurator<TestFixtureGenerationSettings>
    {
        protected override void SetupCustomComponents(Container container, ConfigSettings configSettings, TestFixtureGenerationSettings customSettings)
        {
            container.EasyRegister<IFixtureGenerator, NUnitFixtureGenerator>();
            container.EasyRegister<ITestMethodGenerator, NUnitTestMethodGenerator>();
            container.EasyRegister<CSharpMethodInvocationGenerator>();
            container.EasyRegister<IStoryHandler, FixtureGenerationStoryHandler>();
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
            container.EasyRegister<ITestFixtureWriter, SingleFileTestFixtureWriter>();
            container.Register<IStoryFilter>(new IncludeAllFilter());
        }
    }
}