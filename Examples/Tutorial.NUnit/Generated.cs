using NUnit.Framework;
using System;
using StorEvil;



namespace TestNamespace {
    using Tutorial;
using StorEvil.Utility;
    

    [TestFixture]
    
    public class BasicGrammar_Specs {
#line 1 "BasicGrammar.feature"
#line hidden
        private StorEvil.Interpreter.ParameterConverters.ParameterConverter ParameterConverter = new StorEvil.Interpreter.ParameterConverters.ParameterConverter();
        [TestFixtureSetUp]
        public void WriteStoryToConsole() {
            Console.WriteLine(@"Learning StorEvil

Basic Grammar
 ");
        }
        
        [Test] public void basic_matching_of_plaintext_line(){
            var contextBasicGrammarContext = new Tutorial.BasicGrammarContext();
#line 5
           System.Console.WriteLine(@"When my name is StorEvil");
            contextBasicGrammarContext.When_my_name_is(@"StorEvil");
        }
        [Test] public void a_basic_assertion(){
            var contextBasicGrammarContext = new Tutorial.BasicGrammarContext();
#line 8
           System.Console.WriteLine(@"When my name is StorEvil");
            contextBasicGrammarContext.When_my_name_is(@"StorEvil");
#line 9
           System.Console.WriteLine(@"I should eat all other BDD frameworks");
            contextBasicGrammarContext.I_should_eat_all_other_BDD_frameworks();
        }
        [Test] public void assertion_using_the_ShouldXXX_methods(){
            var contextBasicGrammarContext = new Tutorial.BasicGrammarContext();
#line 12
           System.Console.WriteLine(@"When my name is StorEvil");
            contextBasicGrammarContext.When_my_name_is(@"StorEvil");
#line 13
           System.Console.WriteLine(@"All other BDD frameworks are eaten should be true");
            contextBasicGrammarContext.AllOtherBddFrameworksAreEaten.ShouldBe((System.Object) this.ParameterConverter.Convert(@"true", typeof(System.Object)));
        }
        [Test] public void parameterizing_and_adding_a_basic_assertion(){
            var contextBasicGrammarContext = new Tutorial.BasicGrammarContext();
#line 16
           System.Console.WriteLine(@"When my name is ""Some other BDD framework""");
            contextBasicGrammarContext.When_my_name_is(@"Some other BDD framework");
#line 17
           System.Console.WriteLine(@"All other BDD frameworks are eaten should be false");
            contextBasicGrammarContext.AllOtherBddFrameworksAreEaten.ShouldBe((System.Object) this.ParameterConverter.Convert(@"false", typeof(System.Object)));
        }
        [Test] public void matching_with_an_integer_parameter_in_the_middle(){
            var contextAgeContext = new Tutorial.AgeContext();
#line 20
           System.Console.WriteLine(@"Given I am 42 years old");
            contextAgeContext.Given_I_am_age_years_old((System.Int32) this.ParameterConverter.Convert(@"42", typeof(System.Int32)));
#line 21
           System.Console.WriteLine(@"My age in one year should be 43");
            contextAgeContext.My_age_in_one_year().ShouldBe((System.Object) this.ParameterConverter.Convert(@"43", typeof(System.Object)));
        }

    }
}


namespace TestNamespace {
    
    

    [TestFixture]
    
    public class Chaining_Specs {
#line 1 "Chaining.feature"
#line hidden
        private StorEvil.Interpreter.ParameterConverters.ParameterConverter ParameterConverter = new StorEvil.Interpreter.ParameterConverters.ParameterConverter();
        [TestFixtureSetUp]
        public void WriteStoryToConsole() {
            Console.WriteLine(@"StorEvil supports chaining calls to methods.
That is, it allows you to return any type from a method
and use its methods, fields or properties, as well as any applicable
extension methods, to match the plain text scenario line.
 ");
        }
        
        [Test] public void No_chaining(){

        }
        [Test] public void Property_chaining(){

        }
        [Test] public void Extension_method_chaining(){

        }
        [Test] public void Method_chaining(){

        }
        [Test] public void Multiple_levels_of_chaining_in_one_call(){

        }

    }
}


namespace TestNamespace {
    using Tutorial;
    

    [TestFixture]
    
