using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models
{
    public enum ReservationStatus { ACTIVE, CANCELED};

    public class Reservation
    {
        public String Id { get; set; }
        public String TouristUsername { get; set; }
        public ReservationStatus Status { get; set; }
        public String TravelPackageId { get; set; }
        public String AccommodationUnitId { get; set; }
        public double TotalPrice { get; set; }
        public bool IsDeleted { get; set; }
    }
}