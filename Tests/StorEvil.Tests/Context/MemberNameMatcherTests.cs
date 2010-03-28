using System.Reflection;
using NUnit.Framework;
using StorEvil.Context.Matchers;
using StorEvil.Context.Matches;
using StorEvil.Utility;

namespace StorEvil.Context.Matching_method_names_with_reflection
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
            return typeof(T).GetMethod(name);
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

    public class Matching_words_with_puncutation
    {
        protected NameMatch Match { get; set; }

        [SetUp]
        public void SetupContext()
        {
            var matcher = MethodMatcherTestHelper.GetMatcher<Punctuation_test_context>("its_good_to_handle_apostrophes");
            Match = matcher.GetMatch("It's good to handle apostrophes");
        }

        [Test]
        public void should_match()
        {
            Match.ShouldNotBeNull();
        }

        [Test]
        public void should_be_exact_match()
        {
            Match.ShouldBeOfType<ExactMatch>();
        }

        private class Punctuation_test_context
        {
            public void its_good_to_handle_apostrophes()
            {
            }
        }
    }

    public class Parsing_typed_tables
    {
        private object ParamValue;

        [SetUp]
        public void SetupContext()
        {
            var matcher = MethodMatcherTestHelper.GetMatcher<Typed_table_test_context>("method_takes_a_typed_array");
            Match = matcher.GetMatch("Method takes a typed array\r\n|IntParam|StringParam|\r\n|1|2|\r\n|3|4|\r\n|5|6|");
            ParamValue = Match.ParamValues["values"];
        }

        protected NameMatch Match { get; set; }

        [Test]
        public void should_match_with_typed_array_parameter()
        {
            ParamValue.ShouldEqual("|IntParam|StringParam|\r\n|1|2|\r\n|3|4|\r\n|5|6|");
        }

        private class Typed_table_test_context
        {
            public void method_takes_a_typed_array(TestType[] values)
            {
            }
        }

        internal class TestType
        {
            public int IntParam { get; set; }
            public string StringParam { get; set; }
        }
    }

    public class Partial_matches
    {
        [SetUp]
        public void SetupContext()
        {         
            var matcher = MethodMatcherTestHelper.GetMatcher<Partial_match_test_context>("when_a_user_named");
            Match = matcher.GetMatch("when a user named foo does something");
        }

        protected NameMatch Match { get; set; }

        [Test]
        public void returns_partial_match()
        {
            Match.ShouldBeOfType<PartialMatch>();
        }

        [Test]
        public void should_set_name()
        {
            Match.ParamValues["name"].ShouldEqual("foo");
        }

        private class Partial_match_test_context
        {
            public Test_user_context when_a_user_named(string name)
            {
                return new Test_user_context();
            }
        }

        private class Test_user_context
        {
            public void does_something()
            {
            }
        }
    }
}