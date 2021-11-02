using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.FormData
{
    public class UpdateTravelPackageModel
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String Description { get; set; }
        public String Destination { get; set; }
        public String PackageType { get; set; }
        public String TransportationType { get; set; }
        public int MaxCapacity { get; set; }
        public List<String> AccommodationIds { get; set; }
        public String City { get; set; }
        public String Street { get; set; }
        public String Number { get; set; }
        public int PostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public String Time { get; set; }
        public String Itinerary { get; set; }
        public String Photos { get; set; }

        public bool IsValid()
        {
            bool ret = true;
            if (String.IsNullOrEmpty(Name))
            {
                ret = false;
            }
            else if (String.IsNullOrEmpty(Destination))
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
            else if (MaxCapacity <= 0)
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
            else if (PostalCode <= 0)
            {
                ret = false;
            }
            else if (Latitude <= 0)
            {
                ret = false;
            }
            else if (Longitude <= 0)
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
            else if ((EndDate - StartDate).TotalDays <= 0)
            {
                ret = false;
            }
            return ret;
        }


    }
}