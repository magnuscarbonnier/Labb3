using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Web.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;
        private IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IConfiguration _configuration;

        public OrderController(IConfiguration configuration, IProductService productService, UserManager<ApplicationUser> userManager, IOrderService orderService, ICartService cartService)
        {
            _productService = productService;
            _userManager = userManager;
            _orderService = orderService;
            _cartService = cartService;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            CartViewModel vm = new CartViewModel();
            var user = await _userManager.GetUserAsync(User);

            var cart = _cartService.GetCart(user.Id, HttpContext.Session);

            string token = "";
            if (User.Identity.IsAuthenticated)
            {
                token = GenerateJSONWebToken(user);
            }

            if (cart == null)
            {
                TempData["Error"] = "Lägg till varor i kundvagnen och försök igen...";
                return RedirectToAction("Index", "Home");
            }
            vm.cart = cart;
            var orderresponse = _cartService.PrepareOrder(user, cart);
            if (orderresponse == null)
                TempData["Error"] = Lib.OrderNotAdded;

            vm.Order = orderresponse;
            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["Error"] = Lib.OrderNotGet;
                return RedirectToAction("Index", "Product");
            }
            string token = "";
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                token = GenerateJSONWebToken(user);
            }
            var order = await _orderService.GetOrderById(id, token);

            return View(order);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CartViewModel vm)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Cart");

            var userId = _userManager.GetUserId(User);
            vm.cart = _cartService.GetCart(userId, HttpContext.Session);
            var order = CartToOrder(vm);
            string token = "";
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                token = GenerateJSONWebToken(user);
            }
            if (order != null && order.UserId == userId && vm.cart.CartItems.Count > 0)
            {
                var orderid = await _orderService.PlaceOrder(userId, order, HttpContext.Session, token);

                return RedirectToAction("Details", "Order", new { id = orderid });
            }
            else
                return RedirectToAction("Index", "Cart");
        }

        [Authorize]
        public async Task<IActionResult> Remove(Guid Id)
        {
            var userid = _userManager.GetUserId(User);
            var product = await _productService.GetById(Id);
            var message = _cartService.RemoveItem(userid, product, HttpContext.Session);
            if (message == Lib.CartNotUpdated)
                TempData["Error"] = message;

            return RedirectToAction("index");
        }

        [Authorize]
        public async Task<IActionResult> Increase(Guid Id)
        {
            var userid = _userManager.GetUserId(User);
            var product = await _productService.GetById(Id);
            var message = _cartService.AddOneItem(userid, product, HttpContext.Session);
            if (message == Lib.CartNotUpdated)
                TempData["Error"] = message;

            return RedirectToAction("index");
        }

        [Authorize]
        public async Task<IActionResult> Decrease(Guid Id)
        {
            var userid = _userManager.GetUserId(User);
            var product = await _productService.GetById(Id);
            var message = _cartService.RemoveOneItem(userid, product, HttpContext.Session);
            if (message == Lib.CartNotUpdated)
                TempData["Error"] = message;

            return RedirectToAction("index");
        }

        public Order CartToOrder(CartViewModel vm)
        {
            return new Order
            {
                OrderItems = vm.cart.CartItems.Select(item => new OrderItem
                {
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    Quantity = item.Quantity,
                    ProductId = item.Product.Id
                }).ToList(),
                FirstName = vm.Order.FirstName,
                LastName = vm.Order.LastName,
                City = vm.Order.City,
                ZipCode = vm.Order.ZipCode,
                Address = vm.Order.Address,
                Phone = vm.Order.Phone,
                Email = vm.Order.Email,
                OrderDate = DateTime.Now,
                UserId = vm.Order.UserId,
                Status = Lib.Status.Beställd
            };
        }
        private string GenerateJSONWebToken(ApplicationUser user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, $"{user.Email}")
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Secret")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:44344/",
                audience: "https://localhost:44344/",
                expires: DateTime.Now.AddMinutes(3),
                signingCredentials: credentials,
                claims: claims
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}