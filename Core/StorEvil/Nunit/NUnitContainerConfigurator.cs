using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.NUnit
{
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