using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public OrderController(IProductService productService, UserManager<ApplicationUser> userManager, IOrderService orderService, ICartService cartService)
        {
            _productService = productService;
            _userManager = userManager;
            _orderService = orderService;
            _cartService = cartService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            CartViewModel vm = new CartViewModel();
            var user = await _userManager.GetUserAsync(User);

            var cart = _cartService.GetCart(user.Id, HttpContext.Session);


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
            var order = await _orderService.GetOrderById(id);

            return View(order);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CartViewModel vm)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index","Cart") ;

            var userId = _userManager.GetUserId(User);
            vm.cart = _cartService.GetCart(userId, HttpContext.Session);
            var order = CartToOrder(vm);

            if (order != null && order.UserId == userId && vm.cart.CartItems.Count > 0)
            {
                var orderid = await _orderService.PlaceOrder(userId, order, HttpContext.Session);

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
    }
}