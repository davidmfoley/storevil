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
                .SetsField(x => x.Tags);
        }
    }

    public class InPlaceDebugContainerConfigurator : ContainerConfigurator<InPlaceSettings>
    {
        protected override void SetupCustomComponents(Container container, ConfigSettings settings, InPlaceSettings customSettings)
        {
            container.EasyRegister<IStoryHandler, InPlaceCompilingStoryRunner>();
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
            container.EasyRegister<IRemoteHandlerFactory, RemoteHandlerFactory>();
            container.EasyRegister<AssemblyGenerator>();

            container.Register<IStoryFilter>(new TagFilter(customSettings.Tags ?? new string[0]));

            var listener = container.Resolve<IResultListener>() as CompositeListener;
            listener.Listeners.Add(new DebugListener());
        }
    }
}