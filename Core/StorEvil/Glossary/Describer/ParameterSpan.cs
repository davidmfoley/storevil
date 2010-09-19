using System;

namespace StorEvil.Core
{
    public class ParameterSpan : StepSpan
    {
        public Type ParameterType { get; private set; }
        public string Name { get; private set; }

        public ParameterSpan(Type parameterType, string name)
        {
            ParameterType = parameterType;
            Name = name;
        }

        public string Text
        {
            get
            {
                return string.Format("<{0} {1}>", TranslateTypeName(ParameterType), Name);
            }}

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