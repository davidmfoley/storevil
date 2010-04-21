using System;
using System.Collections.Generic;
using StorEvil.Utility;

namespace StorEvil.NUnit
{
    internal class ParameterValueFormatter
    {
        private delegate string ValueConverter(string input);

        private static readonly Dictionary<Type, ValueConverter> _typeToStringMap =
            new Dictionary<Type, ValueConverter>();

        static ParameterValueFormatter()
        {
            _typeToStringMap.Add(typeof (string), ValueToString);
            _typeToStringMap.Add(typeof (decimal), x => x.StripNonNumericFormatting());
            _typeToStringMap.Add(typeof (int), x => x.StripNonNumericFormatting());
         }

        public static string GetParamString(Type type, object s)
        {
            if (type == typeof(string))
                return ValueToString((string)s);
           
            return GuessFormatting(type, s);
        }

        public static string GuessFormatting(Type type, object s)
        {
            var fmt = "({0}) this.ParameterConverter.Convert({1}, typeof({0}))";

            return String.Format(fmt, type.FullName.Replace("+", "."), ValueToString(s.ToString()));
        }

        public static string ValueToString(string value)
        {
            // escape double quotes
            return "@\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}