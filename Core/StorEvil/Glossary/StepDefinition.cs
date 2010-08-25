using System;
using System.Collections.Generic;
using StorEvil.Context.Matchers;

namespace StorEvil.Glossary
{
    public class StepDefinition
    {
        public StepDefinition()
        {
            Children = new StepDefinition[0];
        }

        public Type DeclaringType { get; set; }

        public IMemberMatcher Matcher { get; set; }

        public IEnumerable<StepDefinition> Children { get; set; }
    }
}