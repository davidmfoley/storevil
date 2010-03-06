using System;
using System.Collections.Generic;

namespace StorEvil.Interpreter.ParameterConverters
{
    public class ParameterConverter
    {
        private static readonly Dictionary<Type, IStorevilConverter> _typeConverters =
            new Dictionary<Type, IStorevilConverter>();

        static ParameterConverter()
        {
            AddConverter<int>(x => int.Parse(ConvertHelper.StripNonNumeric(x)));
            AddConverter<decimal>(new StorevilDecimalConverter());
        }

        private static void AddConverter<T>(IStorevilConverter converter)
        {
            _typeConverters.Add(typeof (T), converter);
        }

        private static void AddConverter<T>(Func<string, object> func)
        {
            _typeConverters.Add(typeof (T), new SimpleConverter<T>(func));
        }

        public object Convert(string paramValue, Type type)
        {
            if (_typeConverters.ContainsKey(type))
                return _typeConverters[type].ConvertParamValue(paramValue);

            if (type.IsEnum)
                return ParseEnumValue(paramValue, type);

            return paramValue;
        }

        private static object ParseEnumValue(string value, Type type)
        {
            return Enum.Parse(type, value, true);
        }
    }
}