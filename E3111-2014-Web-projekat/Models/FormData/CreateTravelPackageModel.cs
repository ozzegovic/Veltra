using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.FormData
{
    public class CreateTravelPackageModel
    {
        [Required]
        public String Name { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public String Description { get; set; }
        [Required]
        public String Destination { get; set; }
        [Required]
        public String PackageType { get; set; }
        [Required]
        public String TransportationType { get; set; }
        [Required]
        public int MaxCapacity { get; set; }
        [Required]
        public List<String> AccommodationIds { get; set; }
        [Required]
        public String City { get; set; }
        [Required]
        public String Street { get; set; }
        [Required]
        public String Number { get; set; }
        [Required]
        public int PostalCode { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public String Time { get; set; }
        [Required]
        public String Itinerary { get; set; }

        public bool IsValid()
        {
            bool ret = true;
            if(String.IsNullOrEmpty(Name))
            {
                ret = false;
            }
            else if(String.IsNullOrEmpty(Destination))
            {
                ret = false;
            }
            else if (String.IsNullOrEmpty(Destination))
            {
                ret = false;
            }
            else if (String.IsNullOrEmpty(Description))
            {
                ret = false;
            }
            else if (String.IsNullOrEmpty(PackageType))
            {
                ret = false;
            }
            else if (String.IsNullOrEmpty(TransportationType))
            {
                ret = false;
            }
            else if (MaxCapacity<=0)
            {
                ret = false;
            }
            else if (String.IsNullOrEmpty(City))
            {
                ret = false;
            }
            else if (String.IsNullOrEmpty(Street))
            {
                ret = false;
            }
            else if (String.IsNullOrEmpty(Number))
            {
                ret = false;
            }
            else if (PostalCode<=0)
            {
                ret = false;
            }
            else if (Latitude<=0)
            {
                ret = false;
            }
            else if (Longitude<=0)
            {
                ret = false;
            }
            else if (String.IsNullOrEmpty(Time))
            {
                ret = false;
            }
            else if (String.IsNullOrEmpty(Itinerary))
            {
                ret = false;
            }
            else if (StartDate.Equals(DateTime.MinValue))
            {
                ret = false;
            }
            else if (EndDate.Equals(DateTime.MinValue))
            {
                ret = false;
            }
            else if ((EndDate-StartDate).TotalDays<=0)
            {
                ret = false;
            }
            return ret;
        }
    }
}