using E3111_2014_Web_projekat.Models.ReturnData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.FormData
{
    public class CreateAccommodationModel
    {
        [Required]
        public String AccommodationType { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public int Stars { get; set; }
        [Required]
        public List<String> Ammenities { get; set; }
       // public List<CreateAccommodationUnitModel> AccommodationUnits { get; set; }

    }
}