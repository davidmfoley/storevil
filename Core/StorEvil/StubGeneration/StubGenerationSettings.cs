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
    public class StubGenerationSettings
    {
        public bool WriteToClipboard { get; set; }

        public string Destination { get; set; }
    }

    public class StubGeneratorContainerConfigurator : ContainerConfigurator<StubGenerationSettings>
    {
        protected override void SetupCustomComponents(Container container, ConfigSettings configSettings, StubGenerationSettings customSettings)
        {
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
            container.Register<IStoryHandler>(
                c => new StubGenerator(c.Resolve<ScenarioInterpreter>(), new ImplementationHelper(), GetWriter(customSettings), c.Resolve<ISessionContext>())
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