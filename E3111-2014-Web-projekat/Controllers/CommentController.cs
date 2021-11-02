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
    [RoutePrefix("api/Comment")]
    public class CommentController : ApiController
    {
        // GET: api/Comment
        public HttpResponseMessage Get(string id)
        {
            HttpResponseMessage response;
            List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
            TravelPackage package = packages.Where(p => p.Id == id && !p.IsDeleted && p.EndDate < DateTime.Now).FirstOrDefault();

            if (String.IsNullOrEmpty(id) || package==null )
            {
                // comments for requested id - id not received, package with that id doesnt exist or its deleted or its not expired yet
                // comments were not even allowed to be created
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            Tuple<bool, string> manager = checkManagerLoggedIn(Request);

            List<Comment> comments = (List<Comment>)HttpContext.Current.Application["Comments"];
            List<User> users = (List<User>)HttpContext.Current.Application["Users"];

            if (manager.Item1 && !String.IsNullOrEmpty(manager.Item2))
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                User user = users.Where(u => (u.Username == manager.Item2) && u.TravelPackages.Any(p=> p==id)).FirstOrDefault();
                
                if(user!=null) //this manager created the package, show all comments
                {
                    comments = comments.Where(c=> c.TravelPackageId == id).ToList();

                }
                else //did not create the package, show only approved comments
                {
                    comments = comments.Where(c => c.TravelPackageId == id && c.Status == CommentStatus.APPROVED).ToList();
                }

            }
            else
            {
                //logged in user not a manager  - for anyone alse - unregistered, logged in tourists, admins..
                //show only approved comments
                response = new HttpResponseMessage(HttpStatusCode.OK);
                comments = comments.Where(c => c.TravelPackageId == id && c.Status == CommentStatus.APPROVED).ToList();
            }

            List<CommentModel> ret = new List<CommentModel>();

            foreach (Comment c  in comments)
            {
                ret.Add(new CommentModel(c));
            }
            response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                           System.Text.Encoding.UTF8, "application/json");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;

        }

        [HttpPost]
        [Route("Create")]
        // POST: api/Comment/Create
        public HttpResponseMessage CreateComment([FromBody] CreateComment model)
        {
            HttpResponseMessage response;

            if (!ModelState.IsValid)
            {
                // model not valid 
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            }
            List<TravelPackage> packages = (List<TravelPackage>)HttpContext.Current.Application["TravelPackages"];
            TravelPackage package = packages.Where(p => p.Id == model.TravelPackageId && !p.IsDeleted && p.EndDate < DateTime.Now).FirstOrDefault();

            if (package == null)
            {
                // requested package doesnt exist or is deleted or is not expired yet == cannot create comment
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            }

            Tuple<bool, string> tourist = checkIfTouristLoggedIn(Request); 

            List<Comment> comments = (List<Comment>)HttpContext.Current.Application["Comments"];
            List<User> users = (List<User>)HttpContext.Current.Application["Users"];

            if (tourist.Item1 && !String.IsNullOrEmpty(tourist.Item2))
            {
                // only tourists that went - have active reservation -  can create a comment while logged in
                List<Reservation> reservations = (List<Reservation>)HttpContext.Current.Application["Reservations"];
                reservations = reservations.Where(r => r.TravelPackageId == model.TravelPackageId && r.Status!= ReservationStatus.CANCELED).ToList();
                User user = users.Where(u => (u.Username == tourist.Item2) && reservations.Any(r => r.TouristUsername == u.Username)).FirstOrDefault();

                if (user != null) //this tourist went on the trip, create new comment
                {

                    Comment comment = new Comment();
                    comment.Id = "COM_" + (comments.Count + 1).ToString();
                    comment.TravelPackageId = model.TravelPackageId;
                    comment.TouristUsername = user.Username;
                    comment.Rating = model.Rating;
                    comment.Status = CommentStatus.PENDING;
                    comment.Content = model.Content;

                    comments.Add(comment);
                    Data.SaveComments(comments);
                    HttpContext.Current.Application["Comments"] = comments;

                    response = new HttpResponseMessage(HttpStatusCode.OK);

                }
                else //logged in tourist didnt go to the trip
                {
                    response = new HttpResponseMessage(HttpStatusCode.BadRequest);

                }

            }
            else
            {
                //user not a logged in tourist
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }        

            return response;

        }
        [HttpPut]
        [Route("Create")]
        // POST: api/Comment/Create
        public HttpResponseMessage UpdateComment([FromBody]UpdateCommentModel model)
        {
            HttpResponseMessage response;

            if (!ModelState.IsValid)
            {
                // model not valid 
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            }
            List<Comment> comments = (List<Comment>)HttpContext.Current.Application["Comments"];
            Comment comment = comments.Where(c => c.Id == model.Id).FirstOrDefault();

            if (comment == null)
            {
                // requested comment doesnt exist
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            }
            Tuple<bool, string> manager = checkManagerLoggedIn(Request);

            

            List<User> users = (List<User>)HttpContext.Current.Application["Users"];

            if (manager.Item1 && !String.IsNullOrEmpty(manager.Item2))
            {
                // only managers that created the package can approve or deny the comment
                User user = users.Where(u => (u.Username == manager.Item2) && u.TravelPackages.Any(p => p == comment.TravelPackageId)).FirstOrDefault();

                if (user != null) //this manager created the package, update the status
                {              
                    comment.Status = (CommentStatus)Enum.Parse(typeof(CommentStatus), model.Status.ToUpper());
                    Data.SaveComments(comments);
                    HttpContext.Current.Application["Comments"] = comments;

                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    CommentModel ret = new CommentModel(comment);                 
                    response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                                   System.Text.Encoding.UTF8, "application/json");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
                else //logged in manager didnt crete the package
                {
                    response = new HttpResponseMessage(HttpStatusCode.BadRequest);

                }

            }
            else
            {
                //user not a logged in manager
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
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
                if (user == null || user.UserRole != Role.TOURIST)
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
    }
}
