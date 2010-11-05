using System;
using System.Runtime.Serialization;

namespace StorEvil.Interpreter.ParameterConverters
{
    [Serializable]
    public class UnknownFieldException : Exception
    {
        public string DestinationTypeName { get; set; }
        public string MemberName { get; set; }

         public UnknownFieldException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
         {
             DestinationTypeName = serializationInfo.GetString("DestinationTypeName");
             MemberName = serializationInfo.GetString("MemberName");
         }
         public override void GetObjectData(SerializationInfo info, StreamingContext context)
         {
             base.GetObjectData(info, context);
             info.AddValue("DestinationTypeName", DestinationTypeName);
             info.AddValue("MemberName", MemberName);
         }

        public UnknownFieldException(Type destinationType, string memberName)
        {
            DestinationTypeName = destinationType.Name;
            MemberName = memberName;
        }
        public override string Message
        {
            get
            {
                return "Unknown field:" + MemberName + " on type: " + DestinationTypeName;
            }
        }
    }
}