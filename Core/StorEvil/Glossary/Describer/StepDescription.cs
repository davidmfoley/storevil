using System;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Core
{
    public class StepDescription
    {
        public string Description
        {
            get { 
                var spans = Spans.Select(x => x.Text).ToArray();
                return string.Join(" ", spans);
            }
        }
        public string ChildDescription = "";
        public IEnumerable<StepSpan> Spans = new StepSpan[] {};
    }

    public interface StepSpan
    {
        string Text { get; }
    }

    public class ParameterSpan : StepSpan
    {
        private readonly Type _parameterType;
        private readonly string _name;

        public ParameterSpan(Type parameterType, string name)
        {
            _parameterType = parameterType;
            _name = name;
        }

        public string Text
        {
            get
            {
                return string.Format("<{0} {1}>", TranslateTypeName(_parameterType), _name);
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
    public class TextSpan :StepSpan
    {
        public TextSpan(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }

}

