using Funq;
using StorEvil.Core.Configuration;
using StorEvil.InPlace;

namespace StorEvil.Console
{
    public class InPlaceJobFactory : JobFactory<InPlaceSettings>
    {
        protected override void SetupCustomComponents(Container container)
        {
            container.EasyRegister<IStoryHandler, InPlaceRunner>();
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
        }

        protected override void SetupSwitches(SwitchParser<InPlaceSettings> parser)
        {
           
        }
    }
}