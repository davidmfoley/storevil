using StorEvil.Configuration;
using StorEvil.Core;

namespace StorEvil.Console
{
    internal class GlossarySwitchConfigurator : ContainerSwitchConfigurator<GlossaryConfigurator.GlossarySettings>
    {
        public GlossarySwitchConfigurator()
            : base(new GlossaryConfigurator())
        {
        }

        protected override void SetupSwitches(SwitchParser<GlossaryConfigurator.GlossarySettings> parser)
        {
            parser
                .AddSwitch("--glossary-template", "-g")
                .SetsField(s => s.GlossaryTemplate)
                .WithDescription("Sets the spark template for generating the template");

        }
    }
}