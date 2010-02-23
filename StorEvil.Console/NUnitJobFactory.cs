using Funq;
using StorEvil.Core;
using StorEvil.Core.Configuration;
using StorEvil.Nunit;

namespace StorEvil.Console
{
    public class NUnitJobFactory : JobFactory<TestFixtureGenerationSettings>
    {
        protected override void SetupCustomComponents(Container container)
        {
            container.EasyRegister<IFixtureGenerator, NUnitFixtureGenerator>();
            container.EasyRegister<NUnitTestMethodGenerator>();
            container.EasyRegister<CSharpMethodInvocationGenerator>();
            container.EasyRegister<IStoryHandler, FixtureGenerationStoryHandler>();
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
            container.EasyRegister<ITestFixtureWriter, SingleFileTestFixtureWriter>();
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