using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.ReturnData
{
    public class CommentModel
    {
        public String Id { get; set; }
        public String TouristUsername { get; set; }
        public String TravelPackageId { get; set; }
        public String Content { get; set; }
        public int Rating { get; set; }
        public String Status { get; set; }

        public CommentModel(Comment c)
        {
            Id = c.Id;
            TouristUsername = c.TouristUsername;
            TravelPackageId = c.TravelPackageId;
            Content = c.Content;
            Rating = c.Rating;
            Status = c.Status.ToString();
        }
    }
}