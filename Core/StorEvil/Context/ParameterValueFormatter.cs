using System;
using System.Collections.Generic;

namespace StorEvil.Context
{
    internal class ParameterValueFormatter
    {
        private delegate string ValueConverter(string input);

        private static readonly Dictionary<Type, ValueConverter> _typeToStringMap =
            new Dictionary<Type, ValueConverter>();

        static ParameterValueFormatter()
        {
            _typeToStringMap.Add(typeof (string), ValueToString);
            _typeToStringMap.Add(typeof (decimal), x => StringUtility.StripNonNumericFormatting(x));
            _typeToStringMap.Add(typeof (int), x => StringUtility.StripNonNumericFormatting(x));

            _typeToStringMap.Add(typeof (object), GuessFormatting);
        }

        public static string GetParamString(Type type, string s)
        {
            if (_typeToStringMap.ContainsKey(type))
            {
                return _typeToStringMap[type](s);
            }
            return GuessFormatting(s);
        }

        public static string GuessFormatting(string s)
        {
            int parsed;
            if (int.TryParse(s, out parsed))
                return parsed.ToString();

            decimal decimalResult;
            if (decimal.TryParse(s, out decimalResult))
                return decimalResult.ToString();

            DateTime dateResult;
            if (DateTime.TryParse(s, out dateResult))
                return string.Format("new DateTime({0},{1},{2},{3},{4},{5})",
                                     dateResult.Year, dateResult.Month, dateResult.Day, dateResult.Hour,
                                     dateResult.Minute, dateResult.Second);

            return ValueToString(s);
        }

        public static string ValueToString(string value)
        {
            // escape double quotes
            return "@\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}