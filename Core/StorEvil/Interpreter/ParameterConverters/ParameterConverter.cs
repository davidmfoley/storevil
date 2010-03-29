using System;
using System.Collections;
using System.Collections.Generic;

namespace StorEvil.Interpreter.ParameterConverters
{
    public class ParameterConverter
    {
        class ConversionContext
        {
            public Type ParameterType { get; set; }
            public string Value { get; set; }
        }

        class ConverterInfo
        {
            public Predicate<ConversionContext> Predicate;
            public IStorevilConverter Converter;

            public ConverterInfo(Predicate<ConversionContext> predicate, IStorevilConverter converter)
            {
                Predicate = predicate;
                Converter = converter;
            }
        }

        private static readonly List<ConverterInfo> _typeConverters = new List<ConverterInfo>();

        public ParameterConverter()
        {
            AddConverter<int>(x => int.Parse(ConvertHelper.StripNonNumeric(x)));
            AddConverter<decimal>(new StorevilDecimalConverter());
            AddConverter<DateTime>(x=>DateTime.Parse(x));
           
            AddConverterFilter(IsArrayOfArrays, new StorEvilTableConverter(this));
            AddConverterFilter(IsTypedArrayTable, new TypedArrayTableConverter(this));
            AddConverterFilter(IsCommaSeparatedArray, new SimpleArrayConverter(this));
            AddConverterFilter(IsDictionary, new DictionaryConverter(this));
            AddConverterFilter(IsCustomTypeWithTable, new TableToTypeConverter(this));    
        }

        private bool IsArrayOfArrays(ConversionContext context)
        {
            var parameterType = context.ParameterType;
            if (!parameterType.IsArray)
                return false;

            var elementType = parameterType.GetElementType();
            return elementType != null && elementType.IsArray;
        }

        private bool IsDictionary(ConversionContext x)
        {
            return typeof(IDictionary).IsAssignableFrom( x.ParameterType) && x.Value.StartsWith("|");
        }

        private bool IsTypedArrayTable(ConversionContext x)
        {
            return x.ParameterType.IsArray && x.Value.StartsWith("|");
        }

        private bool IsCustomTypeWithTable(ConversionContext x)
        {
            return (!x.ParameterType.Assembly.FullName.StartsWith("System.")) && x.Value.StartsWith("|");
        }

        private bool IsCommaSeparatedArray(ConversionContext x)
        {
            // handle the case where there is no comma
            return x.ParameterType.IsArray;
        }

        private static void AddConverterFilter(Predicate<ConversionContext> predicate, IStorevilConverter converter)
        {
            _typeConverters.Add( new ConverterInfo(predicate, converter));;
        }
        private static void AddConverter<T>(IStorevilConverter converter)
        {
            _typeConverters.Add(new ConverterInfo(t => t.ParameterType == typeof (T), converter));
        }

        private static void AddConverter<T>(Func<string, object> func)
        {
            _typeConverters.Add( new ConverterInfo(t => t.ParameterType == typeof (T), new SimpleConverter<T>(func)));
        }

        public object Convert(string paramValue, Type type)
        {
            var conversionContext = new ConversionContext
            {
                ParameterType = type,
                Value = paramValue
            };

            foreach (var storevilConverter in _typeConverters)
            {
                var predicate = storevilConverter.Predicate;

                if (predicate(conversionContext))
                {
                     return storevilConverter.Converter.ConvertParamValue(paramValue, type);
                }
            }
            
            if (type.IsEnum)
                return ParseEnumValue(paramValue, type);

            return paramValue;
        }

        private static object ParseEnumValue(string value, Type type)
        {
            return Enum.Parse(type, value.Replace(" ", ""), true);
        }
    }
}