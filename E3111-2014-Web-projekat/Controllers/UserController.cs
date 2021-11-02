using E3111_2014_Web_projekat.Models;
using E3111_2014_Web_projekat.Models.ReturnData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace E3111_2014_Web_projekat.Controllers
{
    [RoutePrefix("api/Account")]
    public class UserController : ApiController
    {

        [HttpPost]
        [Route("login")]
        public HttpResponseMessage login(LoginModel user)
        {

            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            User retVal = new User();

            List<User> users = (List<User>)HttpContext.Current.Application["Users"];

            retVal = users.FirstOrDefault(u => u.Username == user.Username 
                                         && u.Password == user.Password && !u.IsDeleted);




            if (retVal != null)
            {

                string cookieTxt = "";
                bool found = false;
                foreach (CookieHeaderValue chv in Request.Headers.GetCookies())
                {
                    foreach (CookieState cs in chv.Cookies)
                    {
                        cookieTxt += cs.Name + "->" + cs.Value + "\n";

                        if (cs.Name.Equals("loggedIn") && cs.Value.Equals(retVal.Username))
                        {
                            found = true;
                        }
                    }
                }

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

                if (!found)
                {
                    // nema cookie koji smo mi izmislili -> onda ćemo ga dodati
                    var cookie = new System.Net.Http.Headers.CookieHeaderValue("loggedIn", retVal.Username);
                    cookie.Expires = DateTime.Now.AddDays(1);
                    cookie.Path = "/";

                    var cookieRole = new System.Net.Http.Headers.CookieHeaderValue("role", retVal.UserRole.ToString());
                    cookieRole.Expires = DateTime.Now.AddDays(1);
                    cookieRole.Path = "/";

                    var cookieUserId = new System.Net.Http.Headers.CookieHeaderValue("User", retVal.UserGender.ToString() + "," + retVal.Id.ToString());
                    cookieUserId.Expires = DateTime.Now.AddDays(1);
                    cookieUserId.Path = "/";

                    response.Headers.AddCookies(new System.Net.Http.Headers.CookieHeaderValue[] { cookie, cookieRole, cookieUserId });
                    cookieTxt += "loggedIn" + "->" + retVal.Username;
                }

                response.Content = new StringContent("/Account/login received 'Cookie': " + cookieTxt);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;

            }
            else
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return response;
            }

        }

        // POST api/Account/Register
        [HttpPost]
        [Route("Register")]
        public HttpResponseMessage Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var user = new User()
            {
                Username = model.Username,
                Email = model.Email,
                Name = model.Name,
                Lastname = model.Lastname,
                UserGender = (Gender)Enum.Parse(typeof(Gender), model.UserGender.ToUpper()),
                Password = model.Password,
                Birthday = DateTime.Parse(model.Birthday),
                UserRole = Role.TOURIST,
                Status = Status.APPROVED,
                IsDeleted = false
            };

            List<User> users = (List<User>)HttpContext.Current.Application["Users"];

            var retVal = users.FirstOrDefault(u => u.Username == user.Username);
            if (retVal == null)
            {
                users.Add(user);
                Data.SaveUsers(users);
                HttpContext.Current.Application["Users"] = users;


                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

                string cookieTxt = "";

                var cookie = new System.Net.Http.Headers.CookieHeaderValue("loggedIn", user.Username);
                cookie.Expires = DateTime.Now.AddDays(1);
                cookie.Path = "/";

                var cookieRole = new System.Net.Http.Headers.CookieHeaderValue("role", user.UserRole.ToString());
                cookieRole.Expires = DateTime.Now.AddDays(1);
                cookieRole.Path = "/";

                var cookieUserId = new System.Net.Http.Headers.CookieHeaderValue("User",retVal.UserGender.ToString() + "," + retVal.Id.ToString());
                cookieUserId.Expires = DateTime.Now.AddDays(1);
                cookieUserId.Path = "/";

                response.Headers.AddCookies(new System.Net.Http.Headers.CookieHeaderValue[] { cookie, cookieRole, cookieUserId });

                cookieTxt += "loggedIn" + "->" + user.Username;


                response.Content = new StringContent("/Account/Register received 'Cookie': " + cookieTxt);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;


            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }


        }
        [HttpGet, Route("logout")]
        public HttpResponseMessage Logout()
        {
            string cookieTxt = "";

            Tuple<bool, string> check = IsLoggedIn(Request);


            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Moved);

            if (check.Item1)
            {
                var cookie = new System.Net.Http.Headers.CookieHeaderValue("loggedIn", check.Item2);
                cookie.Expires = DateTime.Now.AddYears(-30);
                cookie.Path = "/";

                var cookieRole = new System.Net.Http.Headers.CookieHeaderValue("role", "");
                cookieRole.Expires = DateTime.Now.AddYears(-30);
                cookieRole.Path = "/";

                var cookieUserId = new System.Net.Http.Headers.CookieHeaderValue("User", "");
                cookieUserId.Expires = DateTime.Now.AddDays(-30);
                cookieUserId.Path = "/";
                response.Headers.AddCookies(new System.Net.Http.Headers.CookieHeaderValue[] { cookie, cookieRole , cookieUserId});
            }

            response.Content = new StringContent("/Account/logout expired 'Cookie': " + cookieTxt);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            string fullyQualifiedUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            response.Headers.Location = new Uri(fullyQualifiedUrl);
            return response;
        }


        [HttpGet]
        [Route("Profile")]
        // get user profile
        public HttpResponseMessage GetProfile()
        {

            Tuple<bool, string> check = IsLoggedIn(Request);


            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            if (check.Item1 && !String.IsNullOrEmpty(check.Item2))
            {
                List<User> users = (List<User>)HttpContext.Current.Application["Users"];

                var retVal = users.FirstOrDefault(u => u.Username == check.Item2 && !u.IsDeleted);
                if (retVal != null)
                {
                    UserModel ret = new UserModel(retVal);

                    response.StatusCode = HttpStatusCode.OK;

                    response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                                System.Text.Encoding.UTF8, "application/json");
                }
            }

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }



        [HttpPost]
        [Route("UpdateProfile")]
        // get user profile
        public HttpResponseMessage UpdateProfile([FromBody]UserModel model)
        {

            Tuple<bool, string> check = IsLoggedIn(Request);


            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            if (check.Item1 && !String.IsNullOrEmpty(check.Item2))
            {
                List<User> users = (List<User>)HttpContext.Current.Application["Users"];

                var retVal = users.FirstOrDefault(u => u.Username == check.Item2 && u.Username == model.Username && !u.IsDeleted);
                if (retVal != null)
                {

                    retVal.Name = model.Name;
                    retVal.Lastname = model.Lastname;
                    retVal.UserGender = (Gender)Enum.Parse(typeof(Gender), model.UserGender.ToUpper());
                    retVal.Email = model.Email;
                    retVal.Birthday = model.Birthday;

                    //cannot change the rest

                    Data.SaveUsers(users);
                    HttpContext.Current.Application["Users"] = users;

                    UserModel ret = new UserModel(retVal);

                    response.StatusCode = HttpStatusCode.OK;

                    response.Content = new StringContent(JsonConvert.SerializeObject(ret),
                                                System.Text.Encoding.UTF8, "application/json");
                }
            }

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }


        private Tuple<bool, string> IsLoggedIn(HttpRequestMessage req)
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
           
            return new Tuple<bool, string>(found, username);

        }

    }


}
