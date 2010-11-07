namespace Pizza.Model
{
    public class Slice : IMenuItem
    {
        public string Description
        {
            get { return "Slice"; }
        }

        public decimal RetailPrice
        {
            get
            {
                return 2.50M;
            }
        }
    }
}