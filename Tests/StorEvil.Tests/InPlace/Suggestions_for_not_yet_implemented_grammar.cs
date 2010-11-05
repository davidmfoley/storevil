using System.Text.RegularExpressions;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.InPlace;
using StorEvil.Utility;


namespace StorEvil.InPlace
{
    [TestFixture]
    public class Suggestions_for_not_yet_implemented_grammar
    {
        private readonly ImplementationHelper Helper = new ImplementationHelper();

        [Test]
        public void Includes_comment_with_original_text()
        {
            var result = Helper.Suggest("an unimplemented method");
            result.ShouldContain("// an unimplemented method");
        }

        [Test]
        public void For_scenario_outline_uses_bracketed_names_for_params()
        {
            var result = Helper.Suggest("a step with <foo> and <bar> as parameters");
            ShouldMatch(result, "a_step_with_foo_and_bar_as_parameters", "string", "foo", "string", "bar");
        }

        [Test]
        public void Should_suggest_box_car_name_for_simple_case()
        {
            var result = Helper.Suggest("an unimplemented method");
            ShouldMatch(result, "an_unimplemented_method");
        }

        [Test]
        public void Should_use_last_significant_word_instead_of_and()
        {
            Helper.Suggest("then something ");
            var result = Helper.Suggest("and something else");
            ShouldMatch(result, "then_something_else");
        }

        [Test]
        public void Should_suggest_int_param()
        {
            var result = Helper.Suggest("a user with 5 beers");
            ShouldMatch(result, "a_user_with_arg0_beers", "int", "arg0");
        }

        [Test]
        public void Should_suggest_string_param()
        {
            var result = Helper.Suggest("a user with \"dave\" as a name");
            ShouldMatch(result, "a_user_with_arg0_as_a_name", "string", "arg0");
        }

        [Test]
        public void Should_not_include_arg_in_name_if_trailing()
        {
            var result = Helper.Suggest("a user named \"dave\"");
            ShouldMatch(result, "a_user_named", "string", "arg0");
        }

        [Test]
        public void Should_suggest_string_array_array_param_for_table_of_data()
        {
            var result = Helper.Suggest("Given the following\r\n|1|2|\r\n|3|4|");
            ShouldMatch(result, "Given_the_following", "string\\[\\]\\[\\]", "tableData");
        }

        [Test]
        public void Should_suggest_string_array_param_for_comma_separated_data()
        {
            var result = Helper.Suggest("Given the following:1,2,3,4,5");
            ShouldMatch(result, "Given_the_following", "string\\[\\]", "arg0");
        }

        [Test]
        public void Should_handle_punctuation()
        {
            var result = Helper.Suggest("It's a test of embedded apostrophes");
            ShouldMatch(result, "Its_a_test_of_embedded_apostrophes");
        }

        private void ShouldMatch(string code, string name, params string[] additionalStrings)
        {
            Regex r = new Regex(name + "\\(.*" + string.Join(".*", additionalStrings) + ".*\\).*{.*}",
                                RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(code), "code did not match: \r\n" + code);
        }
    }
}