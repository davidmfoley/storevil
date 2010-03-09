using Pizza.TestContext;
using Topping = Pizza.Specifications.Model.Topping;

namespace Pizza.Specifications.Context
{
    public class PizzaSpec
    {
        public PizzaSpec(Model.Pizza pizza)
        {
            Pizza = pizza;
        }

        public Model.Pizza Pizza { get; set; }

        public void With_toppingCount_Toppings(int toppingCount)
        {
            for (int i = 0; i < toppingCount; i++)
            {
                Pizza.AddTopping(new Topping {Name="Test Topping"});
            }
        }
    }
}