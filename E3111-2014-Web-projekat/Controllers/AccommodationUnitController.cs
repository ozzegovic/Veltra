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
    [RoutePrefix("api/AccommodationUnit")]
    public class AccommodationUnitController : ApiController
    {
        [HttpGet]
        // api/AccommodationUnit/id
        public AccommodationUnitModel GetAccommodationUnitDetails(string id)
        {

            List<AccommodationUnit> units = (List<AccommodationUnit>)HttpContext.Current.Application["AccommodationUnits"];

            AccommodationUnit unit = units.Where(a => a.Id == id && !a.IsDeleted).FirstOrDefault();
            AccommodationUnitModel ret;

            if (unit != null)
            {
                ret = new AccommodationUnitModel(unit);
                return ret;
            }

            return null;
        }

        [HttpGet]
        [Route("Added")]
        // api/AccommodationUnit/Added  
        public HttpResponseMessage GetAccommodationUnits(string id, string minCapacity = "" , string maxCapacity = "", 
            string minPrice ="", string maxPrice = "", string petFriendly = "", string sortBy = "", string sortOrder= "")
        {
            HttpResponseMessage response;

            List<Accommodation> accs = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];
            Accommodation ac = accs.Where(a => a.Id == id && !a.IsDeleted).FirstOrDefault();


            if (ac != null)
            {

                List<AccommodationUnit> units = ac.AccommodationUnits.Where(a=>!a.IsDeleted).ToList();

                units = units.Where(u => u.Capacity >= Int32.Parse(String.IsNullOrEmpty(minCapacity) ? "0" : minCapacity) && 
                                        u.Capacity <= Int32.Parse(String.IsNullOrEmpty(maxCapacity) ? "1000" : maxCapacity)).ToList();
                if(String.Equals(petFriendly.ToLower(),"on"))
                {
                    units = units.Where(u => u.PetFriendly).ToList();
                }
                units = units.Where(u => u.Price >= Int32.Parse(String.IsNullOrEmpty(minPrice) ? "0" : minPrice) && 
                                        u.Price<= Int32.Parse(String.IsNullOrEmpty(maxPrice) ? "1000" : maxPrice)).ToList();

                List<AccommodationUnitModel> ret = new List<AccommodationUnitModel>();

                foreach (AccommodationUnit au in units)
                {
                    ret.Add(new AccommodationUnitModel(au));

                }

                if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(sortOrder))
                {
                    if (sortBy.ToLower().Equals("capacity"))
                    {
                        if (sortOrder.ToLower().Equals("asc"))
                            ret = ret.OrderBy(p => p.Capacity).ToList();
                        else
                            ret = ret.OrderByDescending(p => p.Capacity).ToList();
                    }
                    else if (sortBy.ToLower().Equals("price"))
                    {
                        if (sortOrder.ToLower().Equals("asc"))
                            ret = ret.OrderBy(p => p.Price).ToList();
                        else
                            ret = ret.OrderByDescending(p => p.Price).ToList();
                    }                  
                }


                response = new HttpResponseMessage(HttpStatusCode.OK);

                response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                            System.Text.Encoding.UTF8, "application/json");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;
            }

            response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            return response;
        }


        [HttpPost]
        [Route("Create")]
        public HttpResponseMessage CreateAccommodationUnit([FromBody] CreateAccommodationUnitModel model)
        {
            HttpResponseMessage response;

            Tuple<bool, string> check = checkManagerLoggedIn(Request);

            if(!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }


            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            else
            {
                List<Accommodation> accs = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];
                List<AccommodationUnit> units = (List<AccommodationUnit>)HttpContext.Current.Application["AccommodationUnits"];

                int count = accs.Count;
                Accommodation accommodation = accs.Where(a => a.Id == model.AccommodationId && !a.IsDeleted).FirstOrDefault();

                if(accommodation!=null)
                {
                    AccommodationUnit unit = new AccommodationUnit(model, accommodation.AccommodationUnits.Count +1 );
                    accommodation.AccommodationUnits.Add(unit);
                    units.Add(unit);

                    Data.SaveAccommodationUnits(units);
                    Data.SaveAccommodations(accs);
                    HttpContext.Current.Application["Accommodations"] = accs;
                    HttpContext.Current.Application["AccommodationUnits"] = units;

                    AccommodationUnitModel ret = new AccommodationUnitModel(unit);

                    response = new HttpResponseMessage(HttpStatusCode.OK);

                    response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                                System.Text.Encoding.UTF8, "application/json");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    return response;

                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

        }

        [HttpPut]
        [Route("Create")]
        public HttpResponseMessage UpdateAccommodationUnit([FromBody] AccommodationUnitModel model)
        {
            HttpResponseMessage response;

            Tuple<bool, string> check = checkManagerLoggedIn(Request);


            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            else
            {
                List<Accommodation> accs = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];
                List<AccommodationUnit> units = (List<AccommodationUnit>)HttpContext.Current.Application["AccommodationUnits"];

                AccommodationUnit unit = units.Where(a => a.Id == model.Id && !a.IsDeleted).FirstOrDefault();
                // it can be a part of only one accommodation
                Accommodation acc = accs.Where(a => a.AccommodationUnits.Any(u=> u.Id == unit.Id)).FirstOrDefault();

                if (unit != null)
                {
                    AccommodationUnit au = acc.AccommodationUnits.Where(un => un.Id==unit.Id).FirstOrDefault();
                    acc.AccommodationUnits.Remove(au);

                    //unit.Capacity = model.Capacity;
                   

                    List<Reservation> reservations = (List<Reservation>)HttpContext.Current.Application["Reservations"];
                    reservations = reservations.Where(r => r.AccommodationUnitId == unit.Id).ToList();
                    if (reservations.Count != 0)
                    {
                        // do not change beds if there is a reservation that is in the future
                        List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
                        packages = packages.Where(p => reservations.Any(r => r.TravelPackageId == p.Id)).ToList();

                        packages = packages.Where(p => p.StartDate >= DateTime.Now).ToList();

                        if (packages.Count == 0)
                        {
                            unit.Capacity = model.Capacity;

                            if (model.Ammenities != null)
                                unit.PetFriendly = model.Ammenities.Contains("Pet friendly") ? true : false;
                            else
                                unit.PetFriendly = false;
                            unit.Price = model.Price;
                        }
                    }
                    acc.AccommodationUnits.Add(unit);


                    Data.SaveAccommodationUnits(units);
                    Data.SaveAccommodations(accs);
                    HttpContext.Current.Application["Accommodations"] = accs;
                    HttpContext.Current.Application["AccommodationUnits"] = units;

                    AccommodationUnitModel ret = new AccommodationUnitModel(unit);

                    response = new HttpResponseMessage(HttpStatusCode.OK);

                    response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                                System.Text.Encoding.UTF8, "application/json");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    return response;

                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

        }


        [HttpGet]
        [Route("DeleteUnit")]
        public IHttpActionResult DeleteUnit(string id, string AccommodationId)
        {
            Tuple<bool, string> check = checkManagerLoggedIn(Request);


            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                return BadRequest();
            }

            else
            {

                List<Accommodation> accs = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];
                List<AccommodationUnit> units = (List<AccommodationUnit>)HttpContext.Current.Application["AccommodationUnits"];

                AccommodationUnit unit = units.Where(a => a.Id == id && !a.IsDeleted).FirstOrDefault();
                // it can be a part of only one accommodation
                Accommodation acc = accs.Where(a => a.Id == AccommodationId).FirstOrDefault();


                if (unit != null && acc!=null)
                {
                    AccommodationUnit au = acc.AccommodationUnits.Where(u => u.Id == unit.Id).FirstOrDefault();

                    List<Reservation> reservations = (List<Reservation>)HttpContext.Current.Application["Reservations"];
                    reservations = reservations.Where(r => r.AccommodationUnitId == unit.Id).ToList();
                    if(reservations.Count!=0)
                    {
                        // do not delete if there is a reservation that is in the future
                        List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
                        packages = packages.Where(p => reservations.Any(r => r.TravelPackageId == p.Id)).ToList();

                        packages = packages.Where(p => p.StartDate >= DateTime.Now).ToList();

                        if (packages.Count == 0)
                        {
                            unit.IsDeleted = true;
                            au.IsDeleted = true;
                            Data.SaveAccommodationUnits(units);
                            Data.SaveAccommodations(accs);
                            HttpContext.Current.Application["Accommodations"] = accs;
                            HttpContext.Current.Application["AccommodationUnits"] = units;

                            return Ok();
                        }
                    }
                    else
                    {
                        // totally ok to delete :)
                        unit.IsDeleted = true;
                        au.IsDeleted = true;
                        Data.SaveAccommodationUnits(units);
                        Data.SaveAccommodations(accs);
                        HttpContext.Current.Application["Accommodations"] = accs;
                        HttpContext.Current.Application["AccommodationUnits"] = units;

                        return Ok();
                    }
                    

                }

                return BadRequest();
            }
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
                if (user == null || user.UserRole != Role.TOURIST || user.Status== Status.DENIED)
                {
                    found = false;
                    username = "";
                }
            }
            found = true; //DELETE LATER, NOW ITS JUST TO MAKE IT EASIER
            username = "admin";
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
            found = true; //DELETE LATER, NOW ITS JUST TO MAKE IT EASIER
            username = "admin";
            return new Tuple<bool, string>(found, username);

        }
    }
}
