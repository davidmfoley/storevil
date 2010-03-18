using System;
using StorEvil.Utility;

namespace StorEvil.Interpreter.ParameterConverters
{
    public class TableToTypeConverter : IStorevilConverter
    {
        private readonly ParameterConverter _parameterConverter;

        public TableToTypeConverter(ParameterConverter parameterConverter)
        {
            _parameterConverter = parameterConverter;
        }

        public object ConvertParamValue(string val, Type destinationType)
        {
            var rows = (string[][])new StorEvilTableConverter(_parameterConverter).ConvertParamValue(val, typeof(string[][]));
            var converted = Activator.CreateInstance(destinationType);

            foreach (var row in rows)
            {
                var name = row[0];
                var propValue = row[1];
                var memberType = destinationType.GetMemberType(name);
                converted.SetWithReflection(name, _parameterConverter.Convert(propValue, memberType));
            }
            return converted;
        }
    }
}