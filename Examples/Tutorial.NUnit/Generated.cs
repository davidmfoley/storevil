using NUnit.Framework;
using System;
using StorEvil;



namespace StorEvilSpecifications {
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
           System.Console.WriteLine(@"When my name is StorEvil");
#line 6
contextBasicGrammarContext.When_my_name_is(@"StorEvil");
#line hidden


        }
        [Test] public void a_basic_assertion(){
            var contextBasicGrammarContext = new Tutorial.BasicGrammarContext();
           System.Console.WriteLine(@"When my name is StorEvil");
#line 9
contextBasicGrammarContext.When_my_name_is(@"StorEvil");
#line hidden
           System.Console.WriteLine(@"I should eat all other BDD frameworks");
#line 10
contextBasicGrammarContext.I_should_eat_all_other_BDD_frameworks();
#line hidden


        }
        [Test] public void assertion_using_the_ShouldXXX_methods(){
            var contextBasicGrammarContext = new Tutorial.BasicGrammarContext();
           System.Console.WriteLine(@"When my name is StorEvil");
#line 13
contextBasicGrammarContext.When_my_name_is(@"StorEvil");
#line hidden
           System.Console.WriteLine(@"All other BDD frameworks are eaten should be true");
#line 14
contextBasicGrammarContext.AllOtherBddFrameworksAreEaten.ShouldBe((System.Object) this.ParameterConverter.Convert(@"true", typeof(System.Object)));
#line hidden


        }
        [Test] public void parameterizing_and_adding_a_basic_assertion(){
            var contextBasicGrammarContext = new Tutorial.BasicGrammarContext();
           System.Console.WriteLine(@"When my name is ""Some other BDD framework""");
#line 17
contextBasicGrammarContext.When_my_name_is(@"Some other BDD framework");
#line hidden
           System.Console.WriteLine(@"All other BDD frameworks are eaten should be false");
#line 18
contextBasicGrammarContext.AllOtherBddFrameworksAreEaten.ShouldBe((System.Object) this.ParameterConverter.Convert(@"false", typeof(System.Object)));
#line hidden


        }
        [Test] public void matching_with_an_integer_parameter_in_the_middle(){
            var contextAgeContext = new Tutorial.AgeContext();
           System.Console.WriteLine(@"Given I am 42 years old");
#line 21
contextAgeContext.Given_I_am_age_years_old((System.Int32) this.ParameterConverter.Convert(@"42", typeof(System.Int32)));
#line hidden
           System.Console.WriteLine(@"My age in one year should be 43");
#line 22
contextAgeContext.My_age_in_one_year().ShouldBe((System.Object) this.ParameterConverter.Convert(@"43", typeof(System.Object)));
#line hidden


        }

    }
}


