using System;

namespace StorEvil.Interpreter.ParameterConverters
{
    public class StorevilDecimalConverter : IStorevilConverter
    {
        public object ConvertParamValue(string val, Type destinationType)
        {
            string stripped = ConvertHelper.StripNonNumeric(val);

            return Convert.ToDecimal(stripped);
        }
    }
}