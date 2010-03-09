namespace Pizza.Specifications.Model
{
    public class Soda : Specifications.Model.IMenuItem
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