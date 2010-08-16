using System.Collections.Generic;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Infrastructure;
using StorEvil.InPlace.CompilingRunner;

namespace StorEvil.InPlace
{

    public interface IRemoteHandlerFactory
    {
        IRemoteStoryHandler GetHandler(Story story, IEnumerable<Scenario> scenarios, IEventBus bus);
    }

    public class RemoteHandlerFactory : IRemoteHandlerFactory
    {
        private readonly AssemblyGenerator _assemblyGenerator;
        private readonly AssemblyRegistry _assemblyRegistry;

        private readonly IFilesystem _filesystem;

        public RemoteHandlerFactory(AssemblyGenerator assemblyGenerator, AssemblyRegistry assemblyRegistry, IFilesystem filesystem)
        {
            _assemblyGenerator = assemblyGenerator;
            _assemblyRegistry = assemblyRegistry;
           
            _filesystem = filesystem;
        }

        public virtual IRemoteStoryHandler GetHandler(Story story, IEnumerable<Scenario> scenarios, IEventBus bus)
        {
            var spec = new AssemblyGenerationSpec { Assemblies = _assemblyRegistry.GetAssemblyLocations() };
            spec.AddStory(story, scenarios);

            var assemblyLocation = _assemblyGenerator.GenerateAssembly(spec);
            return new RemoteStoryHandler(assemblyLocation, _filesystem, bus, _assemblyRegistry.GetAssemblyLocations());
        }
    }
}