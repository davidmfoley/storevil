using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StorEvil.Interpreter.ParameterConverters;

namespace StorEvil.Assertions
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
            Assert.IsTrue(actual is T, "Expected type " +  typeof(T).Name + " but got " + (actual == null ? "null" : actual.GetType().Name));
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

        public static void ShouldNotMatch(this string actual, string regex)
        {
            bool isMatch = Regex.IsMatch(actual, regex, RegexOptions.Singleline);
            Assert.IsFalse(isMatch, "Expected '" + actual + "' not to match pattern: '" + regex + "'");
        }

        public static void ShouldContain<T>(this IEnumerable<T> collection, T expected)
        {
            collection.Contains(expected).ShouldBe(true);
        }
    }

    static class Assert
    {
        public static void IsTrue(bool isMatch, string message)
        {
            if (!isMatch)
                throw new AssertionException(message);
        }

        public static void IsFalse(bool isMatch, string message)
        {
            IsTrue(!isMatch, message);
        }

        public static void That(bool b)
        {
            IsTrue(b, "");
        }

        public static void AreEqual(object a, object b)
        {
            IsTrue(a.Equals(b), "");
        }

        public static void IsNull(object actual)
        {
            IsTrue(actual == null, "Excpected null but got " + actual);
        }

        public static void IsNotNull(object actual)
        {
            IsTrue(actual != null, "Excpected not null");        
        }

        public static void Fail()
        {
            throw new AssertionException("Failed");
        }
    }

    [Serializable]
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message)
        {
            
        }
    }
}