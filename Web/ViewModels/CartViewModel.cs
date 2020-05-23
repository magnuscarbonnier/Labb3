using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web.ViewModels
{
    public class CartViewModel
    {
        public Order Order { get; set; }
        public Cart cart { get; set; }
    }
}
