using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.ReturnData
{
    public class AccommodationDetailsModel
    {
        public String Id { get; set; }
        public String AccommodationType { get; set; }
        public String Name { get; set; }
        public int Stars { get; set; }
        public List<String> Ammenities { get; set; }
        public List<AccommodationUnitModel> AccommodationUnits { get; set; }

        public AccommodationDetailsModel(Accommodation acc)
        {
            Id = acc.Id;
            AccommodationType = acc.AccomodationType.ToString();
            Name = acc.Name;
            Stars = acc.Stars;
            Ammenities = new List<String>();
            if (acc.HasPool)
            {
                Ammenities.Add("Pool");
            }
            if (acc.HasSpa)
            {
                Ammenities.Add("Spa");
            }
            if (acc.HasWifi)
            {
                Ammenities.Add("Wifi");
            }
            if (acc.WheelchairAccessible)
            {
                Ammenities.Add("Wheelchair accessible");
            }

            AccommodationUnits = new List<AccommodationUnitModel>();
            foreach (AccommodationUnit au in acc.AccommodationUnits)
            {
                AccommodationUnits.Add(new AccommodationUnitModel(au));
            }
        }
    }
}