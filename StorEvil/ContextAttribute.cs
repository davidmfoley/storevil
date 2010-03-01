using System;

namespace StorEvil
{
   
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ContextAttribute : Attribute
    {
       
    }
}
