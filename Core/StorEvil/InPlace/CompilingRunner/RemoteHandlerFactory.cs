using System;
using System.Collections.Generic;
using System.Reflection;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Infrastructure;
using StorEvil.InPlace.CompilingRunner;

namespace StorEvil.InPlace
{

    public interface IRemoteHandlerFactory
    {
        IRemoteStoryHandler GetHandler(IEnumerable<Story> stories, IEventBus bus);
    }

    public class SameDomainHandlerFactory : IRemoteHandlerFactory
    {
        private readonly AssemblyGenerator _assemblyGenerator;
        private readonly AssemblyRegistry _assemblyRegistry;


        public SameDomainHandlerFactory(AssemblyGenerator assemblyGenerator, AssemblyRegistry assemblyRegistry, IFilesystem filesystem)
        {
            _assemblyGenerator = assemblyGenerator;
            _assemblyRegistry = assemblyRegistry;

        }

        public virtual IRemoteStoryHandler GetHandler(IEnumerable<Story> stories, IEventBus bus)
        {
            var spec = new AssemblyGenerationSpec { Assemblies = _assemblyRegistry.GetAssemblyLocations() };
            foreach (var story in stories)
            {
                spec.AddStory(story, story.Scenarios);
            }

            var assemblyLocation = _assemblyGenerator.GenerateAssembly(spec);
            var assembly = Assembly.LoadFrom(assemblyLocation);
            return new LocalHandler(Activator.CreateInstance(assembly.GetType("StorEvilTestAssembly.StorEvilDriver"), new object[] {bus}) as IStoryHandler);
           // return new RemoteStoryHandler(assemblyLocation, _filesystem, bus, _assemblyRegistry.GetAssemblyLocations());
        }

        public class LocalHandler : IRemoteStoryHandler
        {
            private readonly IStoryHandler _storyHandler;

            public LocalHandler(IStoryHandler storyHandler)
            {
                _storyHandler = storyHandler;
            }

            public JobResult HandleStories(IEnumerable<Story> stories)
            {
                return _storyHandler.HandleStories(stories);
            }

            public void Dispose()
            {
                
            }
        }
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

        public virtual IRemoteStoryHandler GetHandler(IEnumerable<Story> stories, IEventBus bus)
        {
            var spec = new AssemblyGenerationSpec { Assemblies = _assemblyRegistry.GetAssemblyLocations() };
            foreach (var story in stories)
            {
                spec.AddStory(story, story.Scenarios);    
            }            

            var assemblyLocation = _assemblyGenerator.GenerateAssembly(spec);
            return new RemoteStoryHandler(assemblyLocation, _filesystem, bus, _assemblyRegistry.GetAssemblyLocations());
        }
    }
}