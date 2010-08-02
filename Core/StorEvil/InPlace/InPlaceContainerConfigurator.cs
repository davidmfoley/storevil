using Funq;
using StorEvil.Configuration;
using StorEvil.Console;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    public class InPlaceContainerConfigurator : ContainerConfigurator<InPlaceSettings>
    {
        protected override void SetupCustomComponents(Container container, ConfigSettings configSettings, InPlaceSettings customSettings)
        {
            container.EasyRegister<IStoryHandler, InPlaceStoryRunner>();
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
            container.Register<IStoryFilter>(new TagFilter(customSettings.Tags ?? new string[0]));
        }
    }
}