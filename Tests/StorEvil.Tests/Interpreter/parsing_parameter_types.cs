using System;
using NUnit.Framework;
using StorEvil.Interpreter.ParameterConverters;
using StorEvil.Utility;

namespace StorEvil.Interpreter.ParameterConverter_Specs
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
            var result = Converter.Convert("$42.00", typeof (decimal));
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

        [Test]
        public void should_convert_table_to_string_array()
        {
            var result = Converter.Convert("|a1|a2|\r\n|b1|b2|", typeof (string[][])) as string[][];
            result.ShouldNotBeNull();
            result[0][0].ShouldBe("a1");
            result[0][1].ShouldBe("a2");
            result[1][0].ShouldBe("b1");
            result[1][1].ShouldBe("b2");
        }

        [Test]
        public void should_convert_typed_table_to_an_array_of_types()
        {
            var result = Converter.Convert("|StringField|IntField|\r\n|a|1|\r\n|b|2|", typeof(TypedArrayTestType[])) as TypedArrayTestType[];
            result.ShouldNotBeNull();
            result.Length.ShouldEqual(2);
            result[0].StringField.ShouldBe("a");
            result[0].IntField.ShouldBe(1);

            result[1].StringField.ShouldBe("b");
            result[1].IntField.ShouldBe(2);
          
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
            var result = Converter.Convert("foo", typeof (TestValues));
            result.ShouldBeOfType<TestValues>();
            result.ShouldEqual(TestValues.Foo);
        }
    }

    public class TypedArrayTestType
    {
        public string StringField { get; set; }
        public int IntField { get; set; }
    }
}