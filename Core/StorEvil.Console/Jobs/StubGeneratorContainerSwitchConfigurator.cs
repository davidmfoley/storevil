using System;
using Funq;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.StubGeneration;
using StorEvil.Utility;

namespace StorEvil.Console
{
    public class StubGeneratorContainerSwitchConfigurator : ContainerSwitchConfigurator<StubGenerationSettings>
    {
        public StubGeneratorContainerSwitchConfigurator()
            : base(new StubGeneratorContainerConfigurator())
        {
        }


        protected override void SetupSwitches(SwitchParser<StubGenerationSettings> parser)
        {
            parser.AddSwitch("--clipboard", "-b").SetsField(x => x.WriteToClipboard).WithDescription("Writes the generated stubs to the clipboard");
            parser.AddSwitch("--destination", "-d").SetsField(x => x.Destination).WithDescription("Writes the generated stubs to a file");
        }
    }

    public class StubGeneratorContainerConfigurator : ContainerConfigurator<StubGenerationSettings>
    {
        protected override void SetupCustomComponents(Container container, ConfigSettings configSettings, StubGenerationSettings customSettings)
        {
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
            container.Register<IStoryHandler>(
                c => new StubGenerator(c.Resolve<ScenarioInterpreter>(), new ImplementationHelper(), GetWriter(customSettings), c.Resolve<IStoryContextFactory>())
                );

            container.Register<IStoryFilter>(new IncludeAllFilter());
        }

        private ITextWriter GetWriter(StubGenerationSettings customSettings)
        {
            if (customSettings.WriteToClipboard)
                return new ClipboardWriter();

            if (!string.IsNullOrEmpty(customSettings.Destination))
                return new FileWriter(customSettings.Destination, true);

            return new StdOutWriter();
        }
    }
}