
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using StorEvil.Context.Matchers;
using StorEvil.Context.Matches;
using StorEvil.Utility;
using System.Linq;

namespace StorEvil.Context.Matching_method_names_with_reflection
{
    public enum MatchingTest
    {
        Foo,
        Bar,
        FoobarBaz,
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
        private MethodNameMatcher Matcher;

        [SetUp]
        public void SetupContext()
        {
            Matcher = MethodMatcherTestHelper.GetMatcher<EnumTestContext>("Test_parsing_of_enumValue");
        }

        private NameMatch GetMatch(string s)
        {
            return Matcher.GetMatches(s).FirstOrDefault();
        }
        [Test]
        public void Should_be_exact_match()
        {


            FoundMatch = GetMatch("Test parsing of foo");
            FoundMatch.ShouldBeOfType<ExactMatch>();
        }

        [Test]
        public void Should_parse_param()
        {
            FoundMatch = GetMatch("Test parsing of foo");
            FoundMatch.ParamValues["enumValue"].ShouldEqual(MatchingTest.Foo);
        }

        [Test]
        public void Should_match_multiple_words_in_enum_name()
        {
            FoundMatch = GetMatch("Test parsing of foobar baz");
            FoundMatch.ParamValues["enumValue"].ShouldEqual(MatchingTest.FoobarBaz);
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
            Match = matcher.GetMatches("Method takes a table\r\n|1|2|\r\n|3|4|\r\n|5|6|").FirstOrDefault();
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

    public class Matching_a_numeric_int_parameter
    {
        private MethodNameMatcher Matcher;

        [SetUp]
        public void SetupContext()
        {
            Matcher = MethodMatcherTestHelper.GetMatcher<Numeric_matching_test_context>("method_takes_an_int");            
        }

        [Test]
        public void should_match_a_negative_int()
        {
            var matches = Matcher.GetMatches("method takes an int -42");
            matches.Count().ShouldBe(1);
            matches.First().ParamValues.First().Value.ShouldEqual(-42);
        }

        public class Numeric_matching_test_context
        {
            public void method_takes_an_int(int val)
            {
                
            }
        }
    }

    public class Matching_words_with_punctuation
    {
        private IEnumerable<NameMatch> Matches;

        [SetUp]
        public void SetupContext()
        {
            var matcher = MethodMatcherTestHelper.GetMatcher<Punctuation_test_context>("its_good_to_handle_apostrophes");
            Matches = matcher.GetMatches("It's good to handle apostrophes");
        }

        [Test]
        public void should_match()
        {
            Matches.ShouldNotBeNull();
            Matches.Count().ShouldBe(1);
        }

        [Test]
        public void should_be_exact_match()
        {
            Matches.First().ShouldBeOfType<ExactMatch>();
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
            Match = matcher.GetMatches("Method takes a typed array\r\n|IntParam|StringParam|\r\n|1|2|\r\n|3|4|\r\n|5|6|").FirstOrDefault();
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
            Match = matcher.GetMatches("when a user named foo does something").FirstOrDefault();
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

    [TestFixture]
    public class Multiple_word_parameters
    {
        private MethodNameMatcher Matcher;
        private IEnumerable<NameMatch> FoundMatches;
        private ExactMatch Exact;
        private PartialMatch Partial;

        [SetUp]
        public void SetupContext()
        {
            Matcher = MethodMatcherTestHelper.GetMatcher<MultiWordTestContext>("Foo");
            FoundMatches =  Matcher.GetMatches("Foo Bar Baz");
            Partial = FoundMatches.OfType<PartialMatch>().FirstOrDefault();
            Exact = FoundMatches.OfType<ExactMatch>().FirstOrDefault();
        
        }

        [Test]
        public void Should_have_multiple_matches()
        {
           FoundMatches.Count().ShouldBe(2);
        }

        [Test]
        public void Should_have_partial()
        {
            Partial.ShouldNotBeNull();
        }

        [Test]
        public void Should_have_exact()
        {
            Exact.ShouldNotBeNull();
        }

        [Test]
        public void Should_have_one_match_for_each_combination()
        {
            FoundMatches.Count().ShouldBe(2);
        }

        [Test]
        public void should_have_param_values_for_partial()
        {
            Partial.ParamValues["param"].ShouldBe("Bar");
        }

        [Test]
        public void should_have_param_values_for_exact()
        {
            Exact.ParamValues["param"].ShouldBe("Bar Baz");
        }
    }

    public class MultiWordTestContext
    {
        public void Foo([MultipleWords] string param)
        {
        }
    }

}