using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.FormData
{
    public class SearchTravelPackageModel
    {
        public DateTime FromMinDate { get; set; }

        public DateTime ToMinDate { get; set; }

        public DateTime FromMaxDate { get;set; }

        public DateTime ToMaxDate { get; set; }

        public String TransportationType { get; set; }

        public String TravelPackageType { get; set; }

        public String Name { get; set; }

        public String SortBy { get; set; }

        public String SortOrder { get; set; }

        public String FilterPackages { get; set; }
    }
}