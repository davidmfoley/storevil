using System;

namespace StorEvil.Interpreter.ParameterConverters
{
    internal interface IStorevilConverter
    {
        object ConvertParamValue(string val, Type destinationType);
    }
}