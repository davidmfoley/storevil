using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace StorEvil.Configuration
{
    public class SwitchInfo<T>
    {
        private Action<T, string[]> _action;
        private readonly Func<string[], object> BoolParamTransform = (ignored => true);
        private readonly Func<string[], object> StringParamTransform = (values => values[0]);
        private readonly Func<string[], object> CollectionParamTransform = (values => values.ToArray());

        public SwitchInfo(string[] names)
        {
            Names = names;
        }

        public string[] Names { get; private set; }

        public bool Matches(string s)
        {
            return Names.Any(x => x == s);
        }

        public void Execute(T settings, string[] additionalParams)
        {
            _action(settings, additionalParams);
        }

        public SwitchInfo<T> SetsField(Expression<Func<T, bool>> func)
        {
            SetFieldFromLambda(func, BoolParamTransform);
            return this;
        }

        public SwitchInfo<T> SetsField(Expression<Func<T, string>> func)
        {
            TakesAString = true;
            SetFieldFromLambda(func, StringParamTransform);
            return this;
        }

        public SwitchInfo<T> SetsField(Expression<Func<T, IEnumerable<string>>> func)
        {
            TakesAList = true;
            SetFieldFromLambda(func, CollectionParamTransform);
            return this;
        }

        public SwitchInfo<T> SetsEnumField<enumT>(Expression<Func<T, enumT>> func)
        {
            
            var t = typeof (enumT);
            TakesAnEnum = true;
            EnumType = t;

            if (t.IsEnum)
            {
                SetFieldFromLambda(func, values =>
                                             {
                                                 var foo = Enum.Parse(t, values[0], true);
                                                 return foo;
                                             });
            }
            else
            {
                throw new ApplicationException("Can't parse switch with type:" + t.Name);
            }

            return this;
        }

        public SwitchInfo<T> SetsField(MemberInfo member)
        {
            var switchToParam = GetSwitchToParamTransformation(member);

            SetFieldFromMemberInfo(member, switchToParam);
            return this;    
        }

        public SwitchInfo<T> WithDescription(string description)
        {
            Description = description;

            return this;
        }

        private string _description;
        public bool TakesAString;
        public bool TakesAList;
        public bool TakesAnEnum;
        public Type EnumType;

        public string Description
        {
            get { return _description ?? ""; }
            set { _description = value; }
        }

        private Func<string[], object> GetSwitchToParamTransformation(MemberInfo member)
        {
            var propOrFieldType = GetMemberType(member);
            if (propOrFieldType == typeof (bool))
                return BoolParamTransform;
            if (propOrFieldType == typeof (string))
                return StringParamTransform;
            if (typeof (IEnumerable).IsAssignableFrom(propOrFieldType))
                return CollectionParamTransform;

            ThrowBadExpressionException();
            return null;
        }

        private Type GetMemberType(MemberInfo member)
        {
            if (member is PropertyInfo)
                return ((PropertyInfo) member).PropertyType;

            if (member is FieldInfo)
                return ((FieldInfo) member).FieldType;

            ThrowBadExpressionException();
            return null;
        }

        private void SetFieldFromLambda(Expression expression, Func<string[], object> switchToParam)
        {
            var lambdaExpression = expression as LambdaExpression;

            var memberExpression = lambdaExpression.Body as MemberExpression;

            SetFieldFromMemberExpression(memberExpression, switchToParam);
        }

        private void SetFieldFromMemberExpression(MemberExpression memberExpression,
                                                  Func<string[], object> switchToParam)
        {
            if (memberExpression == null)
                ThrowBadExpressionException();

            var member = memberExpression.Member;
            SetFieldFromMemberInfo(member, switchToParam);
        }

        private void SetFieldFromMemberInfo(MemberInfo member, Func<string[], object> switchToParam)
        {
            var type = member.DeclaringType;

            if (member.MemberType == MemberTypes.Property)
            {
                var propInfo = type.GetProperty(member.Name);
                _action = (settings, values) => propInfo.SetValue(settings, switchToParam(values), new object[0]);
            }
            else if (member.MemberType == MemberTypes.Field)
            {
                var fieldInfo = type.GetField(member.Name);
                _action = (settings, values) => fieldInfo.SetValue(settings, switchToParam(values));
            }
            else
            {
                ThrowBadExpressionException();
            }
        }

        public void WithAction(Action<T> action)
        {
            _action = (settings, ignored2) => action(settings);
        }

        public void WithSingleParamAction(Action<T, string> action)
        {
            _action = (settings, parameters) => action(settings, parameters.FirstOrDefault());
        }

        public void WithMultiParamAction(Action<T, string[]> action)
        {
            _action = action;
        }

        private void ThrowBadExpressionException()
        {
            throw new ArgumentException("Only simple property and field expressions are supported.");
        }
    }
}