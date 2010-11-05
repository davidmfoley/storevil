using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StorEvil.Context
{
    public class AssemblyRegistry
    {
        private IEnumerable<Type> _allTypes;
        private IEnumerable<string> _assemblyLocations;

        public AssemblyRegistry()
        {
            _allTypes = new Type[0];
        }

        public AssemblyRegistry(IEnumerable<Assembly> assemblies)
        {
            _assemblyLocations = assemblies.Select(x => x.Location);
            _allTypes = assemblies.SelectMany(a => a.GetTypes());
        }
        public AssemblyRegistry(IEnumerable<string> assemblyLocations)
        {
            _assemblyLocations = assemblyLocations;
            _allTypes = assemblyLocations.Select(LoadAssembly).SelectMany(a => a.GetTypes());
        }

        private static Assembly LoadAssembly(string location)
        {
            if (File.Exists(location))
                return Assembly.LoadFrom(location);

            var inWorkingDirectory = Path.GetFileName(location);

            if (File.Exists(inWorkingDirectory))
                return Assembly.LoadFrom(inWorkingDirectory);

            throw new StorEvilException("Could not load assembly: " + location);
        }

        public virtual IEnumerable<Type> GetTypesWithCustomAttribute<T>()
        {
            return _allTypes.Where(t => TypeHasCustomAttribute(t, typeof (T)));               
        }
    
        private static bool TypeHasCustomAttribute(Type t, Type customAttribute)
        {
            // tolerate version differences between runner and target of context assembly
            return t.GetCustomAttributes(true).Any(x => x.GetType().FullName == customAttribute.FullName);
        }

        public virtual IEnumerable<Type> GetTypesImplementing(Type targetType, bool byName)
        {
            if (byName)
                return _allTypes.Where(t => t.IsPublic && ImplementsName(t, targetType));
            return _allTypes.Where(t => t.IsPublic && Implements(t, targetType));
        }

        private bool ImplementsName(Type t, Type targetType)
        {
            var parentTypes = GetParentTypes(t).ToArray();
            if (parentTypes.Any(x => x.FullName == targetType.FullName))
                return true;

            if (!targetType.IsInterface)
                return false;

            if (targetType.IsGenericType)
                 return t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().FullName == targetType.FullName);

            return t.GetInterfaces().Any(x => x.FullName == targetType.FullName);
        }

        private IEnumerable<Type> GetParentTypes(Type type)
        {
            Type current = type.BaseType;

            while (current != null && typeof(object) != current) 
            {
                yield return current;
                current = current.BaseType;
            }
        }

        private bool Implements(Type t, Type targetType)
        {
            return t.IsSubclassOf(targetType) 
                   || t.GetInterfaces().Contains(targetType) 
                   || t.GetInterfaces().Any(i=>i.IsGenericType && i.GetGenericTypeDefinition() == targetType);
        }

        public IEnumerable<Type> GetStaticClasses()
        {
            return _allTypes.Where(IsStatic);

        }

        private bool IsStatic(Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }

        public IEnumerable<string> GetAssemblyLocations()
        {
            return _assemblyLocations;
        }
    }
}