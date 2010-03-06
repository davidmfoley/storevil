namespace StorEvil.Interpreter.ParameterConverters
{
    public class ConvertHelper
    {
        public static string StripNonNumeric(string val)
        {
            var digits = new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            return val.Substring(val.IndexOfAny(digits));
        }
    }
}