using System.Collections.Generic;

namespace Pizza.Specifications.Model
{
    public  class Order
    {
        readonly List<Specifications.Model.IMenuItem> _items = new List<Specifications.Model.IMenuItem>();
        public void Add(Specifications.Model.IMenuItem   item)
        {
            _items.Add(item);
        }

        public IEnumerable<Specifications.Model.IMenuItem> Items
        {
            get
            {
                return _items;
            }
        }
    }
}