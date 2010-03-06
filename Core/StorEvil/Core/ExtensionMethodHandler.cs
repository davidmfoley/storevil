using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StorEvil.Core
{
    public class ExtensionMethodHandler
    {
        static ExtensionMethodHandler()
        {
            AddExtensionMethods(typeof (TestExtensionMethods));
        }
        private static void AddExtensionMethods(Type type)
        {
            var publicStaticMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            var extensionMethods = publicStaticMethods.Where(methodInfo => methodInfo.IsStatic & methodInfo.GetParameters().Length > 0 );

            foreach (var methodInfo in
                extensionMethods)
            {
                _allExtensionMethods.Add(methodInfo);
            }
        }

        private static readonly List<MethodInfo> _allExtensionMethods = new List<MethodInfo>();

        public IEnumerable<MethodInfo> GetExtensionMethodsFor(Type _type)
        {
            // for now, just local
            return _allExtensionMethods.Where(m => m.GetParameters()[0].ParameterType.IsAssignableFrom(_type));

        
        }

    }
}