using Pizza.TestContext;

namespace Pizza.Specifications.Model
{
    public class Discount : IMenuItem
    {
        public Discount(string description, decimal discountAmount)
        {
            Description = description;
            RetailPrice = -discountAmount;
        }

        public string Description
        {
            get;
            private set;
        }

        public decimal RetailPrice
        { get; private set; }


    }
}