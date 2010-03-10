

namespace Pizza.Model
{
    public class Beer : IMenuItem
    {
        public string Description
        {
            get { return "Beer"; }
        }

        public decimal RetailPrice
        {
            get { return 4M; }
        }
    }
}