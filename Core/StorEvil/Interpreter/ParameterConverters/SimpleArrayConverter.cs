using System;
using System.Linq;

namespace StorEvil.Interpreter.ParameterConverters
{
    public class SimpleArrayConverter : IStorevilConverter
    {
        private readonly ParameterConverter _parameterConverter;
        private ArrayBuilder _arrayBuilder = new ArrayBuilder();

        public SimpleArrayConverter(ParameterConverter parameterConverter)
        {
            _parameterConverter = parameterConverter;
        }

        public object ConvertParamValue(string val, Type destinationType)
        {
            var elementType = destinationType.GetElementType();
                    
            var parsed = val.Split(',');
            var converted = parsed.Select(x=>ConvertElement(x, elementType)).Where(x=>x != null);
            return _arrayBuilder.BuildArrayOfType(elementType, converted);
        }

        private object ConvertElement(string x, Type elementType)
        {
            if (string.IsNullOrEmpty(x))
                return null;

            return _parameterConverter.Convert(x, elementType);
        }
    }
}