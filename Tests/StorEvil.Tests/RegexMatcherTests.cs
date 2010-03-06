using System.Linq;
using NUnit.Framework;
using StorEvil.Context;
using StorEvil.Context.Matchers;
using StorEvil.Context.Matches;

namespace StorEvil.RegExMatcherTests
{
    [TestFixture]
    public class when_regex_matches_whole_line
    {
        private NameMatch TestMatch;
        private string TestText;

        [SetUp]
        public void SetupContext()
        {
            var matcher = new RegexMatcher("^Some fake method name", null);
            TestText = "Some fake method name";
            TestMatch = matcher.GetMatch(TestText);
        }
        [Test]
        public void should_return_an_exact_match()
        {
            TestMatch.ShouldBeOfType<ExactMatch>();
        }

        [Test]
        public void returned_match_should_span_entire_line()
        {
            TestMatch.MatchedText.ShouldEqual(TestText);
        }
    }

    [TestFixture]
    public class when_regex_does_not_match
    {
        private NameMatch TestMatch;

        [SetUp]
        public void SetupContext()
        {
            var matcher = new RegexMatcher("^Some fake method name", null);
            TestMatch = matcher.GetMatch("this does not match");
        }

        [Test]
        public void returns_null()
        {
            TestMatch.ShouldBeNull();
        }
    }

    [TestFixture]
    public class when_regex_is_a_partial_match 
    {
        private NameMatch TestMatch;
        private string TestText;

        [SetUp]
        public void SetupContext()
        {
            var matcher = new RegexMatcher("^Some fake method name", typeof(RegexTestContext).GetMethod("MethodWithReturnValue"));
            TestText = "Some fake method name and some other text";
            TestMatch = matcher.GetMatch(TestText);
        }

        [Test]
        public void returns_Partial_Match()
        {
            TestMatch.ShouldBeOfType<PartialMatch>();
        }

        [Test]
        public void sets_matched_text()
        {
            TestMatch.MatchedText.ShouldEqual("Some fake method name");
        }

        [Test]
        public void sets_remaining_text()
        {
            ((PartialMatch)TestMatch).RemainingText.ShouldEqual("and some other text");
        }
    }

    [TestFixture]
    public class partial_matching_on_different_types_of_members
    {
      
        [Test]
        public void No_partial_matching_on_void_methods()
        {
            var Matcher = new RegexMatcher("^Some fake method name", typeof(RegexTestContext).GetMethod("VoidMethod"));
            Matcher.GetMatch("Some fake method name blah blah blah").ShouldBeNull();
        }

        [Test]
        public void can_partially_match_on_properties()
        {
            var Matcher = new RegexMatcher("^Foo Bar", typeof(RegexTestContext).GetProperty("SomeProperty"));
            Matcher.GetMatch("Foo Bar Baz").ShouldBeOfType<PartialMatch>();
        }

        [Test]
        public void can_partially_match_on_fields()
        {
            var Matcher = new RegexMatcher("^Foo Bar", typeof(RegexTestContext).GetField("SomeField"));
            Matcher.GetMatch("Foo Bar Baz").ShouldBeOfType<PartialMatch>();
        }
    }

    [TestFixture]
    public class regex_parameter_matching
    {
        private NameMatch TestMatch;
        private string TestText;

        [SetUp]
        public void SetupContext()
        {
            var matcher = new RegexMatcher("^Some fake method name (.+)", typeof(RegexTestContext).GetMethod("MethodWithParams"));
            TestText = "Some fake method name and some other text";
            TestMatch = matcher.GetMatch(TestText);
        }

        [Test]
        public void should_have_one_parameter()
        {
            TestMatch.ParamValues.Keys.Count.ShouldEqual(1);
        }

        [Test]
        public void should_use_parameter_name_to_method_as_key()
        {
            TestMatch.ParamValues.Keys.First().ShouldEqual("parameterName");
        }

        [Test]
        public void should_use_captured_for_param_value()
        {
            TestMatch.ParamValues.Values.First().ShouldEqual("and some other text");
        }
    }

    public class RegexTestContext
    {
        public int MethodWithReturnValue()
        {
            return 0;
        }

        public void VoidMethod () {}

        public string SomeProperty { get { return "foo"; } }

        public void MethodWithParams(string parameterName) {}
    }
}