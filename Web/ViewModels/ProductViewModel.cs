using System.Collections.Generic;
using Web.Models;

namespace Web.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
    }
}
