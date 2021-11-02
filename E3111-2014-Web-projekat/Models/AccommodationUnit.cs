using E3111_2014_Web_projekat.Models.FormData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models
{
    public class AccommodationUnit
    {
        public String Id { get; set; }
        public int Capacity { get; set; }
        public bool PetFriendly { get; set; }
        public double Price { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAvailable { get; set; }
        
        public AccommodationUnit() { }

        public AccommodationUnit(CreateAccommodationUnitModel au, int i)
        {
            Id = au.AccommodationId + "_U_" + i.ToString();
            Capacity = au.Capacity;
            if (au.Ammenities != null)
                PetFriendly = au.Ammenities.Contains("Pet friendly") ? true: false;
            else
                PetFriendly = false;
            Price = au.Price;
            IsAvailable = true;
        }
    }
}