using System;

using StorEvil;


namespace Pizza.TestContext
{
    [Context("Basic Pricing", "Discounts")]
    public class PriceCalculatorTestContext
    {
        readonly Order _order = new Order();

        public void When_customer_orders_a_size_pizzaType_pizza(string size, string pizzaType)
        {
            var pizza = new Pizza((PizzaSize)Enum.Parse(typeof(PizzaSize),size, true));

            // add toppings for type

            _order.Add(pizza);

        }

        public void When_customer_orders_quantity_size_pizzaType_pizzas(int quantity, string size, string pizzaType)
        {
            for (var i = 0; i < quantity; i++)
            {
                var pizza = new Pizza(ParsePizzaSize(size));
                _order.Add(pizza);
            }           
        }
        
        public void When_customer_orders_quantity_pizzaType_slices_and_a_coke(int quantity, string pizzaType)
        {
            for (int i = 0; i < quantity; i++)
                _order.Add(new Slice());

            _order.Add(new Soda());
        }

        public PizzaSpec When_Customer_Orders_a_size_Pizza(string size)
        {
            var pizza = new Pizza(ParsePizzaSize(size));
            _order.Add(pizza);
            return new PizzaSpec(pizza);
        }

        public void When_Customer_Orders_A_Beer()
        {
            _order.Add(new Beer());
        }

        public void When_Orders_A_Slice()
        {
            _order.Add(new Slice());
        }

        private static PizzaSize ParsePizzaSize(string size)
        {
            return (PizzaSize)Enum.Parse(typeof(PizzaSize), size, true);
        }

        /// <summary>
        /// The grand total for the order
        /// </summary>
        public decimal Grand_Total
        {
            get
            {
                return new PriceCalculator().CalculateTotal(_order);

            }
        }
    }

    public class Beer : IMenuItem
    {
        public string Description
        {
            get { return "Beer"; }
        }

        public decimal RetailPrice
        {
            get { return 4M; }
        }
    }
}
