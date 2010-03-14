using System;
using System.Collections.Generic;
using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Nunit;
using StorEvil.Utility;

namespace StorEvil.Console
{
    public class NUnitContainerConfigurator : ContainerConfigurator<TestFixtureGenerationSettings>
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