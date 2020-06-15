﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static Web.Lib;

namespace Web.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        [Display(Name = "Beställningsdatum")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "AnvändarID")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Adress")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Postnummer")]
        public string ZipCode { get; set; }

        [Required]
        [Display(Name = "Stad")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Epost")]
        public string Email { get; set; }

        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        public Status Status { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        [Display(Name = "Totalpris")]
        public decimal Total()
        {
            return OrderItems.Sum(x => x.Price * x.Quantity);
        }
    }
}