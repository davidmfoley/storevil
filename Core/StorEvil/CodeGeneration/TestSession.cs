using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Infrastructure;
using StorEvil.Interpreter;
using StorEvil.Interpreter.ParameterConverters;

namespace StorEvil.CodeGeneration
{
    public class TestSession
    {
        private static SessionContext _sessionContext;
        private static List<Assembly> _assemblies = new List<Assembly>();

        public static TestSessionContextFactory TestSessionContextFactory = new TestSessionContextFactory();

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

        public static void AddAssembly(Assembly a)
        {
            _assemblies.Add(a);    
        }

        public static void EndSession()
        {
            if (_sessionContext == null)
                return;

            _sessionContext.Dispose();
            _sessionContext = null;
        }            

        public static bool IsInitialized
        {
            get { return _sessionContext != null; }
        }

        private static SessionContext GetSessionContext(string currentAssemblyLocation)
        {
            return TestSessionContextFactory.GetSessionContext(currentAssemblyLocation, _assemblies);
        }
    }

    public class TestSessionContextFactory
    {
        private static NUnitAssertWrapper _nunitAssertWrapper = new NUnitAssertWrapper();

        public virtual SessionContext GetSessionContext(string currentAssemblyLocation, IEnumerable<Assembly> assemblies)
        {
            var configReader = new FilesystemConfigReader(new Filesystem(), new ConfigParser());

            ConfigSettings settings = configReader.GetConfig(currentAssemblyLocation);
            if (!settings.AssemblyLocations.Any())
                settings = configReader.GetConfig(Directory.GetCurrentDirectory());

            if (!settings.AssemblyLocations.Any() && !assemblies.Any())
            {
                var message = "No storevil assemblies were found.\r\nCurrent location:"
                    + currentAssemblyLocation + "\r\nCurrent directory:"
                    + Directory.GetCurrentDirectory();

                _nunitAssertWrapper.Ignore(message);
            }

            var assemblyRegistry = new AssemblyRegistry(assemblies.Select(x => x.Location).Union(settings.AssemblyLocations));

            ParameterConverter.AddCustomConverters(assemblyRegistry);

            var extensionMethodHandler = new ExtensionMethodHandler(assemblyRegistry);
            return new SessionContext(assemblyRegistry);
            
        }
    }
}