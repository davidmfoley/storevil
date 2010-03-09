namespace Pizza.Specifications.Model
{
    public class Slice : Specifications.Model.IMenuItem
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