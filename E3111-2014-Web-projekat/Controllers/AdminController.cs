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
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        [HttpGet]
        [Route("GetUsers")]
        public HttpResponseMessage Get(string name="", string lastname="", string role = "", string sortBy="", string sortOrder="", string filterBy="")
        {


            Tuple<bool,string> check = checkAdminLoggedIn(Request);
            
            HttpResponseMessage response;

            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                response = new HttpResponseMessage(HttpStatusCode.Redirect);
                string fullyQualifiedUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                response.Headers.Add("FORCE_REDIRECT", fullyQualifiedUrl);
                return response;
            }

            else
            {

                List<User> users = (List<User>)HttpContext.Current.Application["Users"];

                users = users.Where(u => !u.IsDeleted && u.UserRole != Role.ADMINISTRATOR).ToList();
                if(!String.IsNullOrEmpty(name))
                {
                    users = users.Where(u => (u.Name.ToLower().Contains(name.ToLower()))).ToList();

                }
                if (!String.IsNullOrEmpty(lastname))
                {
                    users = users.Where(u => (u.Lastname.ToLower().Contains(lastname.ToLower()))).ToList();

                }
                if (!String.IsNullOrEmpty(role) && !role.ToLower().Equals("all"))
                {
                    Role r = (Role)Enum.Parse(typeof(Role), role.ToUpper());
                    users = users.Where(u => u.UserRole == r).ToList();
                }

                //get users with more than 2 canceled reservations
                List<Reservation> reservations = (List<Reservation>)HttpContext.Current.Application["Reservations"];

                if (!String.IsNullOrEmpty(filterBy))
                {
                    reservations = reservations.Where(r => r.Status == ReservationStatus.CANCELED).ToList();
                    List<string> usernames = reservations.GroupBy(u => u.TouristUsername).Where(x => x.Count() >= 2).Select(x=> x.Key).ToList();
                    if (filterBy.ToLower().Equals("suspicious"))
                        users = users.Where(u => usernames.Any(username=> u.Username==username)).ToList();
                }

                List<UserModel> ret = new List<UserModel>();

                foreach (User u in users)
                {
                    ret.Add(new UserModel(u));
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
                    else if (sortBy.ToLower().Equals("lastname"))
                    {
                        if (sortOrder.ToLower().Equals("asc"))
                            ret = ret.OrderBy(p => p.Lastname).ToList();
                        else
                            ret = ret.OrderByDescending(p => p.Lastname).ToList();
                    }
                    else if (sortBy.ToLower().Equals("role"))
                    {
                        if (sortOrder.ToLower().Equals("asc"))
                            ret = ret.OrderBy(p => p.UserRole).ToList();
                        else
                            ret = ret.OrderByDescending(p => p.UserRole).ToList();
                    }
                }

                response = new HttpResponseMessage(HttpStatusCode.OK);

                response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                            System.Text.Encoding.UTF8, "application/json");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;
            }

        }

        [HttpPut]
        [Route("UpdateRole")]
        public IHttpActionResult UpdateRole([FromBody]UpdateUserRoleModel newValue)
        {
            Tuple<bool, string> check = checkAdminLoggedIn(Request);


            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {               
                return BadRequest();
            }

            else
            {

                List<User> users = (List<User>)HttpContext.Current.Application["Users"];

                User user = users.Where(u => !u.IsDeleted && u.Username ==newValue.Username).FirstOrDefault();

                user.UserRole = (Role)Enum.Parse(typeof(Role), newValue.Role.ToUpper());

                Data.SaveUsers(users);
                HttpContext.Current.Application["Users"] = users;

                return Ok();
            }
        }

        [HttpPut]
        [Route("LockUser")]
        public IHttpActionResult LockUser([FromBody] UpdateUserStatusModel model)
        {
            Tuple<bool, string> check = checkAdminLoggedIn(Request);


            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                return BadRequest();
            }

            else
            {

                List<User> users = (List<User>)HttpContext.Current.Application["Users"];

                User user = users.Where(u => !u.IsDeleted && u.Username == model.Username).FirstOrDefault();

                user.Status = Status.DENIED;

                Data.SaveUsers(users);
                HttpContext.Current.Application["Users"] = users;

                return Ok();
            }
        }


        [HttpDelete]
        [Route("DeleteUser")]
        public IHttpActionResult DeleteUser(string username)
        {
            Tuple<bool, string> check = checkAdminLoggedIn(Request);


            if (!check.Item1 || String.IsNullOrEmpty(check.Item2))
            {
                return BadRequest();
            }

            else
            {

                List<User> users = (List<User>)HttpContext.Current.Application["Users"];

                User user = users.Where(u => !u.IsDeleted && u.Username == username 
                                        && u.UserRole!=Role.ADMINISTRATOR).FirstOrDefault();

                if(user!=null)
                {
                    user.IsDeleted = true;
                    Data.SaveUsers(users);
                    HttpContext.Current.Application["Users"] = users;
                    return Ok();
                }
                

                return BadRequest();
            }
        }

        private Tuple<bool,string> checkAdminLoggedIn(HttpRequestMessage req)
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
            if(!String.IsNullOrEmpty(username))
            {
                List<User> users = (List<User>)HttpContext.Current.Application["Users"];

                User user = users.FirstOrDefault(u => u.Username == username);
                if (user == null || user.UserRole != Role.ADMINISTRATOR)
                {
                    found = false;
                    username = "";
                }
            }
            return new Tuple<bool, string>(found,username);

        }
    }
}
