using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using StorEvil.Interpreter.ParameterConverters;

namespace StorEvil.Utility
{
    /// <summary>
    /// An (obviously) incomplete set of extension methods that encapsulate assertions
    /// that can be invoked in spec code.
    /// 
    /// for example:
    /// blah should equal 12
    /// 
    /// will translate (if mapped) to:
    /// _context.Blah().ShouldEqual("12");
    /// 
    /// note that the type conversion is handled here (also incomplete)
    /// need a better way to do this
    /// </summary>
    public static class TestExtensionMethods
    {
        private static readonly ParameterConverter _parameterConverter = new ParameterConverter();
        public static void ShouldEqual(this object actual, object expected)
        {
            if (actual == null && expected == null)
                return;

            if (actual == expected)
                return;

            if ((actual ?? "").ToString().ToLower() == (expected ?? "").ToString().ToLower())
                return;

            if (actual == null)
                Assert.Fail();

            object expectedConverted = ConvertToType(actual.GetType(), expected);

            // depending on actual type, parse expected
            Assert.AreEqual(expectedConverted, actual);
        }

        private static object ConvertToType(Type type, object expected)
        {
            //TODO: break this out or use TypeConverters
            return _parameterConverter.Convert(expected.ToString(), type);
        }

        public static void ShouldBe(this object actual, object expected)
        {
            ShouldEqual(actual, expected);
        }

        public static void ShouldBeNull(this object actual)
        {
            Assert.IsNull(actual);
        }

        public static void ShouldNotBeNull(this object actual)
        {
            Assert.IsNotNull(actual);
        }

        public static void ShouldBeOfType<T>(this object actual)
        {
            Assert.IsInstanceOfType(typeof (T), actual);
        }

        public static void ElementsShouldEqual<T>(this IEnumerable<T> collection, params T[] expected)
        {
            Assert.AreEqual(expected.Length, collection.Count());

            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(collection.ElementAt(i), expected[i]);
        }

        public static void ShouldContain(this string actual, string expectedContent)
        {
            actual.IndexOf(expectedContent).ShouldBeGreaterThan(-1);
        }

        public static void ShouldBeGreaterThan<T>(this T actual, T expected) where T : IComparable
        {
            Assert.That(actual.CompareTo(expected) > 0);
        }

        public static void ShouldBeLessThan<T>(this T actual, T expected) where T : IComparable
        {
            Assert.That(actual.CompareTo(expected) < 0);
        }

        public static void ShouldBeLessThanOrEqualTo<T>(this T actual, T expected) where T : IComparable
        {
            Assert.That(actual.CompareTo(expected) <= 0);
        }

        public static void ShouldBeGreaterThanOrEqualTo<T>(this T actual, T expected) where T : IComparable
        {
            Assert.That(actual.CompareTo(expected) >= 0);
        }

        public static void ShouldMatch(this string actual, string regex)
        {
            bool isMatch = Regex.IsMatch(actual, regex, RegexOptions.Singleline);
            Assert.IsTrue(isMatch, "Expected '" + actual + "' to match pattern: '" + regex + "'");
        }
    }

   
}