using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.ReturnData
{
    public class ReservationModel
    {
        public String Id { get; set; }
        public String TouristUsername { get; set; }
        public String Status { get; set; }
        public String TravelPackageName { get; set; }
        public String TravelPackageId { get; set; }
        public String AccommodationUnitId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalPrice { get; set; }
        public String Photo { get; set; }

        public ReservationModel() { }

        public ReservationModel(Reservation r)
        {
            Id = r.Id;
            TouristUsername = r.TouristUsername;
            Status = r.Status.ToString();
            List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
            TravelPackage package = packages.Where(p => p.Id == r.TravelPackageId).FirstOrDefault();
            TravelPackageName = package.Name;
            TravelPackageId = package.Id;
            StartDate = package.StartDate;
            EndDate = package.EndDate;
            Photo = package.PosterSrc;
            AccommodationUnitId = r.AccommodationUnitId;
            TotalPrice = r.TotalPrice;
        }
    }
}