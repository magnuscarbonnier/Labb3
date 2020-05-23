using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Services
{
    public interface ICartService
    {
        Cart GetCart(string userId, ISession session);
        string AddItemToCart(string userId, Product product, ISession session);
        string AddOneItem(string userId, Product product, ISession session);
        string RemoveOneItem(string userId, Product product, ISession session);
        string RemoveItem(string userId, Product product, ISession session);
        //List<OrderItem> CartItemsToOrderItems(List<CartItem> cartItems);
        Order PrepareOrder(ApplicationUser user, Cart cart);
    }
}
