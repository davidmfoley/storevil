using System;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Interpreter.ParameterConverters
{
    public class ParameterConverter
    {
        class ConverterInfo
        {
           public Predicate<Type> Predicate;
            public IStorevilConverter Converter;

            public ConverterInfo(Predicate<Type> predicate, IStorevilConverter converter)
            {
                Predicate = predicate;
                Converter = converter;
            }
        }
        private static List<ConverterInfo> _typeConverters = new List<ConverterInfo>();

        public ParameterConverter()
        {
            AddConverter<int>(x => int.Parse(ConvertHelper.StripNonNumeric(x)));
            AddConverter<decimal>(new StorevilDecimalConverter());
            AddConverter<string[][]>(new StorEvilTableConverter());
            AddConverterFilter(t=> t.IsArray, new TypedArrayConverter(this));
        }

        private static void AddConverterFilter(Predicate<Type> predicate, IStorevilConverter converter)
        {
            _typeConverters.Add( new ConverterInfo(predicate, converter));;
        }
        private static void AddConverter<T>(IStorevilConverter converter)
        {
            _typeConverters.Add(new ConverterInfo(t => t == typeof (T), converter));
        }

        private static void AddConverter<T>(Func<string, object> func)
        {
            _typeConverters.Add( new ConverterInfo(t => t == typeof (T), new SimpleConverter<T>(func)));
        }

        public object Convert(string paramValue, Type type)
        {
            foreach (var storevilConverter in _typeConverters)
            {
                var predicate = storevilConverter.Predicate;
                if (predicate(type))
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

    public class TypedArrayConverter : IStorevilConverter
    {
        private readonly ParameterConverter _parameterConverter;

        public TypedArrayConverter(ParameterConverter parameterConverter)
        {
            _parameterConverter = parameterConverter;
        }

        public object ConvertParamValue(string val, Type type)
        {
            var rows =(string[][]) new StorEvilTableConverter().ConvertParamValue(val, typeof(string[][]));
            var fieldNames = rows.First();
            var destinationType = type.GetElementType();
            var setters = fieldNames.Select(f => GetSetter(destinationType, f)).ToArray();
            var types = fieldNames.Select(f => GetMemberType(destinationType, f)).ToArray();
            
            rows = rows.Skip(1).ToArray();

            var typed = Array.CreateInstance(destinationType, rows.Length);

            for (int row = 0; row < rows.Length; row++)
            {
                var instance = Activator.CreateInstance(destinationType);
                typed.SetValue(instance, row);
                for (int column = 0; column < rows[row].Length; column++)
                {
                    // create instance
                    var value = _parameterConverter.Convert(rows[row][column], types[column]);
                    setters[column](instance, value);
                }
            }

            return typed;

        }

        private Type GetMemberType(Type destinationType, string memberName)
        {
            var propertyInfo = destinationType.GetProperty(memberName);
            if (null != propertyInfo)
            {
                return propertyInfo.PropertyType;
            }

            var fieldInfo = destinationType.GetField(memberName);
            if (null != fieldInfo)
            {
                return fieldInfo.FieldType;
            }
            return null;
        }

        private Action<object, object> GetSetter(Type destinationType, string memberName)
        {
            var propertyInfo = destinationType.GetProperty(memberName);
            if (null != propertyInfo)
            {
                return (o, v) => propertyInfo.SetValue(o, v, null);
            }

            var fieldInfo = destinationType.GetField(memberName);
            if (null != fieldInfo)
            {
                return (o, v) => fieldInfo.SetValue(o, v);
            }

            throw new UnknownFieldException(destinationType, memberName);
        }
    }

    public class UnknownFieldException : Exception
    {
        public Type DestinationType { get; set; }
        public string MemberName { get; set; }

        public UnknownFieldException(Type destinationType, string memberName)
        {
            DestinationType = destinationType;
            MemberName = memberName;
        }
        public override string Message
        {
            get
            {
                return "Unknown field:" + MemberName + " on type: " + DestinationType.Name;
            }
        }
    }

    public class StorEvilTableConverter : IStorevilConverter
    {
        public object ConvertParamValue(string val, Type destinationType)
        {
            var table = new List<string[]>();
            var rows = val.Split(new[] {"\r\n"}, StringSplitOptions.None);
           
            foreach (var row in rows)
                table.Add(row.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries));

            return table.ToArray();            
        }

        
    }
}