using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.FormData
{
    public class CreateReservationModel
    {
        [Required]
        public String TouristUsername { get; set; }
        public ReservationStatus Status { get; set; }
        [Required]
        public String TravelPackage { get; set; }
        [Required]
        public String AccommodationUnit { get; set; }

        public CreateReservationModel() { }

        public CreateReservationModel(Reservation r)
        {
            TouristUsername = r.TouristUsername;
            Status = r.Status;
            TravelPackage = r.TravelPackageId;
            AccommodationUnit = r.AccommodationUnitId;

        }
    }
}