using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.ReturnData
{
    public class UserModel
    {
        public String Id { get; set; }
        public String Username { get; set; }
        public String Name { get; set; }
        public String Lastname { get; set; }
        public String UserGender { get; set; }
        public String Email { get; set; }
        public DateTime Birthday { get; set; }  
        public String UserRole { get; set; }
        public String Status { get; set; }

        public UserModel() { }

        public UserModel(User u)
        {
            Id = u.Id;
            Username = u.Username;
            Name = u.Name;
            Lastname = u.Lastname;
            UserGender = u.UserGender.ToString();
            Email = u.Email;
            Birthday = u.Birthday;
            UserRole = u.UserRole.ToString();
            Status = u.Status.ToString();
        }
    }
}