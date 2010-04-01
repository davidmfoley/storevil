using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StorEvil.Interpreter
{
    // TODO! right now this depends on static variables to store the extensions methods
    // should convert this to participate in the normal IoC and just be injected into every place that needs it!
    public class ExtensionMethodHandler
    {
        private static void AddExtensionMethods(Type type)
        {
            if (AlreadyAddedType(type))
                return;
            _addedTypes.Add(type);

            var publicStaticMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            var extensionMethods =
                publicStaticMethods.Where(methodInfo => methodInfo.IsStatic & methodInfo.GetParameters().Length > 0);

            
            foreach (var methodInfo in extensionMethods)
            {
                DebugTrace.Trace("ExtensionMethodHandler", "Adding extension method: " + type.Name + "." + methodInfo.Name);
                _allExtensionMethods.Add(methodInfo);
            }
        }

        private static bool AlreadyAddedType(Type type)
        {
            return _addedTypes.Contains(type);
        }

        private static readonly List<MethodInfo> _allExtensionMethods = new List<MethodInfo>();
        private static readonly List<Type> _addedTypes = new List<Type>();

        public IEnumerable<MethodInfo> GetExtensionMethodsFor(Type _type)
        {
            return _allExtensionMethods.Where(m => m.GetParameters()[0].ParameterType.IsAssignableFrom(_type));
        }

        public void AddAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(IsStatic);
            foreach (var type in types)
            {
                AddExtensionMethods(type);
            }
        }

        private bool IsStatic(Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }
    }
}