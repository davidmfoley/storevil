using System;

namespace StorEvil.Interpreter.ParameterConverters
{
    internal class SimpleConverter<T> : IStorevilConverter
    {
        private readonly Func<string, object> _convertFunc;

        public SimpleConverter(Func<string, object> func)
        {
            _convertFunc = func;
        }

        public object ConvertParamValue(string val, Type destinationType)
        {
            return _convertFunc(val);
        }
    }
}