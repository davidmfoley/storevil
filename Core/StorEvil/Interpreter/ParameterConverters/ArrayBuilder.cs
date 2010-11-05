using System;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Interpreter.ParameterConverters
{
    internal class ArrayBuilder
    {
        public Array BuildArrayOfType(Type elementType, IEnumerable<object> items)
        {
            var typed = Array.CreateInstance(elementType, items.Count());
            var i = 0;
            foreach (var item in items)
                typed.SetValue(item, i++);

            return typed;
        }
    }
}