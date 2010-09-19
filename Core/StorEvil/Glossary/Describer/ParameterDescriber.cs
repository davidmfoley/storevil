using System;

namespace StorEvil.Core
{
    internal class ParameterDescriber
    {
        public string DescribeParameter( Type parameterType, string parameterName)
        {
            return string.Format("<{0} {1}>", TranslateTypeName(parameterType), parameterName);
        }

        private string TranslateTypeName(Type t)
        {
            if (t == typeof(int))
            {
                return "int";
            }
            if (t == typeof(double))
            {
                return "double";
            }
            if (t == typeof(string))
            {
                return "string";
            }
            if (t == typeof(bool))
            {
                return "bool";
            }
            return t.Name;
        }
    }
}