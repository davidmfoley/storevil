using System;
using System.Collections.Generic;
using System.Linq;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    public class ContextViewer
    {
        private CodeCompiler _compiler = new CodeCompiler();

        public object Create(Dictionary<Type, object> dictionary)
        {
            var lines = dictionary.Select(x => BuildProperty(x.Key)).ToArray();
            var propertySource = string.Join("\r\n", lines);
            var source = "public class DebugContext { \r\n" + propertySource + "\r\n}";

            var assembly = _compiler.CompileInMemory(source, new[] { GetType().Assembly.Location });
            var context = Activator.CreateInstance(assembly.GetTypes().First());
            foreach (var type in dictionary.Keys)
            {
                context.SetWithReflection(type.Name, dictionary[type]);
            }

            return context;
        }

        private string BuildProperty(Type type)
        {
            return string.Format("    public {0} {1} {{ get;set; }}", type.FullName, type.Name);
        }
    }
}