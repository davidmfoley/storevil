using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Interpreter.ParameterConverters
{
    public class StorEvilTableConverter : IStorevilConverter
    {
        private readonly ParameterConverter _converter;

        public StorEvilTableConverter(ParameterConverter converter)
        {
            _converter = converter;
        }

        public object ConvertParamValue(string val, Type destinationType)
        {
            var t = new ArrayList();
            var elementType = destinationType.GetElementType();
            var innerElementType = elementType.GetElementType();
            var rows = val.Split(new[] {"\r\n"}, StringSplitOptions.None);

            foreach (var row in rows)
            {
                var values = row.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                var destRow = new ArrayList();
                foreach (var converted in values.Select(t1 => _converter.Convert(t1.Trim(), innerElementType)))
                {
                    destRow.Add(converted);
                }
                t.Add(destRow.ToArray(innerElementType));
                
            }

            return t.ToArray(elementType);
        }
    }
}