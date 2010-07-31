using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.ResultListeners;
using StorEvil.Utility;

namespace StorEvil.Console
{
    public class InPlaceDebugContainerSwitchConfigurator : ContainerSwitchConfigurator<InPlaceSettings>
    {
        public InPlaceDebugContainerSwitchConfigurator() : base(new InPlaceDebugContainerConfigurator())
        {
        }


        protected override void SetupSwitches(SwitchParser<InPlaceSettings> parser)
        {
            parser.AddSwitch("--tags", "-g")
                .WithDescription("Selects @tags to include in the session")
                .SetsField(x => x.Tags);
        }
    }

   
}