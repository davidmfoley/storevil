using System;
using System.Reflection;
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
       
        public static ISessionContext SessionContext
        {
            get { return _sessionContext = _sessionContext ?? GetSessionContext(); }
        }

        public static bool IsInitialized
        {
            get { return _sessionContext != null; }
        }

        private static SessionContext GetSessionContext()
        {
            _sessionContext = new SessionContext();
            var configReader = new FilesystemConfigReader(new Filesystem(), new ConfigParser());
            var currentAssemblyLocation = Assembly.GetExecutingAssembly().Location;

            ConfigSettings settings = configReader.GetConfig(currentAssemblyLocation);

            foreach (var location in settings.AssemblyLocations)
                _sessionContext.AddAssembly(location);

            return _sessionContext;
        }
    }
}