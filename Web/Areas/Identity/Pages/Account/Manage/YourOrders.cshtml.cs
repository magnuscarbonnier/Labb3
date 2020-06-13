using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Web.Models;
using Web.Services;

namespace Web.Areas.Identity.Pages.Account.Manage
{
    public class YourOrders : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public YourOrders(IConfiguration configuration, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
            _configuration = configuration;
        }


        public List<Order> Orders = new List<Order>();
        public void OnGet()
        {
            var userId = _userManager.GetUserId(User);
            string token = "";
            if(User.Identity.IsAuthenticated)
            {
                var user = _userManager.GetUserAsync(User);
                token = GenerateJSONWebToken(user.Result);
            }
            var orders = _orderService.GetUserOrders(userId,token);

            if (orders != null)
            {
                Orders = orders.Result.ToList();
            }
        }
        private string GenerateJSONWebToken(ApplicationUser user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, $"{user.Email}")
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Secret")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://www.yogihosting.com",
                audience: "https://www.yogihosting.com",
                expires: DateTime.Now.AddMinutes(3),
                signingCredentials: credentials,
                claims: claims
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
