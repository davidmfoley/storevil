using System;
using System.Reflection;

namespace StorEvil.CodeGeneration
{
    public class NUnitAssertWrapper
    {
        private readonly string _nunitAssemblyName;
        private Assembly _assembly;

        public NUnitAssertWrapper()
            : this("nunit.framework")
        {
        }

        public NUnitAssertWrapper(string nunitAssemblyName)
        {
            _nunitAssemblyName = nunitAssemblyName;
        }

        public void Fail(string message)
        {
            InvokeMethod("Fail", message, () => new StorEvil.AssertionException(message));
        }

        public void Ignore(string message)
        {
            InvokeMethod("Ignore", message, () => new StorEvil.IgnoreException(message));
        }

        private MethodInfo GetNUnitMethod(Assembly assembly, string name)
        {
            return assembly.GetType("NUnit.Framework.Assert").GetMethod(name, new[] { typeof(string) });
        }

        private Assembly GetNUnitAssembly()
        {
            return _assembly ?? (_assembly = Assembly.Load(_nunitAssemblyName, AppDomain.CurrentDomain.Evidence));
        }

        private void InvokeMethod(string methodName, string message, Func<Exception> fallback)
        {
            MethodInfo method;

            try
            {
                Assembly assembly = GetNUnitAssembly();
                method = GetNUnitMethod(assembly, methodName);
            }
            catch
            {
                throw fallback();
            }

            try
            {
                method.Invoke(null, new[] { message });
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
            }
        }
    }
}