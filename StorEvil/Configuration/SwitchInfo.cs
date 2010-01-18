using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace StorEvil
{
    public class SwitchInfo<T>
    {
        private Action<T, string[]> _action;

        public SwitchInfo(string[] switches)
        {
            Switches = switches;
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

        public string[] Switches { get; private set; }

        public bool Matches(string s)
        {
            return Switches.Any(x => x == s);
        }

        public void Execute(T settings, string[] additionalParams)
        {
            _action(settings, additionalParams);
        }

        public void SetsField(Expression<Func<T, bool>> func)
        {
            SetFieldFromLambda(func, ignored => true);
        }

        public void SetsField(Expression<Func<T, string>> func)
        {
            SetFieldFromLambda(func, values => values[0]);
        }

        public void SetsField(Expression<Func<T, IEnumerable<string>>> func)
        {
            SetFieldFromLambda(func, values => values.Cast<string>().ToArray());
        }

        private void SetFieldFromLambda(Expression expression, Func<string[], object> switchToParam)
        {
            var member = ((MemberExpression) ((LambdaExpression) expression).Body).Member;

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
        }
    }
}