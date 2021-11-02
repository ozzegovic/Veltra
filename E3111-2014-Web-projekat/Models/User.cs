using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace E3111_2014_Web_projekat.Models
{
    public enum Gender { MALE, FEMALE};
    public enum Role { GUEST, TOURIST, MANAGER, ADMINISTRATOR};
    public enum Status { PENDING, APPROVED, DENIED};

    public class User
    {
        public String Id { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String Name { get; set; }
        public String Lastname { get; set; }
        public Gender UserGender { get; set; }
        public String Email { get; set; }

        [XmlIgnore]
        public DateTime Birthday { get; set; }

        public string OutDate
        {
            get { return Birthday.ToString("dd/MM/yyyy"); }
            set { Birthday = DateTime.ParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture); }
        }

        public Role UserRole { get; set; }
        public List<String> TravelPackages { get; set; }
        public List<String> Reservations { get; set; }
        public Status Status { get; set; }
        public bool IsDeleted { get; set; }

        public User()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}