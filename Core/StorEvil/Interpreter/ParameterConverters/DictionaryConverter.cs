using System;
using System.Collections;
using System.Collections.Generic;

namespace StorEvil.Interpreter.ParameterConverters
{
    public class DictionaryConverter : IStorevilConverter
    {
        public ParameterConverter ParameterConverter { get; set; }

        public DictionaryConverter(ParameterConverter parameterConverter)
        {
            ParameterConverter = parameterConverter;
        }

        public object ConvertParamValue(string val, Type destinationType)
        {
            var rows = (string[][])new StorEvilTableConverter(ParameterConverter).ConvertParamValue(val.Trim(), typeof(string[][]));
            var converted = Activator.CreateInstance(destinationType) as IDictionary;
            foreach (var row in rows)
            {
                var name = row[0];
                var propValue = row[1];
                
                converted[name] = propValue;
            }

            return converted;
        }
    }
}