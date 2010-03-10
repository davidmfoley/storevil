using NUnit.Framework;
using System;
using StorEvil;



namespace TestNamespace {
    

    [TestFixture]
    public class Basic_Pricing_ {
        [TestFixtureSetUp]
        public void WriteStoryToConsole() {
            Console.WriteLine(@"Basic Pricing
 ");
        }
        
        [Test] public void When_customer_orders_a_large_cheese_pizza_Grand_Total_should_be__15(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When customer orders a large cheese pizza");
            contextPriceCalculatorTestContext.When_customer_orders_a_size_pizzaType_pizza(@"large", @"cheese");
           Console.WriteLine(@"Grand Total should be $15");
            contextPriceCalculatorTestContext.Grand_Total.ShouldBe(@"$15");
        }
        [Test] public void When_customer_orders_a_medium_cheese_pizza_Grand_Total_should_be__12(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When customer orders a medium cheese pizza");
            contextPriceCalculatorTestContext.When_customer_orders_a_size_pizzaType_pizza(@"medium", @"cheese");
           Console.WriteLine(@"Grand Total should be $12");
            contextPriceCalculatorTestContext.Grand_Total.ShouldBe(@"$12");
        }
        [Test] public void When_customer_orders_a_small_cheese_pizza_Grand_Total_should_be__10(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When customer orders a small cheese pizza");
            contextPriceCalculatorTestContext.When_customer_orders_a_size_pizzaType_pizza(@"small", @"cheese");
           Console.WriteLine(@"Grand Total should be $10");
            contextPriceCalculatorTestContext.Grand_Total.ShouldBe(@"$10");
        }
        [Test] public void When_Customer_orders_a_large_pizza_with_3_toppings_Grand_Total_should_be__21(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When Customer orders a large pizza with 3 toppings");
            contextPriceCalculatorTestContext.When_Customer_Orders_a_size_Pizza(@"large").With_toppingCount_Toppings(3);
           Console.WriteLine(@"Grand Total should be $21");
            contextPriceCalculatorTestContext.Grand_Total.ShouldBe(@"$21");
        }
        [Test] public void When_Customer_orders_a_medium_pizza_with_3_toppings_Grand_Total_should_be__16_50(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When Customer orders a medium pizza with 3 toppings");
            contextPriceCalculatorTestContext.When_Customer_Orders_a_size_Pizza(@"medium").With_toppingCount_Toppings(3);
           Console.WriteLine(@"Grand Total should be $16.50");
            contextPriceCalculatorTestContext.Grand_Total.ShouldBe(@"$16.50");
        }
        [Test] public void When_Customer_orders_a_small_pizza_with_3_toppings_Grand_Total_should_be__13_50(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When Customer orders a small pizza with 3 toppings");
            contextPriceCalculatorTestContext.When_Customer_Orders_a_size_Pizza(@"small").With_toppingCount_Toppings(3);
           Console.WriteLine(@"Grand Total should be $13.50");
            contextPriceCalculatorTestContext.Grand_Total.ShouldBe(@"$13.50");
        }

    }
}


namespace TestNamespace {
   
    

    [TestFixture]
    public class Discounts_ {
        [TestFixtureSetUp]
        public void WriteStoryToConsole() {
            Console.WriteLine(@"Discounts
 ");
        }
        
        [Test] public void two_mediums_for_20_bucks_special(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When customer orders 2 medium cheese pizzas");
            contextPriceCalculatorTestContext.When_customer_orders_quantity_size_pizzaType_pizzas(2, @"medium", @"cheese");
           Console.WriteLine(@"Grand Total should be $20");
            contextPriceCalculatorTestContext.Grand_Total.ShouldBe(@"$20");
        }
        [Test] public void two_slices_and_a_soda_promo____cheese(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When customer orders 2 cheese slices and a coke");
            contextPriceCalculatorTestContext.When_customer_orders_quantity_pizzaType_slices_and_a_coke(2, @"cheese");
           Console.WriteLine(@"Grand total should be $6");
            contextPriceCalculatorTestContext.Grand_Total.ShouldBe(@"$6");
        }
        [Test] public void two_slices_and_a_soda_promo____combo(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When customer orders 2 combo slices and a coke");
            contextPriceCalculatorTestContext.When_customer_orders_quantity_pizzaType_slices_and_a_coke(2, @"combo");
           Console.WriteLine(@"Grand total should be $6");
            contextPriceCalculatorTestContext.Grand_Total.ShouldBe(@"$6");
        }
        [Test] public void _10__discount_if_ordering_5_or_more_pizzas(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When customer orders 5 large cheese pizzas");
            contextPriceCalculatorTestContext.When_customer_orders_quantity_size_pizzaType_pizzas(5, @"large", @"cheese");
           Console.WriteLine(@"Grand total should be $67.50");
            contextPriceCalculatorTestContext.Grand_Total.ShouldBe(@"$67.50");
        }
        [Test] public void beer_should_be__4(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When customer orders a beer");
            contextPriceCalculatorTestContext.When_Customer_Orders_A_Beer();
           Console.WriteLine(@"Grand total should be $4");
            contextPriceCalculatorTestContext.Grand_Total.ShouldBe(@"$4");
        }
        [Test] public void pizza_and_beer_special_should_be__6__failing_test_(){
            var contextPriceCalculatorTestContext = new Pizza.TestContext.PriceCalculatorTestContext();
           Console.WriteLine(@"When customer orders a beer");
            contextPriceCalculatorTestContext.When_Customer_Orders_A_Beer();
           Console.WriteLine(@"and orders a slice");
            contextPriceCalculatorTestContext.When_Orders_A_Slice();
        }

    }
}