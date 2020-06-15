using Microsoft.AspNetCore.Http;
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
        Order PrepareOrder(ApplicationUser user, Cart cart);
    }
}
