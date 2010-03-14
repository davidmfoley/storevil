using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.StubGeneration;
using StorEvil.Utility;

namespace StorEvil.Console
{
    public class StubGeneratorContainerConfigurator : ContainerConfigurator<StubGenerationSettings>
    {
        protected override void SetupCustomComponents(Container container)
        {
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
            container.Register<IStoryHandler>(
                c => new StubGenerator(c.Resolve<ScenarioInterpreter>(), new ImplementationHelper(), GetWriter())
                );
        }

        private ITextWriter GetWriter()
        {
            if (CustomSettings.WriteToClipboard)
                return new ClipboardWriter();

            if (!string.IsNullOrEmpty(CustomSettings.Destination))
                return new FileWriter(CustomSettings.Destination, true);

            return new StdOutWriter();
        }

        protected override void SetupSwitches(SwitchParser<StubGenerationSettings> parser)
        {
            parser.AddSwitch("--clipboard", "-b").SetsField(x => x.WriteToClipboard);
            parser.AddSwitch("--destination", "-d").SetsField(x => x.Destination);
        }
    }
}