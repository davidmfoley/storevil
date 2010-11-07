namespace Pizza.Model
{
    public class Soda : IMenuItem
    {
        public string Description
        {
            get { return "Soda"; }
        }

        public decimal RetailPrice
        {
            get { return 1; }
        }
    }
}