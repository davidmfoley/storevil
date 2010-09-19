using NUnit.Framework;
using System;
using StorEvil;

namespace StorEvilSpecs{

[NUnit.Framework.TestFixtureAttribute] public class AltNet_feature : StorEvil.CodeGeneration.TestFixture {
   public object Contexts { get { return base.GetContexts();}}    [NUnit.Framework.SetUpAttribute]
    public void HandleSetUp() { base.BeforeEach(); }
    [NUnit.Framework.TestFixtureSetUpAttribute]
    public void HandleTestFixtureSetUp() { SetListener(new StorEvil.CodeGeneration.NUnitListener()); base.BeforeAll(); }
    [NUnit.Framework.TearDownAttribute]
    public void HandleTearDown() { base.AfterEach(); }
    [NUnit.Framework.TestFixtureTearDownAttribute]
    public void HandleTestFixtureTearDown() { base.AfterAll(); }
  [NUnit.Framework.TestAttribute] public void Example() {
#line 1  "AltNet.feature"
#line hidden
#line 2
ExecuteLine(@"Given I have 5 cards in my hand");
#line hidden

#line 3
ExecuteLine(@"And my name is Dave");
#line hidden
  }
  }
}
namespace StorEvilSpecs{

[NUnit.Framework.TestFixtureAttribute] public class AmbiguousMatches_feature : StorEvil.CodeGeneration.TestFixture {
   public object Contexts { get { return base.GetContexts();}}    [NUnit.Framework.SetUpAttribute]
    public void HandleSetUp() { base.BeforeEach(); }
    [NUnit.Framework.TestFixtureSetUpAttribute]
    public void HandleTestFixtureSetUp() { SetListener(new StorEvil.CodeGeneration.NUnitListener()); base.BeforeAll(); }
    [NUnit.Framework.TearDownAttribute]
    public void HandleTearDown() { base.AfterEach(); }
    [NUnit.Framework.TestFixtureTearDownAttribute]
    public void HandleTestFixtureTearDown() { base.AfterAll(); }
  [NUnit.Framework.TestAttribute] public void An_ambiguous_match() {
#line 1  "AmbiguousMatches.feature"
#line hidden
#line 5
ExecuteLine(@"using foo context");
#line hidden

#line 6
ExecuteLine(@"call an ambiguous method");
#line hidden

#line 7
ExecuteLine(@"method on foo should have been called");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void An_ambiguous_match_where_both_are_called() {
#line 1  "AmbiguousMatches.feature"
#line hidden
#line 10
ExecuteLine(@"using foo context");
#line hidden

#line 11
ExecuteLine(@"using bar context");
#line hidden

#line 12
ExecuteLine(@"call an ambiguous method");
#line hidden

#line 13
ExecuteLine(@"method on bar should have been called");
#line hidden
  }
  }
}
namespace StorEvilSpecs{

[NUnit.Framework.TestFixtureAttribute] public class Background_feature : StorEvil.CodeGeneration.TestFixture {
   public object Contexts { get { return base.GetContexts();}}    [NUnit.Framework.SetUpAttribute]
    public void HandleSetUp() { base.BeforeEach(); }
    [NUnit.Framework.TestFixtureSetUpAttribute]
    public void HandleTestFixtureSetUp() { SetListener(new StorEvil.CodeGeneration.NUnitListener()); base.BeforeAll(); }
    [NUnit.Framework.TearDownAttribute]
    public void HandleTearDown() { base.AfterEach(); }
    [NUnit.Framework.TestFixtureTearDownAttribute]
    public void HandleTestFixtureTearDown() { base.AfterAll(); }
  [NUnit.Framework.TestAttribute] public void Background_run_count_should_be_1() {
#line 1  "Background.feature"
#line hidden
#line 4
ExecuteLine(@"Run the background");
#line hidden

#line 7
ExecuteLine(@"Background run count should be 1");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Background_run_count_should_be_2() {
#line 1  "Background.feature"
#line hidden
#line 4
ExecuteLine(@"Run the background");
#line hidden

#line 10
ExecuteLine(@"Background run count should be 2");
#line hidden
  }
  }
}
namespace StorEvilSpecs{

[NUnit.Framework.TestFixtureAttribute] public class BasicGrammar_feature : StorEvil.CodeGeneration.TestFixture {
   public object Contexts { get { return base.GetContexts();}}    [NUnit.Framework.SetUpAttribute]
    public void HandleSetUp() { base.BeforeEach(); }
    [NUnit.Framework.TestFixtureSetUpAttribute]
    public void HandleTestFixtureSetUp() { SetListener(new StorEvil.CodeGeneration.NUnitListener()); base.BeforeAll(); }
    [NUnit.Framework.TearDownAttribute]
    public void HandleTearDown() { base.AfterEach(); }
    [NUnit.Framework.TestFixtureTearDownAttribute]
    public void HandleTestFixtureTearDown() { base.AfterAll(); }
  [NUnit.Framework.TestAttribute] public void basic_matching_of_plaintext_line() {
#line 1  "BasicGrammar.feature"
#line hidden
#line 6
ExecuteLine(@"When my name is StorEvil");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void a_basic_assertion() {
#line 1  "BasicGrammar.feature"
#line hidden
#line 9
ExecuteLine(@"When my name is StorEvil");
#line hidden

#line 10
ExecuteLine(@"I should eat all other BDD frameworks");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void assertion_using_the_ShouldXXX_methods() {
#line 1  "BasicGrammar.feature"
#line hidden
#line 13
ExecuteLine(@"When my name is StorEvil");
#line hidden

#line 14
ExecuteLine(@"All other BDD frameworks are eaten should be true");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void parameterizing_and_adding_a_basic_assertion() {
#line 1  "BasicGrammar.feature"
#line hidden
#line 17
ExecuteLine(@"When my name is ""Some other BDD framework""");
#line hidden

#line 18
ExecuteLine(@"All other BDD frameworks are eaten should be false");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void matching_with_an_integer_parameter_in_the_middle() {
#line 1  "BasicGrammar.feature"
#line hidden
#line 21
ExecuteLine(@"Given I am 42 years old");
#line hidden

#line 22
ExecuteLine(@"My age in one year should be 43");
#line hidden
  }
  }
}
namespace StorEvilSpecs{

[NUnit.Framework.TestFixtureAttribute] public class Chaining_feature : StorEvil.CodeGeneration.TestFixture {
   public object Contexts { get { return base.GetContexts();}}    [NUnit.Framework.SetUpAttribute]
    public void HandleSetUp() { base.BeforeEach(); }
    [NUnit.Framework.TestFixtureSetUpAttribute]
    public void HandleTestFixtureSetUp() { SetListener(new StorEvil.CodeGeneration.NUnitListener()); base.BeforeAll(); }
    [NUnit.Framework.TearDownAttribute]
    public void HandleTearDown() { base.AfterEach(); }
    [NUnit.Framework.TestFixtureTearDownAttribute]
    public void HandleTestFixtureTearDown() { base.AfterAll(); }
  [NUnit.Framework.TestAttribute] public void No_chaining() {
#line 1  "Chaining.feature"
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Property_chaining() {
#line 1  "Chaining.feature"
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Extension_method_chaining() {
#line 1  "Chaining.feature"
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Method_chaining() {
#line 1  "Chaining.feature"
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Multiple_levels_of_chaining_in_one_call() {
#line 1  "Chaining.feature"
#line hidden
  }
  }
}
namespace StorEvilSpecs{

[NUnit.Framework.TestFixtureAttribute] public class CustomParameterConversion_feature : StorEvil.CodeGeneration.TestFixture {
   public object Contexts { get { return base.GetContexts();}}    [NUnit.Framework.SetUpAttribute]
    public void HandleSetUp() { base.BeforeEach(); }
    [NUnit.Framework.TestFixtureSetUpAttribute]
    public void HandleTestFixtureSetUp() { SetListener(new StorEvil.CodeGeneration.NUnitListener()); base.BeforeAll(); }
    [NUnit.Framework.TearDownAttribute]
    public void HandleTearDown() { base.AfterEach(); }
    [NUnit.Framework.TestFixtureTearDownAttribute]
    public void HandleTestFixtureTearDown() { base.AfterAll(); }
  [NUnit.Framework.TestAttribute] public void custom_float_conversion() {
#line 1  "CustomParameterConversion.feature"
#line hidden
#line 4
ExecuteLine(@"We can parse floats like 3.14159265");
#line hidden

#line 5
ExecuteLine(@"and the result should be between 3.14 and 3.15");
#line hidden
  }
  }
}
namespace StorEvilSpecs{

[NUnit.Framework.TestFixtureAttribute] public class Tables_feature : StorEvil.CodeGeneration.TestFixture {
   public object Contexts { get { return base.GetContexts();}}    [NUnit.Framework.SetUpAttribute]
    public void HandleSetUp() { base.BeforeEach(); }
    [NUnit.Framework.TestFixtureSetUpAttribute]
    public void HandleTestFixtureSetUp() { SetListener(new StorEvil.CodeGeneration.NUnitListener()); base.BeforeAll(); }
    [NUnit.Framework.TearDownAttribute]
    public void HandleTearDown() { base.AfterEach(); }
    [NUnit.Framework.TestFixtureTearDownAttribute]
    public void HandleTestFixtureTearDown() { base.AfterAll(); }
  [NUnit.Framework.TestAttribute] public void A_table_of_data_can_map_to_a_string____() {
#line 1  "Tables.feature"
#line hidden
#line 17
ExecuteLine(@"Given the following competition groups:
| South Africa| Mexico    | Uruguay     | France   |
| Argentina   | Nigeria   | South Korea | Greece   |
| USA         | Algeria   | England     | Slovenia |
| Germany     | Australia | Serbia      | Ghana    |");
#line hidden

#line 23
ExecuteLine(@"Then there should be 4 groups");
#line hidden

#line 24
ExecuteLine(@"And USA and England should be in the same group");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void A_table_of_data_that_maps_to_an_array_of_types() {
#line 1  "Tables.feature"
#line hidden
#line 28
ExecuteLine(@"Given the following teams:
| Rank | Nation      | Region       |
| 1    | Spain       | Europe       |
| 2    | Brazil      | SouthAmerica |
| 3    | Netherlands | Europe       |
| 4    | Italy       | Europe       |");
#line hidden

#line 35
ExecuteLine(@"Then there should be 4 teams");
#line hidden

#line 36
ExecuteLine(@"and Spain should be ranked 1");
#line hidden

#line 37
ExecuteLine(@"and Italy should be in Europe");
#line hidden

#line 38
ExecuteLine(@"and Brazil should be ranked 2");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Setting_attributes_of_a_single_instance_of_any_type() {
#line 1  "Tables.feature"
#line hidden
#line 42
ExecuteLine(@"Given the following team:
| Rank	 | 18           |
| Nation | USA          |
| Region | NorthAmerica |");
#line hidden

#line 47
ExecuteLine(@"And the following team:
| Rank	 | 39           |
| Nation | Ireland      |
| Region | Europe       |");
#line hidden

#line 52
ExecuteLine(@"Then there should be 2 teams");
#line hidden

#line 53
ExecuteLine(@"and USA should be ranked 18");
#line hidden

#line 54
ExecuteLine(@"and Ireland should be in Europe");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Using_a_hash_table() {
#line 1  "Tables.feature"
#line hidden
#line 58
ExecuteLine(@"Given the following roster:
| 7  | Beasley          |
| 12 | Altidore         |
| 1  | Howard           |");
#line hidden

#line 63
ExecuteLine(@"Then there should be 3 players");
#line hidden

#line 64
ExecuteLine(@"and Beasley should be number 7");
#line hidden

#line 65
ExecuteLine(@"and Altidore should be number 12");
#line hidden

#line 66
ExecuteLine(@"and Howard should be number 1");
#line hidden
  }
  }
}namespace StorEvilSpecs { [SetUpFixture] public class SetupAndTearDown {
  [SetUp] public void SetUp() {
    var assemblyRegistry = new StorEvil.Context.AssemblyRegistry( new System.Reflection.Assembly[] {
typeof(Tutorial.CustomParameterConversionContext).Assembly
    });
   var eh = new StorEvil.Interpreter.ExtensionMethodHandler(assemblyRegistry);
   // _sessionContext = new SessionContext();

  }
  [TearDown] public void TearDown() {
      StorEvil.CodeGeneration.TestSession.EndSession();
  }
} }

