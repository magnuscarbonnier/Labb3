using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Services
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
        public static string CheckUserId(this ISession session, HttpContext httpContext, UserManager<ApplicationUser> userManager)
        {
            var loggedinuserid = userManager.GetUserId(httpContext.User);
            var sessionUserid = session.Get<string>(Lib.SessionKeyUserId);
            string message;

            if (loggedinuserid != null && loggedinuserid == sessionUserid)
            {
                message = "loggedinuser == sessionuser";
            }
            else if (loggedinuserid == null)
            {
                message = "ej inloggad";
            }
            else
            {
                session.Clear();
                session.Set<string>(Lib.SessionKeyUserId, loggedinuserid);
                message = "Session rensad";
            }

            return message;
        }
    }
}