namespace StorEvilSpecifications {
    
    

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


namespace StorEvilSpecifications {
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
           System.Console.WriteLine(@"Given the following competition groups:
| South Africa| Mexico    | Uruguay     | France   |
| Argentina   | Nigeria   | South Korea | Greece   |
| USA         | Algeria   | England     | Slovenia |
| Germany     | Australia | Serbia      | Ghana    |");
#line 17
contextTableContext.Given_the_following_competition_groups((System.String[][]) this.ParameterConverter.Convert(@"| South Africa| Mexico    | Uruguay     | France   |
| Argentina   | Nigeria   | South Korea | Greece   |
| USA         | Algeria   | England     | Slovenia |
| Germany     | Australia | Serbia      | Ghana    |", typeof(System.String[][])));
#line hidden
           System.Console.WriteLine(@"Then there should be 4 groups");
#line 23
contextTableContext.then_there_should_be_number_groups((System.Int32) this.ParameterConverter.Convert(@"4", typeof(System.Int32)));
#line hidden
           System.Console.WriteLine(@"And USA and England should be in the same group");
#line 24
contextTableContext.then_team1_and_team2_should_be_in_the_same_group(@"USA", @"England");
#line hidden


        }
        [Test] public void A_table_of_data_that_maps_to_an_array_of_types(){
            var contextTableContext = new Tutorial.TableContext();
           System.Console.WriteLine(@"Given the following teams:
| Rank | Nation      | Region       |
| 1    | Spain       | Europe       |
| 2    | Brazil      | SouthAmerica |
| 3    | Netherlands | Europe       |
| 4    | Italy       | Europe       |");
#line 28
contextTableContext.Given_the_following_teams((Tutorial.TeamInfo[]) this.ParameterConverter.Convert(@"| Rank | Nation      | Region       |
| 1    | Spain       | Europe       |
| 2    | Brazil      | SouthAmerica |
| 3    | Netherlands | Europe       |
| 4    | Italy       | Europe       |", typeof(Tutorial.TeamInfo[])));
#line hidden
           System.Console.WriteLine(@"Then there should be 4 teams");
#line 35
contextTableContext.then_there_should_be_count_teams((System.Int32) this.ParameterConverter.Convert(@"4", typeof(System.Int32)));
#line hidden
           System.Console.WriteLine(@"and Spain should be ranked 1");
#line 36
contextTableContext.then_nation_should_be_ranked_expectedRank(@"Spain", (System.Int32) this.ParameterConverter.Convert(@"1", typeof(System.Int32)));
#line hidden
           System.Console.WriteLine(@"and Italy should be in Europe");
#line 37
contextTableContext.then_nation_should_be_in_region(@"Italy", (Tutorial.Regions) this.ParameterConverter.Convert(@"Europe", typeof(Tutorial.Regions)));
#line hidden
           System.Console.WriteLine(@"and Brazil should be ranked 2");
#line 38
contextTableContext.then_nation_should_be_ranked_expectedRank(@"Brazil", (System.Int32) this.ParameterConverter.Convert(@"2", typeof(System.Int32)));
#line hidden


        }
        [Test] public void Setting_attributes_of_a_single_instance_of_any_type(){
            var contextTableContext = new Tutorial.TableContext();
           System.Console.WriteLine(@"Given the following team:
| Rank	 | 18           |
| Nation | USA          |
| Region | NorthAmerica |");
#line 42
contextTableContext.Given_the_following_team((Tutorial.TeamInfo) this.ParameterConverter.Convert(@"| Rank	 | 18           |
| Nation | USA          |
| Region | NorthAmerica |", typeof(Tutorial.TeamInfo)));
#line hidden
           System.Console.WriteLine(@"And the following team:
| Rank	 | 39           |
| Nation | Ireland      |
| Region | Europe       |");
#line 47
contextTableContext.Given_the_following_team((Tutorial.TeamInfo) this.ParameterConverter.Convert(@"| Rank	 | 39           |
| Nation | Ireland      |
| Region | Europe       |", typeof(Tutorial.TeamInfo)));
#line hidden
           System.Console.WriteLine(@"Then there should be 2 teams");
#line 52
contextTableContext.then_there_should_be_count_teams((System.Int32) this.ParameterConverter.Convert(@"2", typeof(System.Int32)));
#line hidden
           System.Console.WriteLine(@"and USA should be ranked 18");
#line 53
contextTableContext.then_nation_should_be_ranked_expectedRank(@"USA", (System.Int32) this.ParameterConverter.Convert(@"18", typeof(System.Int32)));
#line hidden
           System.Console.WriteLine(@"and Ireland should be in Europe");
#line 54
contextTableContext.then_nation_should_be_in_region(@"Ireland", (Tutorial.Regions) this.ParameterConverter.Convert(@"Europe", typeof(Tutorial.Regions)));
#line hidden


        }
        [Test] public void Using_a_hash_table(){
            var contextTableContext = new Tutorial.TableContext();
           System.Console.WriteLine(@"Given the following roster:
| 7  | Beasley          |
| 12 | Altidore         |
| 1  | Howard           |");
#line 58
contextTableContext.Given_the_following_roster((System.Collections.Hashtable) this.ParameterConverter.Convert(@"| 7  | Beasley          |
| 12 | Altidore         |
| 1  | Howard           |", typeof(System.Collections.Hashtable)));
#line hidden
           System.Console.WriteLine(@"Then there should be 3 players");
#line 63
contextTableContext.then_there_should_be_count_players((System.Int32) this.ParameterConverter.Convert(@"3", typeof(System.Int32)));
#line hidden
           System.Console.WriteLine(@"and Beasley should be number 7");
#line 64
contextTableContext.then_playerName_should_be_number_expectedNumber(@"Beasley", (System.Int32) this.ParameterConverter.Convert(@"7", typeof(System.Int32)));
#line hidden
           System.Console.WriteLine(@"and Altidore should be number 12");
#line 65
contextTableContext.then_playerName_should_be_number_expectedNumber(@"Altidore", (System.Int32) this.ParameterConverter.Convert(@"12", typeof(System.Int32)));
#line hidden
           System.Console.WriteLine(@"and Howard should be number 1");
#line 66
contextTableContext.then_playerName_should_be_number_expectedNumber(@"Howard", (System.Int32) this.ParameterConverter.Convert(@"1", typeof(System.Int32)));
#line hidden


        }

    }
}