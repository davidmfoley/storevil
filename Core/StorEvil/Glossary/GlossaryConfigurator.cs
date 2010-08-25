using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.Interpreter;
using StorEvil.Utility;

namespace StorEvil.Glossary
{
    public class GlossaryConfigurator : ContainerConfigurator<GlossaryConfigurator.GlossarySettings>
    {
        protected override void SetupCustomComponents(Container container, ConfigSettings configSettings, GlossarySettings customSettings)
        {
            container.EasyRegister<IStorEvilJob, StorEvilGlossaryJob>();
            container.EasyRegister<IStepDescriber, StepDescriber>();
            container.EasyRegister<IStepProvider, StepProvider>();
            container.EasyRegister<ContextTypeFactory>();

            var templateSpecifiied = !(string.IsNullOrEmpty(customSettings.GlossaryTemplate) );

            DebugTrace.Trace(this, "GlossaryTemplate:" + customSettings.GlossaryTemplate);
            DebugTrace.Trace(this, "GlossaryDestination:" + customSettings.GlossaryDestination);
            if (!templateSpecifiied)
            {
                container.Register<IGlossaryFormatter>(new NoOpGlossaryFormatter());
            }
            else
            {

                var glossaryDestination = string.IsNullOrEmpty(customSettings.GlossaryDestination) ? "StorEvilGlossary.html" : customSettings.GlossaryDestination;
                DebugTrace.Trace(this, "GlossaryDestination:" + glossaryDestination);
                container.Register<IGlossaryFormatter>(new SparkGlossaryFormatter(new FileWriter(glossaryDestination, true), customSettings.GlossaryTemplate));
            }
        }

        public class GlossarySettings
        {
            public string GlossaryDestination { get; set; }
            public string GlossaryTemplate { get; set; }
        }
    }
}