using E3111_2014_Web_projekat.Models.FormData;
using E3111_2014_Web_projekat.Models.ReturnData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models
{
    public enum AccommodationType { HOTEL, MOTEL, VILLA};

    public class Accommodation
    {
        public String Id { get; set; }
        public AccommodationType AccomodationType { get; set; }
        public String Name { get; set; }
        public int Stars { get; set; }
        public bool HasPool { get; set; }
        public bool HasSpa { get; set; }
        public bool WheelchairAccessible { get; set; }
        public bool HasWifi { get; set; }
        public List<AccommodationUnit> AccommodationUnits { get; set; }
        public bool IsDeleted { get; set; }


        public Accommodation() { }

        public Accommodation(CreateAccommodationModel acc, int id)
        {

            Id = "AC_" + id.ToString();
            AccomodationType = (AccommodationType)Enum.Parse(typeof(AccommodationType), acc.AccommodationType.ToUpper()); 
            Name = acc.Name;
            Stars = acc.Stars;
            if(acc.Ammenities!=null)
            {
                HasPool = acc.Ammenities.Contains("Pool") ? true : false;
                HasSpa = acc.Ammenities.Contains("Spa") ? true : false;
                HasWifi = acc.Ammenities.Contains("Wifi") ? true : false;
                WheelchairAccessible = acc.Ammenities.Contains("Wheelchair accessible") ? true : false;
            }        
            AccommodationUnits = new List<AccommodationUnit>();           
            IsDeleted = false;
        }

        public double GetLowestPrice()
        {
            double ret = 0;
            if (AccommodationUnits.Count != 0)
                ret = AccommodationUnits.Where(au => !au.IsDeleted).Min(a => a.Price);
            return ret;
        }
    }
}