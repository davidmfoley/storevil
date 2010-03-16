using System.Collections.Generic;
using NUnit.Framework;
using StorEvil.Interpreter.ParameterConverters;
using StorEvil.Utility;

namespace StorEvil.Interpreter.ParameterConverter_Specs
{
    [TestFixture]
    public class parsing_parameter_types
    {
        protected ParameterConverter Converter = new ParameterConverter();

      
    }
    [TestFixture]
    public class simple_conversions : parsing_parameter_types
    {

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
        public void should_convert_enum_to_correct_value()
        {
            var result = Converter.Convert("foo", typeof(TestValues));
            result.ShouldBeOfType<TestValues>();
            result.ShouldEqual(TestValues.Foo);
        }

        private enum TestValues
        {
            Foo,
            Bar,
            FooBar
        }

    }
    [TestFixture]
    public class converting_tables_of_data : parsing_parameter_types
    {
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
            var result =
                Converter.Convert("|StringField|IntField|\r\n|a|1|\r\n|b|2|", typeof (TypedArrayTestType[])) as
                TypedArrayTestType[];
            result.ShouldNotBeNull();
            result.Length.ShouldEqual(2);
            result[0].StringField.ShouldBe("a");
            result[0].IntField.ShouldBe(1);

            result[1].StringField.ShouldBe("b");
            result[1].IntField.ShouldBe(2);
        }
    }

    [TestFixture]
    public class converting_comma_separated_arrays : parsing_parameter_types
    {
        [Test]
        public void should_convert_comma_separated_values_to_an_array_of_ints()
        {
            var result = Converter.Convert("1,2,3,4", typeof (int[])) as int[];
            result.ShouldNotBeNull();
            result.Length.ShouldEqual(4);
            result[0].ShouldEqual(1);
            result[1].ShouldEqual(2);
            result[2].ShouldEqual(3);
            result[3].ShouldEqual(4);
        }

        [Test]
        public void should_convert_empty_value_to_zero_element_array()
        {
            var result = Converter.Convert("", typeof(int[])) as int[];
            result.ShouldNotBeNull();
            result.Length.ShouldEqual(0);
            
        }
    }

    [TestFixture]
    public class converting_a_table_to_a_user_type :parsing_parameter_types
    {
        [Test]
        public void should_populate_fields_of_user_type_from_table()
        {
            var result = Converter.Convert("|IntField|42|\r\n|StringProp|foobar|", typeof(ConversionTestType)) as ConversionTestType;
            result.ShouldNotBeNull();
            result.IntField.ShouldEqual(42);
            result.StringProp.ShouldEqual("foobar");
        }

        [Test]
        public void should_populate_dictionary_from_table()
        {
            var result = Converter.Convert("|IntField|42|\r\n|StringProp|foobar|", typeof(Dictionary<string, string>)) as Dictionary<string, string>;
            result.ShouldNotBeNull();
            result["IntField"].ShouldEqual("42");
            result["StringProp"].ShouldEqual("foobar");

        }

        class ConversionTestType
        {
            public int IntField;
            public string StringProp { get; set; }
        }
    }

    public class TypedArrayTestType
    {
        public string StringField { get; set; }
        public int IntField { get; set; }
    }
}