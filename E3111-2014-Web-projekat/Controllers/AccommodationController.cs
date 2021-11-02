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
    [RoutePrefix("api/Accommodation")]
    public class AccommodationController : ApiController
    {


        [HttpGet]
        [Route("All")]
        // get all accommodations with search
        public HttpResponseMessage Get(string name="", string type="", string pool = "" , string spa = "" ,
            string wifi = "" , string wheelchairaccessible = "", string sortBy="", string sortOrder = "")
        {

            HttpResponseMessage response;
            Tuple<bool, string> check = checkManagerLoggedIn(Request);


            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                response = new HttpResponseMessage(HttpStatusCode.Redirect);
                string redirect = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                response.Headers.Add("FORCE_REDIRECT", redirect);
                return response;
            }

            List<Accommodation> accommodations = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];

            accommodations = accommodations.Where(acc => !acc.IsDeleted).ToList();
            if(!String.IsNullOrEmpty(name))
            {
                accommodations = accommodations.Where(acc => acc.Name.ToLower().Contains(name.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(type) && !type.ToLower().Equals("any"))
            {
                AccommodationType t = (AccommodationType)Enum.Parse(typeof(AccommodationType), type.ToUpper());
                accommodations = accommodations.Where(acc => acc.AccomodationType == t).ToList();
            }
            if (!String.IsNullOrEmpty(wifi))
            {
                if(wifi.ToLower().Equals("on"))
                    accommodations = accommodations.Where(acc => acc.HasWifi).ToList();
            }
            if (!String.IsNullOrEmpty(pool))
            {
                if (wifi.ToLower().Equals("on"))
                    accommodations = accommodations.Where(acc => acc.HasPool).ToList();
            }
            if (!String.IsNullOrEmpty(wheelchairaccessible))
            {
                if (wheelchairaccessible.ToLower().Equals("on"))
                    accommodations = accommodations.Where(acc => acc.WheelchairAccessible).ToList();
            }
            if (!String.IsNullOrEmpty(spa))
            {
                if (spa.ToLower().Equals("on"))
                    accommodations = accommodations.Where(acc => acc.HasSpa).ToList();
            }
            List<AccommodationModel> ret = new List<AccommodationModel>();

            if (accommodations != null)
            {
                foreach (Accommodation a in accommodations)
                {
                    ret.Add(new AccommodationModel(a));

                }

                if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(sortOrder))
                {
                    if (sortBy.ToLower().Equals("name"))
                    {
                        if (sortOrder.ToLower().Equals("asc"))
                            ret = ret.OrderBy(p => p.Name).ToList();
                        else
                            ret = ret.OrderByDescending(p => p.Name).ToList();
                    }
                    else if (sortBy.ToLower().Equals("totalunits"))
                    {
                        if (sortOrder.ToLower().Equals("asc"))
                            ret = ret.OrderBy(p => p.TotalUnits).ToList();
                        else
                            ret = ret.OrderByDescending(p => p.TotalUnits).ToList();
                    }
                    else if (sortBy.ToLower().Equals("availableunits"))
                    {
                        if (sortOrder.ToLower().Equals("asc"))
                            ret = ret.OrderBy(p => p.AvailableUnitsCount).ToList();
                        else
                            ret = ret.OrderByDescending(p => p.AvailableUnitsCount).ToList();
                    }
                }

                response = new HttpResponseMessage(HttpStatusCode.OK);

                response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                            System.Text.Encoding.UTF8, "application/json");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;
            }



            response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string fullyQualifiedUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            //response.Headers.Add("FORCE_REDIRECT", fullyQualifiedUrl);
            return response;

           
        }


        [HttpGet]
        // api/Accommodation/id
        public AccommodationDetailsModel GetAccommodationDetails(string id)
        {

            List<Accommodation> accommodations = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];

            Accommodation accommodation = accommodations.Where(a => a.Id == id && !a.IsDeleted).FirstOrDefault();
            AccommodationDetailsModel ret;

            if (accommodation!=null)
            {
                ret = new AccommodationDetailsModel(accommodation);
                return ret;
            }

            return null;
        }

        [HttpPost]
        [Route("Create")]
        public AccommodationModel CreateAccommodation([FromBody] CreateAccommodationModel model)
        {
            Tuple<bool, string> check = checkManagerLoggedIn(Request);

            if(!ModelState.IsValid)
            {
                return null; 
            }

            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                return null;
            }

            else
            {
                List<Accommodation> accs = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];
                List<AccommodationUnit> units = (List<AccommodationUnit>)HttpContext.Current.Application["AccommodationUnits"];

                int count = accs.Count;
                Accommodation newAccommodation = new Accommodation(model, count + 1);

                accs.Add(newAccommodation);

                foreach(AccommodationUnit u in newAccommodation.AccommodationUnits)
                {
                    units.Add(u);

                }
                Data.SaveAccommodationUnits(units);
                Data.SaveAccommodations(accs);
                HttpContext.Current.Application["Accommodations"] = accs;
                HttpContext.Current.Application["AccommodationUnits"] = units;

                return new AccommodationModel(newAccommodation);
            }

        }

        [HttpPut]
        [Route("Create")]
        public AccommodationModel UpdateAccommodation([FromBody]AccommodationModel model)
        {
            Tuple<bool, string> check = checkManagerLoggedIn(Request);


            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                return null;
            }

            else
            {
                List<Accommodation> accs = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];


                Accommodation updateAcc = accs.Where(a => a.Id == model.Id && !a.IsDeleted).FirstOrDefault();
                
                updateAcc.AccomodationType = (AccommodationType)Enum.Parse(typeof(AccommodationType), model.AccommodationType.ToUpper());
                updateAcc.Name = model.Name;
                updateAcc.Stars = model.Stars;
                updateAcc.HasPool = model.Ammenities.Contains("Pool") ? true : false;
                updateAcc.HasSpa = model.Ammenities.Contains("Spa") ? true : false;
                updateAcc.HasWifi = model.Ammenities.Contains("Wifi") ? true : false;
                updateAcc.WheelchairAccessible = model.Ammenities.Contains("Wheelchair accessible") ? true : false;
                //updateAcc.AccommodationUnits = new List<AccommodationUnit>();

                Data.SaveAccommodations(accs);
                HttpContext.Current.Application["Accommodations"] = accs;

                return new AccommodationModel(updateAcc);
            }

        }

        [HttpDelete]
        [Route("DeleteAccommodation")]
        public IHttpActionResult DeleteAccommodation(string id)
        {
            Tuple<bool, string> check = checkManagerLoggedIn(Request);


            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                return BadRequest();
            }

            else
            {

                List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
                TravelPackage package = packages.Where(p => !p.IsDeleted && p.AccommodationIds.Contains(id)
                                        && p.StartDate>=DateTime.Now).FirstOrDefault();

                if (package == null)
                {
                    List<Accommodation> accs = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];
                    Accommodation updateAcc = accs.Where(a => a.Id == id && !a.IsDeleted).FirstOrDefault();

                    if (updateAcc != null)
                    {
                        updateAcc.IsDeleted = true;
                        Data.SaveAccommodations(accs);
                        HttpContext.Current.Application["Accommodations"] = accs;
                        return Ok();
                    }
                }

                return BadRequest();
            }
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
            found = true; // DELETE LATER 
            username = "admin";
            return new Tuple<bool, string>(found, username);

        }      


    }
}
