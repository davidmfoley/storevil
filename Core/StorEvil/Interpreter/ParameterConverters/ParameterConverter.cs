using System;
using System.Collections.Generic;
using System.Linq;

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
            AddConverter<string[][]>(new StorEvilTableConverter());
            AddConverterFilter(IsTypedArrayTable, new TypedArrayTableConverter(this));
            AddConverterFilter(IsCommaSeparatedArray, new SimpleArrayConverter(this));
        }

        private bool IsTypedArrayTable(ConversionContext x)
        {
            return x.ParameterType.IsArray && x.Value.StartsWith("|");
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
                    return storevilConverter.Converter.ConvertParamValue(paramValue, type);
            }
            
            if (type.IsEnum)
                return ParseEnumValue(paramValue, type);

            return paramValue;
        }

        private static object ParseEnumValue(string value, Type type)
        {
            return Enum.Parse(type, value, true);
        }
    }

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