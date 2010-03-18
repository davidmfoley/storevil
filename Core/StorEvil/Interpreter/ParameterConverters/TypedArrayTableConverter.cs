using System;
using System.Collections.Generic;
using System.Linq;
using StorEvil.Utility;

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
            var rows =(string[][]) new StorEvilTableConverter(_parameterConverter).ConvertParamValue(val, typeof(string[][]));

            return BuildTypedArray(rows, type);
        }

        private object BuildTypedArray(string[][] rows, Type type)
        {
            var fieldNames = rows.First().Select(x=>x.Trim());
            var destinationType = type.GetElementType();
            var setters = fieldNames.Select(f => destinationType.GetSetter(f)).ToArray();
            var types = fieldNames.Select(f => destinationType.GetMemberType(f)).ToArray();
            
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
    }
}