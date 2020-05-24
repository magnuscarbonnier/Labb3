using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class Cart
    {
        public string CartUserId { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public decimal Total()
        {
            return CartItems.Sum(x => x.Product.Price * x.Quantity);
        }
    }

}
