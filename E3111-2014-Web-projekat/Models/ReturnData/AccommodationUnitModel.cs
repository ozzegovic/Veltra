using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.ReturnData
{
    public class AccommodationUnitModel
    {
        public String Id { get; set; }
        public int Capacity { get; set; }
        public List<String> Ammenities { get; set; }
        public double Price { get; set; }
        public bool IsAvailable { get; set; }

        public AccommodationUnitModel() { }

        public AccommodationUnitModel(AccommodationUnit au)
        {
            Id = au.Id;
            Capacity = au.Capacity;
            Ammenities = new List<string>();
            if(au.PetFriendly)
            {
                Ammenities.Add("Pet friendly");
            }
          
            Price = au.Price;
            IsAvailable = au.IsAvailable;
        }
    }
}