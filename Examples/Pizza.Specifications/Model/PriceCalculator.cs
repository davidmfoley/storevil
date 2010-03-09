using System.Collections.Generic;

namespace Pizza.Specifications.Model
{
    class PriceCalculator
    {
        public decimal CalculateTotal(Specifications.Model.Order _order)
        {

            List<Specifications.Model.IMenuItem> items = new List<Specifications.Model.IMenuItem>(_order.Items);

            List<Specifications.Model.IMenuItem> pizzas = items.FindAll(x => x is Specifications.Model.Pizza);

            // check for two mediums special
            List<Specifications.Model.IMenuItem> mediums =
                pizzas.FindAll(x => (((Specifications.Model.Pizza)x).Size == Specifications.Model.PizzaSize.Medium));

            if (mediums.Count > 1)
            {
                items.Add(new Specifications.Model.Discount("discount for 2 or more medium pizzas", 2M * mediums.Count));
            }
            else if (pizzas.Count >= 5)
            {
                decimal discountTotalAmount = 0;
                foreach (var pizza in pizzas)
                {
                    discountTotalAmount += pizza.RetailPrice / 10;
                }

                items.Add(new Specifications.Model.Discount("10% discount for 5 or more pizzas", discountTotalAmount));
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


