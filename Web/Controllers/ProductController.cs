﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Web.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;
        private ICartService _cartService;

        public ProductController(IProductService productService, UserManager<ApplicationUser> userManager, ICartService cartService)
        {
            _productService = productService;
            _userManager = userManager;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            ProductViewModel vm = new ProductViewModel();
            var products = await _productService.GetAll();
            vm.Products = products;
            return View(vm);
        }

        //hit kommer cart.js
        public async Task<IActionResult> AddToCart(Guid Id)
        {
            var product = await _productService.GetById(Id);
            var userId = _userManager.GetUserId(User);
            var message = _cartService.AddItemToCart(userId, product, HttpContext.Session);

            if (message == Lib.CartNotUpdated)
                TempData["Error"] = message;

            //undrar vad den ska redirectas till
            return RedirectToAction("index", "Cart");
        }
    }
}
