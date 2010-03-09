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
            AddConverter<string[][]>(new StorEvilTableConverter());
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

    public class StorEvilTableConverter : IStorevilConverter
    {
        public object ConvertParamValue(string val)
        {
            var table = new List<string[]>();
            var rows = val.Split(new[] {"\r\n"}, StringSplitOptions.None);

            foreach (var row in rows)
                table.Add(row.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries));

            return table.ToArray();            
        }
    }
}