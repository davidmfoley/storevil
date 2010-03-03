namespace Pizza.TestContext
{
    public class PizzaSpec
    {
        public PizzaSpec(Pizza pizza)
        {
            Pizza = pizza;
        }

        public Pizza Pizza { get; set; }

        public void With_toppingCount_Toppings(int toppingCount)
        {
            for (int i = 0; i < toppingCount; i++)
            {
                Pizza.AddTopping(new Topping {Name="Test Topping"});
            }
        }
    }
}