    public class Tables_Specs {
#line 1 "Tables.feature"
#line hidden
        private StorEvil.Interpreter.ParameterConverters.ParameterConverter ParameterConverter = new StorEvil.Interpreter.ParameterConverters.ParameterConverter();
        [TestFixtureSetUp]
        public void WriteStoryToConsole() {
            Console.WriteLine(@"StorEvil has various different ways you can use tabular data to
drive a scenario. It supports mapping tables of data to parameters
on your context methods depending on the type of parameter.

You can map tabular data to:
- an array of arrays of any basic type that is supported by StorEvil
- (int, decimal, string, as well as enum types)
- an array of any type with public properties and fields
- top row of the table is used to determine the names of the fields

You can also map name/value pairs (in a table with two columns) to:
- A single instance of any type
- A hashtable
 ");
        }
        
        [Test] public void A_table_of_data_can_map_to_a_string____(){
            var contextTableContext = new Tutorial.TableContext();
#line 16
           System.Console.WriteLine(@"Given the following competition groups:
| South Africa| Mexico    | Uruguay     | France   |
| Argentina   | Nigeria   | South Korea | Greece   |
| USA         | Algeria   | England     | Slovenia |
| Germany     | Australia | Serbia      | Ghana    |");
            contextTableContext.Given_the_following_competition_groups((System.String[][]) this.ParameterConverter.Convert(@"| South Africa| Mexico    | Uruguay     | France   |
| Argentina   | Nigeria   | South Korea | Greece   |
| USA         | Algeria   | England     | Slovenia |
| Germany     | Australia | Serbia      | Ghana    |", typeof(System.String[][])));
#line 22
           System.Console.WriteLine(@"Then there should be 4 groups");
            contextTableContext.then_there_should_be_number_groups((System.Int32) this.ParameterConverter.Convert(@"4", typeof(System.Int32)));
#line 23
           System.Console.WriteLine(@"And USA and England should be in the same group");
            contextTableContext.then_team1_and_team2_should_be_in_the_same_group(@"USA", @"England");
        }
        [Test] public void A_table_of_data_that_maps_to_an_array_of_types(){
            var contextTableContext = new Tutorial.TableContext();
#line 27
           System.Console.WriteLine(@"Given the following teams:
| Rank | Nation      | Region       |
| 1    | Spain       | Europe       |
| 2    | Brazil      | SouthAmerica |
| 3    | Netherlands | Europe       |
| 4    | Italy       | Europe       |");
            contextTableContext.Given_the_following_teams((Tutorial.TeamInfo[]) this.ParameterConverter.Convert(@"| Rank | Nation      | Region       |
| 1    | Spain       | Europe       |
| 2    | Brazil      | SouthAmerica |
| 3    | Netherlands | Europe       |
| 4    | Italy       | Europe       |", typeof(Tutorial.TeamInfo[])));
#line 34
           System.Console.WriteLine(@"Then there should be 4 teams");
            contextTableContext.then_there_should_be_count_teams((System.Int32) this.ParameterConverter.Convert(@"4", typeof(System.Int32)));
#line 35
           System.Console.WriteLine(@"and Spain should be ranked 1");
            contextTableContext.then_nation_should_be_ranked_expectedRank(@"Spain", (System.Int32) this.ParameterConverter.Convert(@"1", typeof(System.Int32)));
#line 36
           System.Console.WriteLine(@"and Italy should be in Europe");
            contextTableContext.then_nation_should_be_in_region(@"Italy", (Tutorial.Regions) this.ParameterConverter.Convert(@"Europe", typeof(Tutorial.Regions)));
#line 37
           System.Console.WriteLine(@"and Brazil should be ranked 2");
            contextTableContext.then_nation_should_be_ranked_expectedRank(@"Brazil", (System.Int32) this.ParameterConverter.Convert(@"2", typeof(System.Int32)));
        }
        [Test] public void Setting_attributes_of_a_single_instance_of_any_type(){
            var contextTableContext = new Tutorial.TableContext();
#line 41
           System.Console.WriteLine(@"Given the following team:
| Rank	 | 18           |
| Nation | USA          |
| Region | NorthAmerica |");
            contextTableContext.Given_the_following_team((Tutorial.TeamInfo) this.ParameterConverter.Convert(@"| Rank	 | 18           |
| Nation | USA          |
| Region | NorthAmerica |", typeof(Tutorial.TeamInfo)));
#line 46
           System.Console.WriteLine(@"And the following team:
| Rank	 | 39           |
| Nation | Ireland      |
| Region | Europe       |");
            contextTableContext.Given_the_following_team((Tutorial.TeamInfo) this.ParameterConverter.Convert(@"| Rank	 | 39           |
| Nation | Ireland      |
| Region | Europe       |", typeof(Tutorial.TeamInfo)));
#line 51
           System.Console.WriteLine(@"Then there should be 2 teams");
            contextTableContext.then_there_should_be_count_teams((System.Int32) this.ParameterConverter.Convert(@"2", typeof(System.Int32)));
#line 52
           System.Console.WriteLine(@"and USA should be ranked 18");
            contextTableContext.then_nation_should_be_ranked_expectedRank(@"USA", (System.Int32) this.ParameterConverter.Convert(@"18", typeof(System.Int32)));
#line 53
           System.Console.WriteLine(@"and Ireland should be in Europe");
            contextTableContext.then_nation_should_be_in_region(@"Ireland", (Tutorial.Regions) this.ParameterConverter.Convert(@"Europe", typeof(Tutorial.Regions)));
        }
        [Test] public void Using_a_hash_table(){
            var contextTableContext = new Tutorial.TableContext();
#line 57
           System.Console.WriteLine(@"Given the following roster:
| 7  | Beasley          |
| 12 | Altidore         |
| 1  | Howard           |");
            contextTableContext.Given_the_following_roster((System.Collections.Hashtable) this.ParameterConverter.Convert(@"| 7  | Beasley          |
| 12 | Altidore         |
| 1  | Howard           |", typeof(System.Collections.Hashtable)));
#line 62
           System.Console.WriteLine(@"Then there should be 3 players");
            contextTableContext.then_there_should_be_count_players((System.Int32) this.ParameterConverter.Convert(@"3", typeof(System.Int32)));
#line 63
           System.Console.WriteLine(@"and Beasley should be number 7");
            contextTableContext.then_playerName_should_be_number_expectedNumber(@"Beasley", (System.Int32) this.ParameterConverter.Convert(@"7", typeof(System.Int32)));
#line 64
           System.Console.WriteLine(@"and Altidore should be number 12");
            contextTableContext.then_playerName_should_be_number_expectedNumber(@"Altidore", (System.Int32) this.ParameterConverter.Convert(@"12", typeof(System.Int32)));
#line 65
           System.Console.WriteLine(@"and Howard should be number 1");
            contextTableContext.then_playerName_should_be_number_expectedNumber(@"Howard", (System.Int32) this.ParameterConverter.Convert(@"1", typeof(System.Int32)));
        }

    }
}