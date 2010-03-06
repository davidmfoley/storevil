using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Nunit
{
    public class TestContextSet : IEnumerable<TestContextField>
    {
        public void Add(TestContextField field)
        {
            if (_contexts.Any(x => x.Name == field.Name))
                return;

            _contexts.Add(field);
        }

        public void AddRange(IEnumerable<TestContextField> contexts)
        {
            foreach (var field in contexts)
            {
                Add(field);
            }
        }

        private readonly List<TestContextField> _contexts = new List<TestContextField>();

        public IEnumerator<TestContextField> GetEnumerator()
        {
            return _contexts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _contexts.GetEnumerator();
        }
    }
}