using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Models
{
    public class Order
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
            return OrderItems.Sum(x => x.ProductPrice * x.Quantity);
        }
    }
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public string ImgSrc { get; set; }
        public int Quantity { get; set; }
        public Order Order { get; set; }
    }
    public enum Status { Beställd = 0, Bekräftad = 1, Packas = 2, Skickad = 3, Avbeställd = 4 }
}
