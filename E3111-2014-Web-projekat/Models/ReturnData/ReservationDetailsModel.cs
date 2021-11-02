using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.ReturnData
{
    public class ReservationDetailsModel
    {
        public String Id { get; set; }
        public UserModel User { get; set; }
        public String Status { get; set; }
        public TravelPackageModel TravelPackage { get; set; }
        public AccommodationUnitModel AccommodationUnit { get; set; }
        public AccommodationModel Accommodation { get; set; }
        public double TotalPrice { get; set; }

        public ReservationDetailsModel() { }

        public ReservationDetailsModel(Reservation r)
        {
            Id = r.Id;
            Status = r.Status.ToString();
            List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
            List<User> users = (List<User>)HttpContext.Current.Application["Users"];
            List<AccommodationUnit> accs = (List<AccommodationUnit>)HttpContext.Current.Application["AccommodationUnits"];
            TravelPackage package = packages.Where(p => p.Id == r.TravelPackageId).FirstOrDefault();
            AccommodationUnit accommodation = accs.Where(a => a.Id == r.AccommodationUnitId).FirstOrDefault();
            User user = users.Where(u => u.Username == r.TouristUsername).FirstOrDefault();
            User = new UserModel(user);
            TravelPackage = new TravelPackageModel(package);
            AccommodationUnit = new AccommodationUnitModel(accommodation);
            List<Accommodation> accommodations = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];
            Accommodation acm = accommodations.Where(ac => ac.AccommodationUnits.Any(u => u.Id == accommodation.Id)).FirstOrDefault();
            Accommodation = new AccommodationModel(acm);
            TotalPrice = r.TotalPrice;
        }
    }
}