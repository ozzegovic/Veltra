using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.FormData
{
    public class CreateAccommodationUnitModel
    {
        [Required]
        public String AccommodationId { get; set; }
        [Required]
        public int Capacity { get; set; }
        public List<String> Ammenities { get; set; }
        [Required]
        public double Price { get; set; }
        public String IsAvailable { get; set; }

       
    }
}