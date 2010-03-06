using System;
using System.Collections.Generic;

namespace StorEvil.Interpreter
{
    public class ParameterConverter
    {
        private static readonly Dictionary<Type, IStorevilConverter> _typeConverters = new Dictionary<Type, IStorevilConverter>();

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
            _typeConverters.Add(typeof(T), new SimpleConverter<T>(func));
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

    public class StorevilDecimalConverter : IStorevilConverter
    {
        public object ConvertParamValue(string val)
        {
            string stripped = ConvertHelper.StripNonNumeric(val);

            return Convert.ToDecimal(stripped);
        }

       
    }

    public class ConvertHelper
    {
        public static string StripNonNumeric(string val)
        {
            var digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            return val.Substring(val.IndexOfAny(digits));
        }
    }

    internal class SimpleConverter<T> : IStorevilConverter
    {
        private readonly Func<string, object> _convertFunc;

        public SimpleConverter(Func<string, object> func)
        {
            _convertFunc = func;
        }

        public object ConvertParamValue(string val)
        {
            return _convertFunc(val);
        }
    }

    internal interface IStorevilConverter
    {
        object ConvertParamValue(string val);
    }
}