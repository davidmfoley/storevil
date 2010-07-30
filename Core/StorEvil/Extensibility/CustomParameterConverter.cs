using System;

namespace StorEvil.Extensibility
{
    public abstract class CustomParameterConverter
    {
        public abstract object Convert(Type targetType, string asString);
        public static object CouldNotParse { get { return new CouldNotParseParameter(); } }
    }
}