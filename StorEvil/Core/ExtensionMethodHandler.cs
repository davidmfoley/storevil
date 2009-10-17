using System;
using System.Collections.Generic;
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
            foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (methodInfo.IsStatic & methodInfo.GetParameters().Length > 0)
                {
                    _allExtensionMethods.Add(methodInfo);
                }
            }
        }

        private static readonly List<MethodInfo> _allExtensionMethods = new List<MethodInfo>();

        public IEnumerable<MethodInfo> GetExtensionMethodsFor(Type _type)
        {
            // for now, just local

            foreach (var methodInfo in _allExtensionMethods)
            {
                var parameterType = methodInfo.GetParameters()[0].ParameterType;
                if (parameterType.IsAssignableFrom(_type))
                {
                    yield return methodInfo;
                }
            }
        }

    }
}