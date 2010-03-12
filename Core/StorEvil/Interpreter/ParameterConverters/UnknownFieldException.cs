using System;

namespace StorEvil.Interpreter.ParameterConverters
{
    public class UnknownFieldException : Exception
    {
        public Type DestinationType { get; set; }
        public string MemberName { get; set; }

        public UnknownFieldException(Type destinationType, string memberName)
        {
            DestinationType = destinationType;
            MemberName = memberName;
        }
        public override string Message
        {
            get
            {
                return "Unknown field:" + MemberName + " on type: " + DestinationType.Name;
            }
        }
    }
}