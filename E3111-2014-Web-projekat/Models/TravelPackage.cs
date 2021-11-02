using E3111_2014_Web_projekat.Models.FormData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace E3111_2014_Web_projekat.Models
{
    public enum TravelPackageType { ALLINCLUSIVE, FULLBOARD, HALFBOARD, BREAKFASTINCLUDED, SELFCATERING };
    public enum TransportationType { BUS, AIRPLANE, BUS_AIRPLANE, INDIVIDUAL, OTHER};

    public class TravelPackage
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public TravelPackageType PackageType { get; set; }
        public TransportationType TransportationType { get; set; }
        public String Destination { get; set; }
        [XmlIgnore]
        public DateTime StartDate { get; set; }
        public string OutStartDate
        {
            get { return StartDate.ToString("dd/MM/yyyy"); }
            set { StartDate = DateTime.ParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture); }
        }

        [XmlIgnore]
        public DateTime EndDate { get; set; }

        public string OutEndDate
        {
            get { return EndDate.ToString("dd/MM/yyyy"); }
            set { EndDate = DateTime.ParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture); }
        }

        public MeetingLocation Location { get; set; }
        [XmlIgnore]
        public DateTime Time { get; set; }

        public string OutTime
        {
            get { return Time.ToString("HH:mm"); }
            set { Time = DateTime.ParseExact(value, "HH:mm", CultureInfo.InvariantCulture); }
        }
        public int MaxCapacity { get; set; }
        public String Description { get; set; }
        public String Itinerary { get; set; }
        public String PosterSrc { get; set; }
        public List<String> AccommodationIds { get; set; }
        public bool IsDeleted { get; set; }
        [XmlIgnore]
        public bool CreatedByMe { get; set; }
        [XmlIgnore]
        public bool Participated { get; set; }

        public TravelPackage()
        {
        }

        public TravelPackage(CreateTravelPackageModel travelPackage, int id)
        {
            Id = "PACK_" + ++id;
            Name = travelPackage.Name;
            PackageType = (TravelPackageType)Enum.Parse(typeof(TravelPackageType), travelPackage.PackageType.ToUpper());
            TransportationType = (TransportationType)Enum.Parse(typeof(TransportationType), travelPackage.TransportationType.ToUpper());
            //TransportationType = TransportationType.AIRPLANE;
            Destination = travelPackage.Destination;
            StartDate = travelPackage.StartDate;
            EndDate = travelPackage.EndDate;
            Location = new MeetingLocation(){ Address = new Address(){ City = travelPackage.City, Number = travelPackage.Number,
                       PostalCode = travelPackage.PostalCode, Street = travelPackage.Street }, Latitude = travelPackage.Latitude, Longitude = travelPackage.Longitude};
            Time = DateTime.Parse(travelPackage.Time);
            MaxCapacity = travelPackage.MaxCapacity;
            Description = travelPackage.Description;
            Itinerary = travelPackage.Itinerary;
            AccommodationIds = new List<string>();
            AccommodationIds = travelPackage.AccommodationIds;
            IsDeleted = false;
        }
    }

   
}