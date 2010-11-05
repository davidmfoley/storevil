using System.Collections.Generic;

namespace Pizza.Model
{
    public  class Order
    {
        readonly List<IMenuItem> _items = new List<IMenuItem>();
        public void Add(IMenuItem   item)
        {
            _items.Add(item);
        }

        public IEnumerable<IMenuItem> Items
        {
            get
            {
                return _items;
            }
        }
    }
}