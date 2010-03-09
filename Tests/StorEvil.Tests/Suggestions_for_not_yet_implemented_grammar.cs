using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class Suggestions_for_not_yet_implemented_grammar
    {
        private readonly ImplementationHelper Helper = new ImplementationHelper(); 

        [Test]
        public void Should_suggest_box_car_name_for_simple_case()
        {
            var result = Helper.Suggest("an unimplemented method");
            ShouldMatch(result, "an_unimplemented_method");
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
            var result = Helper.Suggest("a user named \"dave\"");
            ShouldMatch(result, "a_user_named_arg0", "string", "arg0");
        }

        [Test]
        public void Should_suggest_string_array_array_param_for_table_of_data()
        {
            var result = Helper.Suggest("Given the following\r\n|1|2|\r\n|3|4|");
            ShouldMatch(result, "Given_the_following", "string[][]", "tableData");
        }

        private void ShouldMatch(string code,  string name, params string[] additionalStrings)
        {
            Regex r = new Regex(name +"\\(.*" + string.Join(".*", additionalStrings) + ".*\\).*{.*}" , RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(code), "code did not match: \r\n" + code);
        }
    }

    
}