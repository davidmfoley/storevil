using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Infrastructure;
using StorEvil.Interpreter.ParameterConverters;

namespace StorEvil.CodeGeneration
{
    public class TestSession
    {
        private static SessionContext _sessionContext;
        private static List<Assembly> _assemblies = new List<Assembly>();
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
            _sessionContext = new SessionContext();
            var configReader = new FilesystemConfigReader(new Filesystem(), new ConfigParser());     

            ConfigSettings settings = configReader.GetConfig(currentAssemblyLocation);
            if (!settings.AssemblyLocations.Any())
                settings = configReader.GetConfig(Directory.GetCurrentDirectory());

            if (!settings.AssemblyLocations.Any() && !_assemblies.Any())
            {
                Assert.Ignore("No storevil assemblies were found.");
            }
   
            foreach (var location in settings.AssemblyLocations)
            {
                _sessionContext.AddAssembly(location);
                ParameterConverter.AddCustomConverters(location);
            }

            foreach (var assembly in _assemblies)
            {
                _sessionContext.AddAssembly(assembly);
            }

            return _sessionContext;
        }
    }
}