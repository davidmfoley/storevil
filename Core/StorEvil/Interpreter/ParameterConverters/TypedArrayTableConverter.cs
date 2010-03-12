using System;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Interpreter.ParameterConverters
{
    public class TypedArrayTableConverter : IStorevilConverter
    {
        private readonly ParameterConverter _parameterConverter;
        private ArrayBuilder _arrayBuilder = new ArrayBuilder();

        public TypedArrayTableConverter(ParameterConverter parameterConverter)
        {
            _parameterConverter = parameterConverter;
        }

        public object ConvertParamValue(string val, Type type)
        {
            var rows =(string[][]) new StorEvilTableConverter().ConvertParamValue(val, typeof(string[][]));

            return BuildTypedArray(rows, type);
        }

        private object BuildTypedArray(string[][] rows, Type type)
        {
            var fieldNames = rows.First().Select(x=>x.Trim());
            var destinationType = type.GetElementType();
            var setters = fieldNames.Select(f => GetSetter(destinationType, f)).ToArray();
            var types = fieldNames.Select(f => GetMemberType(destinationType, f)).ToArray();
            
            rows = rows.Skip(1).ToArray();

            var items = new List<object>();

            foreach (string[] currentRow in rows)
            {
                var instance = Activator.CreateInstance(destinationType);

                for (int column = 0; column < currentRow.Length; column++)
                {
                    // create instance
                    var value = _parameterConverter.Convert(currentRow[column], types[column]);
                    setters[column](instance, value);
                }

                items.Add(instance);
            }

            return _arrayBuilder.BuildArrayOfType(destinationType, items);
        }

       

        private Type GetMemberType(Type destinationType, string memberName)
        {
            var propertyInfo = destinationType.GetProperty(memberName);
            if (null != propertyInfo)
                return propertyInfo.PropertyType;

            var fieldInfo = destinationType.GetField(memberName);
            if (null != fieldInfo)
                return fieldInfo.FieldType;
            
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
}