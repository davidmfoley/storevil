using System;
using System.Reflection;
using NUnit.Framework;
using StorEvil.Context;

namespace StorEvil
{
    public enum MatchingTest
    {
        Foo,
        Bar,
        FooBar
    }

    [TestFixture]
    public class MethodNameMatcherTests
    {
        private NameMatch FoundMatch;

        [SetUp]
        public void SetupContext()
        {
            var matcher = new MethodNameMatcher(GetMethod<EnumTestContext>("Test_parsing_of_enumValue"));
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

        private static MethodInfo GetMethod<T>(string name)
        {
            return typeof(T).GetMethod(name);
        }
    }

    public class EnumTestContext
    {
        MatchingTest? EnumValue = null;
        public void Test_parsing_of_enumValue(MatchingTest enumValue)
        {
            EnumValue = enumValue;
        }
    }
}