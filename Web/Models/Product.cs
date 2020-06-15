using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Pris")]
        public decimal Price { get; set; }

        [Display(Name = "Bildsökväg")]
        public string ImgSrc { get; set; }
    }
}
