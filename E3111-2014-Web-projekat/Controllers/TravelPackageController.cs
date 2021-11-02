using E3111_2014_Web_projekat.Models;
using E3111_2014_Web_projekat.Models.FormData;
using E3111_2014_Web_projekat.Models.ReturnData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace E3111_2014_Web_projekat.Controllers
{
    [RoutePrefix("api/TravelPackage")]
    public class TravelPackageController : ApiController
    {
        [HttpGet]
        [Route("Current")]
        // get all packages currently available, sorted from closest date
        public IEnumerable<TravelPackageModel> GetAvailableTravelPackages(string sortby = "", string sortorder = "")
        {
            List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
            packages = packages.Where(p => p.StartDate > DateTime.Now && !p.IsDeleted).ToList();
            List<TravelPackageModel> ret = new List<TravelPackageModel>();
            foreach(TravelPackage p in packages)
            {
                ret.Add(new TravelPackageModel(p));
            }
            if (!String.IsNullOrEmpty(sortby) && !String.IsNullOrEmpty(sortorder))
            {
                if (sortby.ToLower().Equals("name"))
                {
                    if (sortorder.ToLower().Equals("asc"))
                        ret = ret.OrderBy(p => p.Name).ToList();
                    else
                        ret = ret.OrderByDescending(p => p.Name).ToList();
                }
                else if (sortby.ToLower().Equals("startdate"))
                {
                    if (sortorder.ToLower().Equals("asc"))
                        ret = ret.OrderBy(p => p.StartDate).ToList();
                    else
                        ret = ret.OrderByDescending(p => p.StartDate).ToList();
                }
                else if (sortby.ToLower().Equals("enddate"))
                {
                    if (sortorder.ToLower().Equals("asc"))
                        ret = ret.OrderBy(p => p.EndDate).ToList();
                    else
                        ret = ret.OrderByDescending(p => p.EndDate).ToList();
                }
            }
            else
            {
                ret = ret.OrderBy(p => p.StartDate).ToList();

            }
            return ret;
        }

        [HttpGet]
        [Route("Past")]
        // get all expired , sorted from closest end date
        public IEnumerable<TravelPackageModel> GetPastTravelPackages(string sortby = "", string sortorder = "")
        {
            List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
            packages = packages.Where(p => p.StartDate <= DateTime.Now && !p.IsDeleted).ToList();
            List<TravelPackageModel> ret = new List<TravelPackageModel>();
            foreach (TravelPackage p in packages)
            {
                ret.Add(new TravelPackageModel(p));
            }

            if (!String.IsNullOrEmpty(sortby) && !String.IsNullOrEmpty(sortorder))
            {
                if (sortby.ToLower().Equals("name"))
                {
                    if (sortorder.ToLower().Equals("asc"))
                        ret = ret.OrderBy(p => p.Name).ToList();
                    else
                        ret = ret.OrderByDescending(p => p.Name).ToList();
                }
                else if (sortby.ToLower().Equals("startdate"))
                {
                    if (sortorder.ToLower().Equals("asc"))
                        ret = ret.OrderBy(p => p.StartDate).ToList();
                    else
                        ret = ret.OrderByDescending(p => p.StartDate).ToList();
                }
                else if (sortby.ToLower().Equals("enddate"))
                {
                    if (sortorder.ToLower().Equals("asc"))
                        ret = ret.OrderBy(p => p.EndDate).ToList();
                    else
                        ret = ret.OrderByDescending(p => p.EndDate).ToList();
                }
            }
            else
            {
                ret = ret.OrderBy(p => p.EndDate).ToList();

            }

            return ret;
        }

        [HttpGet]
        // get api/TravelPackage/id
        // get all accomodations for the travel packege 
        public TravelPackageDetailsModel GetTravelPackageDetails(string id)
        {

            List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
            TravelPackage package = packages.Where(p => p.Id == id && !p.IsDeleted).FirstOrDefault();

            TravelPackageDetailsModel ret = new TravelPackageDetailsModel(package);

            List<Accommodation> accommodations = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];

            accommodations = accommodations.Where(a => package.AccommodationIds.Contains(a.Id)).ToList();
            accommodations = accommodations.Where(a => !a.IsDeleted).ToList();

            List<AccommodationModel> acc = new List<AccommodationModel>();
            foreach (Accommodation a in accommodations)
            {
                acc.Add(new AccommodationModel(a));
            }
            ret.Accommodations = acc;
            ret.CalculateLowestPrice();

            List<User> users = (List<User>)HttpContext.Current.Application["Users"];
            User loggedInUser = null;
            Tuple<bool, string> check = checkLoggedIn(Request);
            if (check.Item1 && !String.IsNullOrEmpty(check.Item2))
            {
                loggedInUser = users.Where(u => u.Username == check.Item2 && !u.IsDeleted).FirstOrDefault();
            }



          
            if (loggedInUser != null)
            {
                if (loggedInUser.UserRole == Role.TOURIST)
                {
                    List<Reservation> reservations = (List<Reservation>)HttpContext.Current.Application["Reservations"];
                    Reservation reservation = reservations.Where(r => r.TravelPackageId == ret.Id && r.TouristUsername== loggedInUser.Username && r.Status!=ReservationStatus.CANCELED).FirstOrDefault();

                    foreach (string resId in loggedInUser.Reservations)
                    {
                        if(reservation!=null)
                        {

                            if (resId == reservation.Id)
                            {
                                ret.Participated = true;
                            }
                        }

                       
                    }
                }
                if (loggedInUser.UserRole == Role.MANAGER)
                {
                    foreach (string travelPackageId in loggedInUser.TravelPackages)
                    {

                        if (travelPackageId == ret.Id)
                        {
                            ret.CreatedByMe = true;
                        }
                         
                    }
                }
            }
            else
            {
                ret.Participated = false;
                ret.CreatedByMe = false;
            }

            

            return ret;
        }

        [HttpGet]
        [Route("CreatedByMe")]
        // get api/TravelPackage
        // get all travel packages created by me
        public IEnumerable<TravelPackageModel> GetCreatedByMe()
        {
            Tuple<bool, string> check = checkManagerLoggedIn(Request);
            List<User> users = (List<User>)HttpContext.Current.Application["Users"];

            User manager = users.Where(u => u.Username == check.Item2 && !u.IsDeleted).FirstOrDefault();
            if(manager!=null)
            {
                List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
                packages = packages.Where(p => manager.TravelPackages.Contains(p.Id) && !p.IsDeleted).ToList();

                List<TravelPackageModel> ret = new List<TravelPackageModel>();
                foreach (TravelPackage p in packages)
                {
                    ret.Add(new TravelPackageModel(p));
                }
                if(ret!=null)
                {
                    ret = ret.OrderBy(p => p.EndDate).ToList();
                    return ret;
                }

            }
            return null;
        }


        [HttpPost]
        [Route("Search")]
        public HttpResponseMessage SearchTravelPackages([FromBody] SearchTravelPackageModel model)
        {

            HttpResponseMessage response;
            List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
            packages = packages.Where(p => !p.IsDeleted).ToList();
            List<User> users = (List<User>)HttpContext.Current.Application["Users"];
            User loggedInUser = null;
            Tuple<bool, string> check = checkLoggedIn(Request);
            if (check.Item1 && check.Item2 != null)
            {
                loggedInUser = users.Where(u => u.Username == check.Item2 && !u.IsDeleted).FirstOrDefault();
            }
            
            if(model!=null)
            {


                if (model.FilterPackages != null)
                {
                    if (model.FilterPackages.Equals("ByUser"))
                    {
                        User manager = users.Where(u => u.Username == check.Item2 && !u.IsDeleted).FirstOrDefault();

                        if (manager != null)
                        {
                            //packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
                            packages = packages.Where(p => manager.TravelPackages.Contains(p.Id) && !p.IsDeleted).ToList();
                            foreach (TravelPackage p in packages)
                            {
                                p.CreatedByMe = true;
                                p.Participated = false;
                            }
                        }
                    }
                    else if (model.FilterPackages.Equals("Past"))
                    {
                        packages = packages.Where(p => p.StartDate <= DateTime.Now && !p.IsDeleted).ToList();

                    }
                    else
                    {
                        packages = packages.Where(p => p.StartDate > DateTime.Now && !p.IsDeleted).ToList();
                     
                    }
                }


                if (!model.FromMaxDate.Equals(DateTime.MinValue))
                {
                    packages = packages.Where(p => p.EndDate >= model.FromMaxDate).ToList();

                }
                if (!model.FromMinDate.Equals(DateTime.MinValue))
                {
                    packages = packages.Where(p => (p.StartDate >= model.FromMinDate)).ToList();
                }

                if (!model.ToMaxDate.Equals(DateTime.MinValue))
                {
                    packages = packages.Where(p => p.EndDate <= model.ToMaxDate).ToList();
                }
                if (!model.ToMinDate.Equals(DateTime.MinValue))
                {
                    packages = packages.Where(p => p.StartDate <= model.ToMinDate).ToList();

                }
                if (String.IsNullOrEmpty(model.Name))
                {
                    packages = packages.Where(p => p.Name.ToLower().Contains(model.Name.ToLower())).ToList();
                }

                if (model.TransportationType != null)
                {
                    if (!model.TransportationType.Equals("All"))
                    {
                        TransportationType t = (TransportationType)Enum.Parse(typeof(TransportationType), model.TransportationType.ToUpper());
                        packages = packages.Where(p => p.TransportationType == t).ToList();
                    }
                }
                if (model.TravelPackageType != null)
                {
                    if (!model.TravelPackageType.Equals("All"))
                    {
                        TravelPackageType t = (TravelPackageType)Enum.Parse(typeof(TravelPackageType), model.TravelPackageType.ToUpper());
                        packages = packages.Where(p => p.PackageType == t).ToList();
                    }
                }

               
            }
            else
            {
                packages = packages.Where(p => p.StartDate >= DateTime.Now && !p.IsDeleted).ToList();

            }
            foreach (TravelPackage p in packages)
            {
                if (loggedInUser != null)
                {
                    if (loggedInUser.UserRole == Role.TOURIST && loggedInUser.Reservations!=null)
                    {
                        foreach (string travelPackageId in loggedInUser.Reservations)
                        {

                            if (travelPackageId == p.Id)
                            {
                                p.Participated = true;
                            }
                           
                        }
                    }
                    if (loggedInUser.UserRole == Role.MANAGER && loggedInUser.TravelPackages!=null) 
                    {
                        foreach (string travelPackageId in loggedInUser.TravelPackages)
                        {

                            if (travelPackageId == p.Id)
                            {
                                p.CreatedByMe = true;
                            }
                          
                        }
                    }
                }
                else
                {
                    p.Participated = false;
                    p.CreatedByMe = false;
                }

            }

            List<TravelPackageModel> ret = new List<TravelPackageModel>();
            foreach (TravelPackage p in packages)
            {
                ret.Add(new TravelPackageModel(p));
            }

            if(model!=null)
            {
                if (!String.IsNullOrEmpty(model.SortBy) && !String.IsNullOrEmpty(model.SortOrder))
                {
                    if (model.SortBy.ToLower().Equals("name"))
                    {
                        if (model.SortOrder.ToLower().Equals("asc"))
                            ret = ret.OrderBy(p => p.Name).ToList();
                        else
                            ret = ret.OrderByDescending(p => p.Name).ToList();
                    }
                    else if (model.SortBy.ToLower().Equals("startdate"))
                    {
                        if (model.SortOrder.ToLower().Equals("asc"))
                            ret = ret.OrderBy(p => p.StartDate).ToList();
                        else
                            ret = ret.OrderByDescending(p => p.StartDate).ToList();
                    }
                    else if (model.SortBy.ToLower().Equals("enddate"))
                    {
                        if (model.SortOrder.ToLower().Equals("asc"))
                            ret = ret.OrderBy(p => p.EndDate).ToList();
                        else
                            ret = ret.OrderByDescending(p => p.EndDate).ToList();
                    }
                }

                else
                {
                    ret = ret.OrderBy(p => p.StartDate).ToList();

                }

            }

            else
            {
                ret = ret.OrderBy(p => p.StartDate).ToList();

            }



            response = new HttpResponseMessage(HttpStatusCode.OK);

            response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                        System.Text.Encoding.UTF8, "application/json");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;

        }

        [HttpPost]
        [Route("Create")]
        public async Task<TravelPackageModel> CreateTravelPackage()
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

                var ctx = HttpContext.Current;
                var root = ctx.Server.MapPath("~/assets/images/packages");
                var provider = new MultipartFormDataStreamProvider(root);

                TravelPackage package = null;
                List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
                List<User> users = (List<User>)HttpContext.Current.Application["Users"];
                User user = users.Where(u => check.Item2 == u.Username).FirstOrDefault();


                try
                {
                    await Request.Content.ReadAsMultipartAsync(provider);

                    CreateTravelPackageModel json = JsonConvert.DeserializeObject<CreateTravelPackageModel>(provider.FormData["data"]);

                    if(json.IsValid())
                    {

                        package = new TravelPackage(json, packages.Count);
                        int imgCount = 0;
                        foreach (var file in provider.FileData)
                        {
                            var name = file.Headers.ContentDisposition.FileName;
                            name = name.Trim('"');
                            if (!String.IsNullOrEmpty(name))
                            {
                                imgCount++;
                                var localFileName = file.LocalFileName;
                                string[] ext = name.Split('.');

                                package.PosterSrc = "assets/images/packages/" + package.Id + "/" + imgCount.ToString() + "." + ext[1];

                                System.IO.Directory.CreateDirectory(ctx.Server.MapPath("~/assets/images/packages/" + package.Id));

                                var filePath = Path.Combine(root, package.Id + "/" + imgCount.ToString() + "." + ext[1]);
                                if (!File.Exists(filePath))
                                {
                                    File.Move(localFileName, filePath);

                                }
                            }

                        }
                        if (imgCount > 0)
                        {
                            user.TravelPackages.Add(package.Id);
                            packages.Add(package);
                            Data.SaveTravelPackages(packages);
                            Data.SaveUsers(users);
                            HttpContext.Current.Application["TravelPackages"] = packages;
                            HttpContext.Current.Application["Users"] = users;
                        }
                        else
                        {
                            return null;
                        }
                       
                    }
                    else
                    {
                        return null;
                    }

                    

                }
                catch (Exception e)
                {

                }


                return new TravelPackageModel(package);
            }        

        }


        [HttpPut]
        [Route("Create")]
        public async Task<HttpResponseMessage> UpdateTravelPackage()
        {
            Tuple<bool, string> check = checkManagerLoggedIn(Request);

            HttpResponseMessage response;

            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            else
            {

                var ctx = HttpContext.Current;
                var root = ctx.Server.MapPath("~/assets/images/packages");
                var provider = new MultipartFormDataStreamProvider(root);

                List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];

                UpdateTravelPackageModel package = null;

                try
                {
                    await Request.Content.ReadAsMultipartAsync(provider);

                    package = JsonConvert.DeserializeObject<UpdateTravelPackageModel>(provider.FormData["data"]);

                    if(package.IsValid())
                    {
                        if (package.Id != null && IsAllowedToEditPackage(check.Item2, package.Id))
                        {
                            int imgCount = 0;

                            TravelPackage editedPackage = packages.Where(p => p.Id == package.Id && !p.IsDeleted).FirstOrDefault();

                            if (package != null && editedPackage != null)
                            {

                                foreach (var file in provider.FileData)
                                {
                                    var name = file.Headers.ContentDisposition.FileName;

                                    name = name.Trim('"');
                                    if (!String.IsNullOrEmpty(name))
                                    {
                                        imgCount++;
                                        var localFileName = file.LocalFileName;
                                        string[] ext = name.Split('.');

                                        package.Photos = "assets/images/packages/" + package.Id + "/" + imgCount.ToString() + "." + ext[1];

                                        System.IO.Directory.CreateDirectory(ctx.Server.MapPath("~/assets/images/packages/" + package.Id));

                                        var filePath = Path.Combine(root, package.Id + "/" + imgCount.ToString() + "." + ext[1]);
                                        if (File.Exists(filePath))
                                        {
                                            File.Delete(filePath);
                                            File.Move(localFileName, filePath);

                                        }
                                        else
                                        {
                                            File.Move(localFileName, filePath);

                                        }

                                    }

                                }


                                editedPackage.Name = package.Name;
                                editedPackage.PackageType = (TravelPackageType)Enum.Parse(typeof(TravelPackageType), package.PackageType.ToUpper());
                                editedPackage.TransportationType = (TransportationType)Enum.Parse(typeof(TransportationType), package.TransportationType.ToUpper());
                                editedPackage.Destination = package.Destination;
                                editedPackage.StartDate = package.StartDate;
                                editedPackage.EndDate = package.EndDate;
                                editedPackage.Location = new MeetingLocation()
                                {
                                    Address = new Address()
                                    {
                                        City = package.City,
                                        Number = package.Number,
                                        PostalCode = package.PostalCode,
                                        Street = package.Street
                                    },
                                    Latitude = package.Latitude,
                                    Longitude = package.Longitude
                                };
                                editedPackage.Time = DateTime.Parse(package.Time);
                                editedPackage.MaxCapacity = package.MaxCapacity;
                                editedPackage.Description = package.Description;
                                editedPackage.Itinerary = package.Itinerary;
                                if (package.Photos != null) //new photo added
                                    editedPackage.PosterSrc = package.Photos;

                                //check if there are reservations for any units that are there already
                                if (editedPackage.AccommodationIds.Count != 0)
                                {
                                    List<Reservation> reservations = (List<Reservation>)HttpContext.Current.Application["Reservations"];
                                    reservations = reservations.Where(r => r.TravelPackageId == editedPackage.Id).ToList();
                                    if (reservations.Count != 0)
                                    {
                                        // someone made a reservation, find the accommodation and dont remove it from the package accommodations
                                        List<Accommodation> accommodations = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];
                                        accommodations = accommodations.Where(a => editedPackage.AccommodationIds.Any(ac => ac == a.Id) && !a.IsDeleted).ToList();
                                        // find units that are reserved and locate the accommodations they belong to
                                        //these accommodations must not be removed from the package
                                        accommodations = accommodations.Where(a => a.AccommodationUnits.Any(u => reservations.Any(r => r.AccommodationUnitId == u.Id))).ToList();
                                        //now that we found the ones we cannot delete, we can reset the package accommmodations list
                                        editedPackage.AccommodationIds = new List<string>();

                                        //add the new ones
                                        foreach (string accId in package.AccommodationIds)
                                        {
                                            if (!editedPackage.AccommodationIds.Contains(accId))
                                            {
                                                editedPackage.AccommodationIds.Add(accId);
                                            }
                                        }
                                        // add the ones that have reservations
                                        foreach (Accommodation a in accommodations)
                                        {
                                            if (!editedPackage.AccommodationIds.Contains(a.Id))
                                            {
                                                editedPackage.AccommodationIds.Add(a.Id);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // no reservations, can completely change accommodations
                                    editedPackage.AccommodationIds = new List<string>();
                                    editedPackage.AccommodationIds = package.AccommodationIds;
                                }


                                Data.SaveTravelPackages(packages);
                                HttpContext.Current.Application["TravelPackages"] = packages;

                                return new HttpResponseMessage(HttpStatusCode.OK);

                            }
                        }
                        else
                        {
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                    }
                  
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    }

                }
                catch (Exception e)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);

                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            }

        }

        [HttpDelete]
        [Route("DeletePackage")]
        public IHttpActionResult DeletePackage(string id)
        {
            Tuple<bool, string> check = checkManagerLoggedIn(Request);


            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                return BadRequest();
            }

            else
            {
                if(IsAllowedToEditPackage(check.Item2, id))
                {
                    List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
                    List<Reservation> reservations = (List<Reservation>)HttpContext.Current.Application["Reservations"];

                    TravelPackage package = packages.Where(p => !p.IsDeleted && p.Id == id).FirstOrDefault();
                    reservations = reservations.Where(r => r.TravelPackageId == package.Id).ToList();
                    if (package != null && reservations.Count == 0)
                    {
                        //no reservations for this package, can delete
                        package.IsDeleted = true;
                        Data.SaveTravelPackages(packages);
                        HttpContext.Current.Application["TravelPackage"] = packages;
                        return Ok();
                    }

                }
                // any other case, cannot delete
                return BadRequest();
            }
        }

        private Tuple<bool, string> checkLoggedIn(HttpRequestMessage req)
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
                if (user == null )
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

        private bool IsAllowedToEditPackage(string username, string packageId)
        {
            List<User> users = (List<User>)HttpContext.Current.Application["Users"];
            User user = users.Where(u => u.Username == username).FirstOrDefault();
            if (user != null && user.TravelPackages.Contains(packageId))
                return true;
            return false;
        }
    }
}
