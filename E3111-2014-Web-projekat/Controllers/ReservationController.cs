using E3111_2014_Web_projekat.Models;
using E3111_2014_Web_projekat.Models.FormData;
using E3111_2014_Web_projekat.Models.ReturnData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace E3111_2014_Web_projekat.Controllers
{
    [RoutePrefix("api/Reservation")]
    public class ReservationController : ApiController
    {
        [HttpGet]
        // get all reservations 
        // managers - all reservations for their packages
        // tourists - all reservations they made
        public HttpResponseMessage GetReservations(string TravelPackageId="", string TravelPackageName="",
                                                    string TouristUsername = "", string ReservationStatus="",
                                                    string sortBy = "", string sortOrder = "", string ReservationPeriod="")
        {
            Tuple<bool, string> manager = checkManagerLoggedIn(Request);
            Tuple<bool, string> tourist = checkIfTouristLoggedIn(Request);
            Tuple<bool, string> admin = checkAdminLoggedIn(Request);

            HttpResponseMessage response;
            List<Reservation> reservations = (List<Reservation>)HttpContext.Current.Application["Reservations"];
            List<User> users = (List<User>)HttpContext.Current.Application["Users"];

            //managers see only reservations of packages they created
            if (manager.Item1 && !String.IsNullOrEmpty(manager.Item2))
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                User user = users.FirstOrDefault(u => u.Username == manager.Item2);
                reservations = reservations.Where(r => user.TravelPackages.Any(p => r.TravelPackageId == p)).ToList();


            }
            //tourists see only their reservations
            else if(tourist.Item1 && !String.IsNullOrEmpty(tourist.Item2))
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);                
                reservations = reservations.Where(r => r.TouristUsername == tourist.Item2).ToList();

            }
            //admin see all reservations
            else if(admin.Item1 && !String.IsNullOrEmpty(admin.Item2))
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                
            }
            else
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }


            if(!String.IsNullOrEmpty(TravelPackageId))
            {
                reservations = reservations.Where(p => p.TravelPackageId == TravelPackageId).ToList();
            }
          
            if (!String.IsNullOrEmpty(TouristUsername))
            {
                reservations = reservations.Where(p => p.TouristUsername == TouristUsername).ToList();
            }
           
            if (!String.IsNullOrEmpty(ReservationStatus) && !ReservationStatus.ToLower().Equals("all"))
            {

                ReservationStatus status = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), ReservationStatus.ToUpper());
                reservations = reservations.Where(r => r.Status == status).ToList();
            }
            
            List<ReservationModel> ret = new List<ReservationModel>();

            foreach(Reservation r in reservations)
            {
                ret.Add(new ReservationModel(r));
            }

            //search by name
            if (!String.IsNullOrEmpty(TravelPackageName))
            {
                ret = ret.Where(p => p.TravelPackageName.Contains(TravelPackageName)).ToList();
            }

            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(sortOrder))
            {
                if (sortBy.ToLower().Equals("travelpackagename"))
                {
                    if (sortOrder.ToLower().Equals("asc"))
                        ret = ret.OrderBy(p => p.TravelPackageName).ToList();
                    else
                        ret = ret.OrderByDescending(p => p.TravelPackageName).ToList();
                }
                else if (sortBy.ToLower().Equals("touristusername"))
                {
                    if (sortOrder.ToLower().Equals("asc"))
                        ret = ret.OrderBy(p => p.TouristUsername).ToList();
                    else
                        ret = ret.OrderByDescending(p => p.TouristUsername).ToList();
                }
                else if (sortBy.ToLower().Equals("status"))
                {
                    if (sortOrder.ToLower().Equals("asc"))
                        ret = ret.OrderBy(p => p.Status).ToList();
                    else
                        ret = ret.OrderByDescending(p => p.Status).ToList();
                }
                else if (sortBy.ToLower().Equals("price"))
                {
                    if (sortOrder.ToLower().Equals("asc"))
                        ret = ret.OrderBy(p => p.TotalPrice).ToList();
                    else
                        ret = ret.OrderByDescending(p => p.TotalPrice).ToList();
                }
            }
            // filter by ALL | PAST | CURRENT packages
            if(!String.IsNullOrEmpty(ReservationPeriod))
            {
                List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
                if(ReservationPeriod.ToLower().Equals("past"))
                {
                    // find packages from the past
                    // and take their id
                    // get only those reservations that have id  that is in the list of past package ids
                    List<string> packIds = packages.Where(p => p.EndDate <= DateTime.Now && !p.IsDeleted).Select(p=> p.Id).ToList();
                    ret = ret.Where(r => packIds.Any(p => p == r.TravelPackageId)).ToList();

                }
                else if (ReservationPeriod.ToLower().Equals("future"))
                {
                    // find packages from the future
                    // and take their id
                    // get only those reservations that have id  that is in the list of fuutre package ids
                    List<string> packIds = packages.Where(p => p.StartDate >= DateTime.Now && !p.IsDeleted).Select(p => p.Id).ToList();
                    ret = ret.Where(r => packIds.Any(p => p == r.TravelPackageId)).ToList();

                }
            }
           
            response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                           System.Text.Encoding.UTF8, "application/json");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;

        }

        [HttpGet]
        [Route("Details")]
        public HttpResponseMessage GetReservations(string id)
        {
            Tuple<bool, string> manager = checkManagerLoggedIn(Request);
            Tuple<bool, string> tourist = checkIfTouristLoggedIn(Request);
            Tuple<bool, string> admin = checkAdminLoggedIn(Request);


            HttpResponseMessage response;
            List<Reservation> reservations = (List<Reservation>)HttpContext.Current.Application["Reservations"];
            List<User> users = (List<User>)HttpContext.Current.Application["Users"];


            if (manager.Item1 && !String.IsNullOrEmpty(manager.Item2))
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                User user = users.FirstOrDefault(u => u.Username == manager.Item2);
                reservations = reservations.Where(r => user.TravelPackages.Any(p => r.TravelPackageId == p && r.Id==id)).ToList();


            }
            else if (tourist.Item1 && !String.IsNullOrEmpty(tourist.Item2))
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                reservations = reservations.Where(r => r.TouristUsername == tourist.Item2 && r.Id ==id).ToList();

            }
            else if(admin.Item1 && !String.IsNullOrEmpty(admin.Item2))
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                reservations = reservations.Where(r => r.Id == id).ToList();
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            ReservationDetailsModel ret = null;

            if (reservations.Count == 1)
            {
                ret = new ReservationDetailsModel(reservations.First());
            }

           
            response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                           System.Text.Encoding.UTF8, "application/json");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;

        }




        [HttpPost]
        [Route("Book")]
        // api/Reservation/Book  
        public HttpResponseMessage BookUnit([FromBody] CreateReservationModel model)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            Tuple<bool, string> tourist = checkIfTouristLoggedIn(Request);
            if (!tourist.Item1 || String.IsNullOrEmpty(tourist.Item2))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }


            List<AccommodationUnit> units = (List<AccommodationUnit>)HttpContext.Current.Application["AccommodationUnits"];
            AccommodationUnit unit = units.Where(u => u.Id == model.AccommodationUnit && !u.IsDeleted).FirstOrDefault();

            if (unit==null || !unit.IsAvailable)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
            TravelPackage package = packages.Where(p => p.Id == model.TravelPackage && !p.IsDeleted).FirstOrDefault();

            if (package== null || package.StartDate<= DateTime.Now)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            List<User> users = (List<User>)HttpContext.Current.Application["Users"];
            List<Accommodation> accs = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];
            Accommodation acc = accs.Where(a => a.AccommodationUnits.Any(u => u.Id == unit.Id)).FirstOrDefault();

            User user = users.FirstOrDefault(u => model.TouristUsername == u.Username && !u.IsDeleted && u.Status!=Status.DENIED);
            if (user != null)
            {
                AccommodationUnit au = acc.AccommodationUnits.Where(un => un.Id == unit.Id).FirstOrDefault();
                acc.AccommodationUnits.Remove(au);

                Reservation r = new Reservation();
                List<Reservation> reservations = (List<Reservation>)HttpContext.Current.Application["Reservations"];

                r.Id = "RES_" + (1+reservations.Count).ToString();
                r.Status = model.Status;
                r.TouristUsername = user.Username;
                r.TravelPackageId = package.Id;
                r.AccommodationUnitId = unit.Id;
                r.IsDeleted = false;
                r.TotalPrice = unit.Price * (package.EndDate - package.StartDate).TotalDays;
                user.Reservations.Add(r.Id);
                unit.IsAvailable = false;
                au.IsAvailable = false;

                acc.AccommodationUnits.Add(au);

                reservations.Add(r);

                Data.SaveAccommodations(accs);
                Data.SaveAccommodationUnits(units);
                Data.SaveReservations(reservations);
                Data.SaveUsers(users);

                HttpContext.Current.Application["Accommodations"] = accs;
                HttpContext.Current.Application["Reservations"] = reservations;
                HttpContext.Current.Application["AccommodationUnits"] = units;
                HttpContext.Current.Application["Users"] = users;

                ReservationModel ret = new ReservationModel(r);
                response = new HttpResponseMessage(HttpStatusCode.OK);

                response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                            System.Text.Encoding.UTF8, "application/json");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }


            return response;

        }

        [HttpPut]
        [Route("Cancel")]
        // api/Reservation/Cancel  
        public HttpResponseMessage UpdateBooking([FromBody] EditReservationModel model)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            Tuple<bool, string> tourist = checkIfTouristLoggedIn(Request);
            // not logged in as tourist - cannot book
            if (!tourist.Item1 || String.IsNullOrEmpty(tourist.Item2) || !model.TouristUsername.Equals(tourist.Item2))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            

            List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
            TravelPackage package = packages.Where(p => p.Id == model.TravelPackageId && !p.IsDeleted).FirstOrDefault();

            // package doesnt exist or it started already
            if (package == null || package.StartDate<=DateTime.Now)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            List<User> users = (List<User>)HttpContext.Current.Application["Users"];

            // tourist exists and is not deleted
            User user = users.FirstOrDefault(u => model.TouristUsername == u.Username && !u.IsDeleted);
            if (user != null)
            {
                //find the reservation
                List<Reservation> reservations = (List<Reservation>)HttpContext.Current.Application["Reservations"];

                Reservation reservation = reservations.Where(r => r.Id == model.Id).FirstOrDefault();
                if(reservation!=null)
                {
                    reservation.Status = ReservationStatus.CANCELED;
                    // find the unit and make it available again
                    List<AccommodationUnit> accommodationUnits = (List<AccommodationUnit>)HttpContext.Current.Application["AccommodationUnits"];
                    AccommodationUnit unit = accommodationUnits.Where(u => u.Id == reservation.AccommodationUnitId).FirstOrDefault();

                    if(unit!=null) //else - should never happen
                    {
                        unit.IsAvailable = true;
                        // find accommodation with this unit - one unit is only in one accommodation
                        List<Accommodation> accommodations = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];
                        Accommodation accommodation = accommodations.Where(a => a.AccommodationUnits.Any(au => au.Id == unit.Id)).FirstOrDefault();

                        if(accommodation!=null)
                        {
                            //get the actual unit in accommodation and make it available there too
                            AccommodationUnit replaceUnit = accommodation.AccommodationUnits.Where(au => au.Id == unit.Id).FirstOrDefault();
                            replaceUnit.IsAvailable = true;
                        }
                        Data.SaveAccommodationUnits(accommodationUnits);
                        Data.SaveAccommodations(accommodations);
                        HttpContext.Current.Application["AccommodationUnits"] = accommodationUnits;
                        HttpContext.Current.Application["Accommodations"] = accommodations;

                    }
                }
               
                Data.SaveReservations(reservations);
                HttpContext.Current.Application["Reservations"] = reservations;

                ReservationModel ret = new ReservationModel(reservation);
                response = new HttpResponseMessage(HttpStatusCode.OK);

                response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                            System.Text.Encoding.UTF8, "application/json");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }


            return response;

        }




        private Tuple<bool, string> checkIfTouristLoggedIn(HttpRequestMessage req)
        {
            bool found = false;
            string username = "";
            foreach (CookieHeaderValue chv in req.Headers.GetCookies())
            {
                foreach (CookieState cs in chv.Cookies)
                {
                    if (cs.Name.Equals("loggedIn"))
                    {
                        username = cs.Value;
                        found = true;
                    }
                }
            }
            if (!String.IsNullOrEmpty(username))
            {
                List<User> users = (List<User>)HttpContext.Current.Application["Users"];

                User user = users.FirstOrDefault(u => u.Username == username && !u.IsDeleted);
                if (user == null || user.UserRole != Role.TOURIST || user.Status == Status.DENIED)
                {
                    found = false;
                    username = "";
                }
            }
            return new Tuple<bool, string>(found, username);

        }


        private Tuple<bool, string> checkManagerLoggedIn(HttpRequestMessage req)
        {
            bool found = false;
            string username = "";
            foreach (CookieHeaderValue chv in req.Headers.GetCookies())
            {
                foreach (CookieState cs in chv.Cookies)
                {
                    if (cs.Name.Equals("loggedIn"))
                    {
                        username = cs.Value;
                        found = true;
                    }
                }
            }
            if (!String.IsNullOrEmpty(username))
            {
                List<User> users = (List<User>)HttpContext.Current.Application["Users"];

                User user = users.FirstOrDefault(u => u.Username == username && !u.IsDeleted);
                if (user == null || user.UserRole != Role.MANAGER)
                {
                    found = false;
                    username = "";
                }
            }

            return new Tuple<bool, string>(found, username);

        }

        private Tuple<bool, string> checkAdminLoggedIn(HttpRequestMessage req)
        {
            bool found = false;
            string username = "";
            foreach (CookieHeaderValue chv in req.Headers.GetCookies())
            {
                foreach (CookieState cs in chv.Cookies)
                {
                    if (cs.Name.Equals("loggedIn"))
                    {
                        username = cs.Value;
                        found = true;
                    }
                }
            }
            if (!String.IsNullOrEmpty(username))
            {
                List<User> users = (List<User>)HttpContext.Current.Application["Users"];

                User user = users.FirstOrDefault(u => u.Username == username);
                if (user == null || user.UserRole != Role.ADMINISTRATOR)
                {
                    found = false;
                    username = "";
                }
            }
            return new Tuple<bool, string>(found, username);

        }
    }
}
