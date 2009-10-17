using System;

namespace StorEvil
{
    /// <summary>
    /// When this attribute is on a class it specifies which story/spec is handled
    ///  note that this direction of association doesn't always make sense
    /// ... somethimes you will want the spec to define the context
    /// and sometimes you will want it to be externally configured
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ContextAttribute : Attribute
    {
        public ContextAttribute(params string[] positionalString)
        {
            HandledNames = positionalString;
        }

        public string[] HandledNames { get; private set; }
    }

   
}
