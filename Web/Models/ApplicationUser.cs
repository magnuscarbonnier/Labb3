﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Display(Name = "Adress")]
        public string Address { get; set; }

        [Display(Name = "Postnummer")]
        public string ZipCode { get; set; }

        [Display(Name = "Stad")]
        public string City { get; set; }

        [Display(Name = "Land")]
        public string Country { get; set; }
    }
}
