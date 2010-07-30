using System;
using StorEvil.Extensibility;

namespace Tutorial
{
    public class FloatConverter : CustomParameterConverter
    {
        public override object Convert(Type targetType, string asString)
        {           
            if (targetType != typeof(float))
                return CouldNotParse;
                
            return float.Parse(asString);           
        }
    }
}