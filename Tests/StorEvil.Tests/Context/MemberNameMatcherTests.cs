using System.Reflection;
using NUnit.Framework;
using StorEvil.Context.Matchers;
using StorEvil.Context.Matches;
using StorEvil.Utility;

namespace StorEvil.Context
{
    public enum MatchingTest
    {
        Foo,
        Bar,
        FooBar
    }

    public class MethodMatcherTestHelper
    {
        public static MethodInfo GetMethod<T>(string name)
        {
            return typeof (T).GetMethod(name);
        }

        public static MethodNameMatcher GetMatcher<T>(string methodName)
        {
            return new MethodNameMatcher(GetMethod<T>(methodName));
        }
    }

    [TestFixture]
    public class Enum_values
    {
        private NameMatch FoundMatch;

        [SetUp]
        public void SetupContext()
        {
            var matcher = MethodMatcherTestHelper.GetMatcher<EnumTestContext>("Test_parsing_of_enumValue");

            FoundMatch = matcher.GetMatch("Test parsing of foo");
        }

        [Test]
        public void Should_be_exact_match()
        {
            FoundMatch.ShouldBeOfType<ExactMatch>();
        }

        [Test]
        public void Should_parse_param()
        {
            FoundMatch.ParamValues["enumValue"].ShouldEqual(MatchingTest.Foo);
        }

        public class EnumTestContext
        {
            private MatchingTest? EnumValue;

            public void Test_parsing_of_enumValue(MatchingTest enumValue)
            {
                EnumValue = enumValue;
            }
        }
    }

    public class Parsing_tables
    {
        [SetUp]
        public void SetupContext()
        {
            var matcher = MethodMatcherTestHelper.GetMatcher<Table_matching_test_context>("method_takes_a_table");
            Match = matcher.GetMatch("Method takes a table\r\n|1|2|\r\n|3|4|\r\n|5|6|");
        }

        protected NameMatch Match { get; set; }

        [Test]
        public void should_parse_table()
        {
            Match.ParamValues["values"].ShouldEqual("|1|2|\r\n|3|4|\r\n|5|6|");
        }

        private class Table_matching_test_context
        {
            public void method_takes_a_table(string[][] values)
            {
            }
        }
    }
}