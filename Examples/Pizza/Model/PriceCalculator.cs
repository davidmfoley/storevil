using System.Collections.Generic;

namespace Pizza.Model
{
    class PriceCalculator
    {
        public decimal CalculateTotal(Order _order)
        {

            List<IMenuItem> items = new List<IMenuItem>(_order.Items);

            List<IMenuItem> pizzas = items.FindAll(x => x is Pizza);

            // check for two mediums special
            List<IMenuItem> mediums =
                pizzas.FindAll(x => (((Pizza)x).Size == PizzaSize.Medium));

            if (mediums.Count > 1)
            {
                items.Add(new Discount("discount for 2 or more medium pizzas", 2M * mediums.Count));
            }
            else if (pizzas.Count >= 5)
            {
                decimal discountTotalAmount = 0;
                foreach (var pizza in pizzas)
                {
                    discountTotalAmount += pizza.RetailPrice / 10;
                }

                items.Add(new Discount("10% discount for 5 or more pizzas", discountTotalAmount));
            }

            decimal total = 0;

            foreach (var item in items)
            {
                total += item.RetailPrice;
            }

            return total;
        }
    }
}


