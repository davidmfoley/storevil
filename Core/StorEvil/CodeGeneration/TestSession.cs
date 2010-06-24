using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Infrastructure;

namespace StorEvil.CodeGeneration
{
    public class TestSession
    {
        private static SessionContext _sessionContext;
        
        public static void ShutDown()
        {
            if (_sessionContext != null)
                ((IDisposable) _sessionContext).Dispose();

            _sessionContext = null;
        }
       
        public static ISessionContext SessionContext(string location)
        {
            return _sessionContext = _sessionContext ?? GetSessionContext(location); 
        }

        public static bool IsInitialized
        {
            get { return _sessionContext != null; }
        }

        private static SessionContext GetSessionContext(string currentAssemblyLocation)
        {
            _sessionContext = new SessionContext();
            var configReader = new FilesystemConfigReader(new Filesystem(), new ConfigParser());     

            ConfigSettings settings = configReader.GetConfig(currentAssemblyLocation);
            if (!settings.AssemblyLocations.Any())
                settings = configReader.GetConfig(Directory.GetCurrentDirectory());

            if (!settings.AssemblyLocations.Any())
            {
                Assert.Ignore("No storevil config file was found.");
            }
            foreach (var location in settings.AssemblyLocations)
                _sessionContext.AddAssembly(location);

            return _sessionContext;
        }
    }
}