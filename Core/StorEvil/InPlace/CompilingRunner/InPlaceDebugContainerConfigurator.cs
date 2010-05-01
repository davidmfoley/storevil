using Funq;
using StorEvil.Configuration;
using StorEvil.Console;
using StorEvil.Core;
using StorEvil.ResultListeners;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
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