using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using StorEvil.Core;
using StorEvil.Infrastructure;

namespace StorEvil.InPlace
{
    public interface IRemoteStoryHandler : IDisposable
    {
        IStoryHandler Handler { get; }
    }

    public class RemoteStoryHandler : IRemoteStoryHandler
    {
        private readonly string _assemblyLocation;
        private AppDomain _appDomain;
        private readonly IFilesystem _filesystem;
        private readonly IResultListener _listener;
        private readonly IEnumerable<string> _assemblyLocations;
        private IStoryHandler _handler;

        public RemoteStoryHandler(string assemblyLocation, IFilesystem filesystem, IResultListener listener,
                                  IEnumerable<string> assemblyLocations)
        {
            _assemblyLocation = assemblyLocation;
            _listener = listener;
            _assemblyLocations = assemblyLocations;
            _filesystem = filesystem;
        }

        public bool InTest { get; set; }

        private string GetDirectories(IEnumerable<string> assemblyLocations)
        {
            var dirs = assemblyLocations.Select(x => Path.GetDirectoryName(x)).Distinct().ToArray();
            return string.Join(";", dirs.ToArray());
        }

        public virtual IStoryHandler Handler
        {
            get
            {
                if (_handler == null)
                    _handler = GetHandler();
                return _handler;
            }
        }

        private IStoryHandler GetHandler()
        {
            // Construct and initialize settings for a second AppDomain.
            var domainSetup = new AppDomainSetup();
            if (InTest)
            {
                domainSetup.ApplicationBase = Environment.CurrentDirectory;
                domainSetup.DisallowBindingRedirects = false;
            }
            domainSetup.ShadowCopyDirectories = GetDirectories(_assemblyLocations);
            domainSetup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            // Create the second AppDomain.
            _appDomain = AppDomain.CreateDomain("TestDomain", null, domainSetup);

            return _appDomain.CreateInstanceFrom(
                _assemblyLocation,
                "StorEvilTestAssembly.StorEvilDriver", true, 0, null, new object[] {_listener},
                CultureInfo.CurrentCulture, new object[0], AppDomain.CurrentDomain.Evidence).Unwrap() as IStoryHandler;
        }

        public void Dispose()
        {
            if (_appDomain != null)
                AppDomain.Unload(_appDomain);

            _filesystem.Delete(_assemblyLocation);
        }
    }
}