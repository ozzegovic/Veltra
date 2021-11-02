using E3111_2014_Web_projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace E3111_2014_Web_projekat
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            List<User> users = Data.ReadUsers();
            HttpContext.Current.Application["Users"] = users;

            List<TravelPackage> travelPackages = Data.ReadTravelPackages();
            HttpContext.Current.Application["TravelPackages"] = travelPackages;

            List<Accommodation> accommodations = Data.ReadAccommodations();
            HttpContext.Current.Application["Accommodations"] = accommodations;

            List<AccommodationUnit> accommodationUnits = Data.ReadAccommodationUnits();
            HttpContext.Current.Application["AccommodationUnits"] = accommodationUnits;

            List<Comment> comments = Data.ReadComments();
            HttpContext.Current.Application["Comments"] = comments;


            List<Reservation> reservations = Data.ReadReservations();
            HttpContext.Current.Application["Reservations"] = reservations;
        }


    }
}
