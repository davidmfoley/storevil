using System;
using System.Collections.Generic;

namespace StorEvil.Interpreter.ParameterConverters
{
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