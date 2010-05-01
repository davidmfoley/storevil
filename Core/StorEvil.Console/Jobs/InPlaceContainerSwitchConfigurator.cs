using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Utility;

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