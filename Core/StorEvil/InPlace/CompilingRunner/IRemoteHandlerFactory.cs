using System.Collections.Generic;
using StorEvil.Core;
using StorEvil.Events;

namespace StorEvil.InPlace
{
    public interface IRemoteHandlerFactory
    {
        IRemoteStoryHandler GetHandler(IEnumerable<Story> stories, IEventBus bus);
    }
}