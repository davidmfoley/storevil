using StorEvil.Configuration;
using StorEvil.InPlace;

namespace StorEvil.Console
{
    public class InPlaceContainerSwitchConfigurator : ContainerSwitchConfigurator<InPlaceSettings>
    {
        public InPlaceContainerSwitchConfigurator() : base(new InPlaceContainerConfigurator())
        {
        }

        protected override void SetupSwitches(SwitchParser<InPlaceSettings> parser)
        {
            parser.AddSwitch("--tags", "-g")
                .SetsField(x => x.Tags);
        }
    }

}