using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Utility;

namespace StorEvil.Console
{
    public class InPlaceDebugContainerConfigurator : ContainerConfigurator<InPlaceSettings>
    {
        protected override void SetupCustomComponents(Container container)
        {
            container.EasyRegister<IStoryHandler, InPlaceCompilingStoryRunner>();
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
            container.EasyRegister<IRemoteHandlerFactory, RemoteHandlerFactory>();
            container.EasyRegister<AssemblyGenerator>(); 

            container.Register<IStoryFilter>(new TagFilter(CustomSettings.Tags ?? new string[0]));
        }

        protected override void SetupSwitches(SwitchParser<InPlaceSettings> parser)
        {
            parser.AddSwitch("--tags", "-g")
                .SetsField(x => x.Tags);
        }
    }
}