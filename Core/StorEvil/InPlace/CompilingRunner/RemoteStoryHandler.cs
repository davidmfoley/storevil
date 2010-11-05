using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Infrastructure;

namespace StorEvil.InPlace
{
    public interface IRemoteStoryHandler : IStoryHandler, IDisposable { }

    public class RemoteStoryHandler : IRemoteStoryHandler
    {
        private readonly string _assemblyLocation;
        private AppDomain _appDomain;
        private readonly IFilesystem _filesystem;
        private readonly IEventBus _eventBus;

        private readonly IEnumerable<string> _assemblyLocations;
        private IStoryHandler _handler;

        public RemoteStoryHandler(string assemblyLocation, IFilesystem filesystem,IEventBus eventBus,
                                  IEnumerable<string> assemblyLocations)
        {
            _assemblyLocation = assemblyLocation;
          
            _assemblyLocations = assemblyLocations;
            _filesystem = filesystem;
            _eventBus = eventBus;
        }

        public bool InTest { get; set; }

        private string GetDirectories(IEnumerable<string> assemblyLocations)
        {
            var dirs = assemblyLocations.Select(x => Path.GetDirectoryName(x)).Distinct().ToArray();
            return string.Join(";", dirs.ToArray());
        }

        private IStoryHandler Handler
        {
            get
            {
                if (_handler == null)
                    _handler = GetHandler();
                return _handler;
            }
        }

        public JobResult HandleStories(IEnumerable<Story> stories)
        {
            return Handler.HandleStories(stories);            
        }

        private IStoryHandler GetHandler()
        {
            //return
            //    Activator.CreateInstanceFrom(
            //        _assemblyLocation,
            //        "StorEvilTestAssembly.StorEvilDriver", true, 0, null, new object[] {_eventBus},
            //        CultureInfo.CurrentCulture, new object[0], AppDomain.CurrentDomain.Evidence) as IStoryHandler;

            // Construct and initialize settings for a second AppDomain.
            var domainSetup = new AppDomainSetup();
            if (InTest)
            {
                domainSetup.ApplicationBase = Environment.CurrentDirectory;
                domainSetup.DisallowBindingRedirects = false;
            }
            else
            {
                domainSetup.ApplicationBase = Path.GetDirectoryName(_assemblyLocations.First());
                domainSetup.DisallowBindingRedirects = false;
            }
            domainSetup.ShadowCopyFiles = "true";
            domainSetup.ShadowCopyDirectories = GetDirectories(_assemblyLocations);
            domainSetup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            // Create the second AppDomain.
            _appDomain = AppDomain.CreateDomain("TestDomain", null, domainSetup);
          
            return _appDomain.CreateInstanceFrom(
                _assemblyLocation,
                "StorEvilTestAssembly.StorEvilDriver", true, 0, null, new object[] {_eventBus},
                CultureInfo.CurrentCulture, new object[0], AppDomain.CurrentDomain.Evidence).Unwrap() as IStoryHandler;
        }

        public void Dispose()
        {
            if (_appDomain != null)
                AppDomain.Unload(_appDomain);

            _filesystem.Delete(_assemblyLocation);
            string pdbFile = _assemblyLocation.Substring(0, _assemblyLocation.LastIndexOf(".")) + ".pdb";
            _filesystem.Delete(pdbFile);
        }
    }
}