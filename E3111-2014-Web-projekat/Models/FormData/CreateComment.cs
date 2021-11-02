using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.FormData
{
    public class CreateComment
    {
        [Required]
        public String TouristUsername { get; set; }
        [Required]
        public String TravelPackageId { get; set; }
        [Required]
        public String Content { get; set; }
        [Required]
        public int Rating { get; set; }
        public CommentStatus Status { get; set; }
    }
}