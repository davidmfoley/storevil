using Funq;
using StorEvil.Configuration;
using StorEvil.Console;
using StorEvil.Core;
using StorEvil.Events;
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
            container.EasyRegister<IRemoteHandlerFactory, SameDomainHandlerFactory>();
            container.EasyRegister<AssemblyGenerator>();

            container.Register<IStoryFilter>(new TagFilter(customSettings.Tags ?? new string[0]));

            var bus = StorEvilEvents.Bus;
            container.Register<IEventBus>(bus);
            bus.Register(new DebugListener());

        }
    }
}