using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models
{
    public class MeetingLocation
    {
        public Address Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}