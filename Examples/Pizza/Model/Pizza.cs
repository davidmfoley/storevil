using System.Collections.Generic;

namespace Pizza.Model
{
    public enum PizzaSize { Small, Medium, Large }


    public class Pizza : IMenuItem
    {
        public Pizza(PizzaSize size)
        {
            Size = size;
        }

        private List<Topping> _toppings = new List<Topping>();
        public IEnumerable<Topping> Toppings
        {
            get
            {
                return _toppings.ToArray();
            }
        }


        public PizzaSize Size { get; set; }


        public string Description
        {
            get { return Size + " pizza"; }
        }

        public decimal RetailPrice
        {
            get
            {
                switch (Size)
                {
                    case (PizzaSize.Small):
                        return 10 + _toppings.Count;
                    case (PizzaSize.Medium):
                        return 12 + 1.5M * _toppings.Count;
                    case (PizzaSize.Large):
                        return 15 + 2.0M * _toppings.Count;
                }

                return 0;
            }
        }

        public void AddTopping(Topping topping)
        {
            _toppings.Add(topping);
        }
    }
}


