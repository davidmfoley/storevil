using System;
using StorEvil.Interpreter.ParameterConverters;

namespace StorEvil.Utility
{
    public static class ReflectionExtensions
    {
        public static Action<object, object> GetSetter(this Type destinationType, string memberName)
        {
            var propertyInfo = destinationType.GetProperty(memberName);
            if (null != propertyInfo)
                return (o, v) => propertyInfo.SetValue(o, v, null);

            var fieldInfo = destinationType.GetField(memberName);
            if (null != fieldInfo)
                return (o, v) => fieldInfo.SetValue(o, v);

            throw new UnknownFieldException(destinationType, memberName);
        }

        public static Type GetMemberType(this Type destinationType, string memberName)
        {
            var propertyInfo = destinationType.GetProperty(memberName);
            if (null != propertyInfo)
                return propertyInfo.PropertyType;

            var fieldInfo = destinationType.GetField(memberName);

            return null != fieldInfo ? fieldInfo.FieldType : null;
        }

        public static void ReflectionSet(this object obj, string propertyOrField, object value)
        {
            var setter = obj.GetType().GetSetter(propertyOrField);
            setter(obj, value);
        }

        public static object ReflectionGet(this object obj, string propertyOrField)
        {
            var getter = obj.GetType().GetGetter(propertyOrField);
            return getter(obj);
        }
        public static Func<object, object> GetGetter(this Type destinationType, string memberName)
        {
            var propertyInfo = destinationType.GetProperty(memberName);
            if (null != propertyInfo)
                return (o) => propertyInfo.GetValue(o, null);

            var fieldInfo = destinationType.GetField(memberName);
            if (null != fieldInfo)
                return fieldInfo.GetValue;

            throw new UnknownFieldException(destinationType, memberName);
        }
    }
}