using NUnit.Framework;
using System;
using StorEvil;

namespace StorEvilSpecs{

[NUnit.Framework.TestFixtureAttribute] public class BasicPricing_feature : StorEvil.CodeGeneration.TestFixture {
   public object Contexts { get { return base.GetContexts();}}    [NUnit.Framework.SetUpAttribute]
    public void HandleSetUp() { base.BeforeEach(); }
    [NUnit.Framework.TestFixtureSetUpAttribute]
    public void HandleTestFixtureSetUp() { SetListener(new StorEvil.CodeGeneration.NUnitListener()); base.BeforeAll(); }
    [NUnit.Framework.TearDownAttribute]
    public void HandleTearDown() { base.AfterEach(); }
    [NUnit.Framework.TestFixtureTearDownAttribute]
    public void HandleTestFixtureTearDown() { base.AfterAll(); }
  [NUnit.Framework.TestAttribute] public void When_customer_orders_a_large_cheese_pizza__Grand_Total_should_be__15() {
#line 1  "BasicPricing.feature"
#line hidden
#line 4
ExecuteLine(@"When customer orders a large cheese pizza");
#line hidden

#line 5
ExecuteLine(@"Grand Total should be $15");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void When_customer_orders_a_medium_cheese_pizza__Grand_Total_should_be__12() {
#line 1  "BasicPricing.feature"
#line hidden
#line 8
ExecuteLine(@"When customer orders a medium cheese pizza");
#line hidden

#line 9
ExecuteLine(@"Grand Total should be $12");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void When_customer_orders_a_small_cheese_pizza__Grand_Total_should_be__10() {
#line 1  "BasicPricing.feature"
#line hidden
#line 12
ExecuteLine(@"When customer orders a small cheese pizza");
#line hidden

#line 13
ExecuteLine(@"Grand Total should be $10");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void When_Customer_orders_a_large_pizza_with_3_toppings__Grand_Total_should_be__21() {
#line 1  "BasicPricing.feature"
#line hidden
#line 16
ExecuteLine(@"When Customer orders a large pizza with 3 toppings");
#line hidden

#line 17
ExecuteLine(@"Grand Total should be $21");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void When_Customer_orders_a_medium_pizza_with_3_toppings__Grand_Total_should_be__16_50() {
#line 1  "BasicPricing.feature"
#line hidden
#line 20
ExecuteLine(@"When Customer orders a medium pizza with 3 toppings");
#line hidden

#line 21
ExecuteLine(@"Grand Total should be $16.50");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void When_Customer_orders_a_small_pizza_with_3_toppings__Grand_Total_should_be__13_00() {
#line 1  "BasicPricing.feature"
#line hidden
#line 24
ExecuteLine(@"When Customer orders a small pizza with 3 toppings");
#line hidden

#line 25
ExecuteLine(@"Grand Total should be $13.00");
#line hidden
  }
  }
}
namespace StorEvilSpecs{

[NUnit.Framework.TestFixtureAttribute] public class Discounts_feature : StorEvil.CodeGeneration.TestFixture {
   public object Contexts { get { return base.GetContexts();}}    [NUnit.Framework.SetUpAttribute]
    public void HandleSetUp() { base.BeforeEach(); }
    [NUnit.Framework.TestFixtureSetUpAttribute]
    public void HandleTestFixtureSetUp() { SetListener(new StorEvil.CodeGeneration.NUnitListener()); base.BeforeAll(); }
    [NUnit.Framework.TearDownAttribute]
    public void HandleTearDown() { base.AfterEach(); }
    [NUnit.Framework.TestFixtureTearDownAttribute]
    public void HandleTestFixtureTearDown() { base.AfterAll(); }
  [NUnit.Framework.TestAttribute] public void two_mediums_for_20_bucks_special() {
#line 1  "Discounts.feature"
#line hidden
#line 4
ExecuteLine(@"When customer orders 2 medium cheese pizzas");
#line hidden

#line 5
ExecuteLine(@"Grand Total should be $20");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void two_slices_and_a_soda_promo____cheese() {
#line 1  "Discounts.feature"
#line hidden
#line 8
ExecuteLine(@"When customer orders 2 cheese slices and a coke");
#line hidden

#line 9
ExecuteLine(@"Grand total should be $6");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void two_slices_and_a_soda_promo____combo() {
#line 1  "Discounts.feature"
#line hidden
#line 12
ExecuteLine(@"When customer orders 2 combo slices and a coke");
#line hidden

#line 13
ExecuteLine(@"Grand total should be $6");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void _10__discount_if_ordering_5_or_more_pizzas() {
#line 1  "Discounts.feature"
#line hidden
#line 16
ExecuteLine(@"When customer orders 5 large cheese pizzas");
#line hidden

#line 17
ExecuteLine(@"Grand total should be $67.50");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void beer_should_be__4() {
#line 1  "Discounts.feature"
#line hidden
#line 20
ExecuteLine(@"When customer orders a beer");
#line hidden

#line 21
ExecuteLine(@"Grand total should be $4");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Example_of_a_failure__pizza_and_beer_special_should_be__6() {
#line 1  "Discounts.feature"
#line hidden
#line 25
ExecuteLine(@"When customer orders a beer");
#line hidden

#line 26
ExecuteLine(@"and orders a slice");
#line hidden

#line 27
ExecuteLine(@"Grand total should be $6");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Example_of_a_pending_step() {
#line 1  "Discounts.feature"
#line hidden
#line 31
ExecuteLine(@"When customer orders a beer");
#line hidden

#line 32
ExecuteLine(@"and orders a slice");
#line hidden

#line 34
ExecuteLine(@"and has a superduper discount coupon");
#line hidden

#line 36
ExecuteLine(@"Grand total should be $6");
#line hidden
  }
  }
}namespace StorEvilSpecs { [SetUpFixture] public class SetupAndTearDown {
  [SetUp] public void SetUp() {
   var eh = new StorEvil.Interpreter.ExtensionMethodHandler();
   // _sessionContext = new SessionContext();
    StorEvil.CodeGeneration.TestSession.AddAssembly(typeof(Pizza.Context.PizzaSpec).Assembly);
    eh.AddAssembly(typeof(Pizza.Context.PizzaSpec).Assembly);
    StorEvil.Interpreter.ParameterConverters.ParameterConverter.AddCustomConverters(typeof(Pizza.Context.PizzaSpec).Assembly.Location);
    StorEvil.CodeGeneration.TestSession.AddAssembly(typeof(StorEvil.Utility.TestExtensionMethods).Assembly);
    eh.AddAssembly(typeof(StorEvil.Utility.TestExtensionMethods).Assembly);
    StorEvil.Interpreter.ParameterConverters.ParameterConverter.AddCustomConverters(typeof(StorEvil.Utility.TestExtensionMethods).Assembly.Location);

  }
  [TearDown] public void TearDown() {
      StorEvil.CodeGeneration.TestSession.EndSession();
  }
} }

