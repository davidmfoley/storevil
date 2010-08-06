namespace StorEvil.Interpreter.ParameterConverters
{
    public class ConvertHelper
    {
        public static string StripNonNumeric(string val)
        {
            var isNegative = (val.Trim().StartsWith("-"));

            var digits = new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            var firstDigitPosition = val.IndexOfAny(digits);
            if (firstDigitPosition == -1)
                return "";

            return (isNegative ? "-" : "") + val.Substring(firstDigitPosition);
        }
    }
}