using System.Collections.Generic;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.ResultListeners;

namespace StorEvil.InPlace
{

    public interface IRemoteHandlerFactory
    {
        IRemoteStoryHandler GetHandler(Story story, IEnumerable<Scenario> scenarios, IResultListener listener);
    }

    public class RemoteHandlerFactory : IRemoteHandlerFactory
    {
        private readonly AssemblyGenerator _assemblyGenerator;
        private readonly ConfigSettings _settings;
        private readonly IFilesystem _filesystem;

        public RemoteHandlerFactory(AssemblyGenerator assemblyGenerator, ConfigSettings settings, IFilesystem filesystem)
        {
            _assemblyGenerator = assemblyGenerator;
            _settings = settings;
            _filesystem = filesystem;
        }

        public IRemoteStoryHandler GetHandler(Story story, IEnumerable<Scenario> scenarios, IResultListener listener)
        {
            var assemblyLocation = _assemblyGenerator.GenerateAssembly(story, scenarios, _settings.AssemblyLocations);
            return new RemoteStoryHandler(assemblyLocation, _filesystem, new RemoteListener(listener), _settings.AssemblyLocations);
        }
    }
}