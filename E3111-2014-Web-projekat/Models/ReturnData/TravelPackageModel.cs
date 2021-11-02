using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E3111_2014_Web_projekat.Models.ReturnData
{
   
    public class TravelPackageModel
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String PackageType { get; set; }
        public String TransportationType { get; set; }
        public String Destination { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public MeetingLocation Location { get; set; }
        public DateTime Time { get; set; }
        public int MaxCapacity { get; set; }
        public String Description { get; set; }
        public String Itinerary { get; set; }
        public String Photos { get; set; }
        public List<String> AccommodationIds { get; set; }
        public double LowestPrice { get; set; }
        public bool CreatedByMe { get; set; }
        public bool Participated { get; set; }


        public TravelPackageModel() { }
        public TravelPackageModel(TravelPackage travelPackage)
        {
            Id = travelPackage.Id;
            Name = travelPackage.Name;
            PackageType = travelPackage.PackageType.ToString();
            TransportationType = travelPackage.TransportationType.ToString();
            Destination = travelPackage.Destination;
            StartDate = travelPackage.StartDate;
            EndDate = travelPackage.EndDate;
            Location = travelPackage.Location;
            Time = travelPackage.Time;
            MaxCapacity = travelPackage.MaxCapacity;
            Description = travelPackage.Description;
            Itinerary = travelPackage.Itinerary;
            Photos = travelPackage.PosterSrc;
            AccommodationIds = new List<string>();
            LowestPrice = 0;
            List<Accommodation> accs = (List<Accommodation>)HttpContext.Current.Application["Accommodations"];
            foreach (string id in travelPackage.AccommodationIds)
            {
                Accommodation a = accs.Where(ac => ac.Id == id && !ac.IsDeleted).FirstOrDefault();
                List<double> lowestPrices = new List<double>();
                if(a!=null)
                {
                    lowestPrices.Add(a.GetLowestPrice());
                }
                if(lowestPrices.Count!=0)
                    LowestPrice = lowestPrices.Min() * (EndDate-StartDate).TotalDays;
            }
            AccommodationIds = travelPackage.AccommodationIds;

            CreatedByMe = travelPackage.CreatedByMe;
            Participated = travelPackage.Participated;
        }
    }

    
}