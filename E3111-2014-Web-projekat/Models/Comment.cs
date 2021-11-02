using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models
{
    public enum CommentStatus { PENDING, APPROVED, DENIED}
    public class Comment
    {
        public String Id { get; set; }
        public String TouristUsername { get; set; }
        public String TravelPackageId { get; set; }
        public String Content { get; set; }
        public int Rating { get; set; }
        public CommentStatus Status { get; set; }
    }
}