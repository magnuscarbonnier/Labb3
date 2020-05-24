using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;
        private ICartService _cartService;
        private IOrderService _orderService;

        public CartController(IProductService productService, UserManager<ApplicationUser> userManager, ICartService cartService, IOrderService orderService)
        {
            _orderService = orderService;
            _productService = productService;
            _userManager = userManager;
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var cart = _cartService.GetCart(userId, HttpContext.Session);
            return View(cart);
        }



        public async Task<IActionResult> Remove(Guid Id)
        {
            var userid = _userManager.GetUserId(User);
            var product = await _productService.GetById(Id);
            var message = _cartService.RemoveItem(userid, product, HttpContext.Session);
            if (message == Lib.CartNotUpdated)
                TempData["Error"] = message;

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Increase(Guid Id)
        {
            var userid = _userManager.GetUserId(User);
            var product = await _productService.GetById(Id);
            var message = _cartService.AddOneItem(userid, product, HttpContext.Session);
            if (message == Lib.CartNotUpdated)
                TempData["Error"] = message;

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Decrease(Guid Id)
        {
            var userid = _userManager.GetUserId(User);
            var product = await _productService.GetById(Id);
            var message = _cartService.RemoveOneItem(userid, product, HttpContext.Session);
            if (message == Lib.CartNotUpdated)
                TempData["Error"] = message;

            return RedirectToAction("index");
        }
        public JsonResult GetCartAmount()
        {
            var userId = _userManager.GetUserId(User);
            var cart = _cartService.GetCart(userId, HttpContext.Session);
            var totalitems = cart.CartItems.Sum(x => x.Quantity);
            return new JsonResult(totalitems);
        }
    }
}