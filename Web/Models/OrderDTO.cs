using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Web.Lib;

namespace Web.Models
{
    public class OrderDTO
    {
        public Guid Id { get; set; }

        public DateTime OrderDate { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public Status Status { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public decimal Total()
        {
            return OrderItems.Sum(x => x.Price * x.Quantity);
        }
    }
    public class OrderItem
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImgSrc { get; set; }
        public int Quantity { get; set; }
    }
}
