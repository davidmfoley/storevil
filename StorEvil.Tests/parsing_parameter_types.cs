using NUnit.Framework;
using StorEvil.Core;

namespace StorEvil
{
    [TestFixture]
    public class parsing_parameter_types
    {
        private ParameterConverter Converter;

        [SetUp]
        public void SetupContext()
        {
            Converter = new ParameterConverter();
        }

        [Test]
        public void should_convert_int()
        {
            var result = Converter.Convert("42", typeof (int));
            result.ShouldBeOfType<int>();
            result.ShouldEqual(42);
        }

        [Test]
        public void should_convert_currency()
        {
            var result = Converter.Convert("$42.00", typeof(decimal));
            result.ShouldBeOfType<decimal>();
            result.ShouldEqual(42m);
        }

        [Test]
        public void should_convert_currency_without_decimal_point()
        {
            var result = Converter.Convert("$42", typeof (decimal));
            result.ShouldBeOfType<decimal>();
            result.ShouldEqual(42m);
        }

        private enum TestValues
        {
            Foo,
            Bar,
            FooBar
        }

        [Test]
        public void should_convert_enum_to_correct_value()
        {
            var result = Converter.Convert("foo", typeof(TestValues));
            result.ShouldBeOfType<TestValues>();
            result.ShouldEqual(TestValues.Foo);
        }
    }

    
}