using System;
using System.Globalization;
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
        private readonly AppDomain _appDomain;
        private IFilesystem _filesystem;
        private IResultListener _listener;

        public RemoteStoryHandler(string assemblyLocation, IFilesystem filesystem, IResultListener listener)
        {
            _assemblyLocation = assemblyLocation;
            _listener = listener;
            _filesystem = filesystem;

            // Construct and initialize settings for a second AppDomain.
            var domainSetup = new AppDomainSetup();
            domainSetup.ApplicationBase = Environment.CurrentDirectory;
            domainSetup.DisallowBindingRedirects = false;
            domainSetup.DisallowCodeDownload = true;
            domainSetup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            // Create the second AppDomain.
            _appDomain = AppDomain.CreateDomain("TestDomain", null, domainSetup);

            Handler = _appDomain.CreateInstanceFrom(
                _assemblyLocation,
                "StorEvilTestAssembly.StorEvilDriver",true, 0, null, new object[] {listener}, CultureInfo.CurrentCulture, new object[0], AppDomain.CurrentDomain.Evidence).Unwrap() as IStoryHandler;
        }

        public IStoryHandler Handler { get; private set; }

        public void Dispose()
        {
            AppDomain.Unload(_appDomain);
            _filesystem.Delete(_assemblyLocation);
        }
    }
}