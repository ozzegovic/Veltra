using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace E3111_2014_Web_projekat.Models
{
    public class Data
    {
        public static void SaveUsers(List<User> users)
        {
            
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(List<User>));

            var path = @"/App_Data/users.xml";
            string file = HostingEnvironment.MapPath(path);
            FileStream fs = new FileStream(file, FileMode.Create);
            writer.Serialize(fs, users);
            fs.Close();


        }

        public static List<User> ReadUsers()
        {

            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(List<User>));

            var path = @"/App_Data/users.xml";
            string file = HostingEnvironment.MapPath(path);

            FileStream fs = new FileStream(file, FileMode.Open);

            List<User> users = (List<User>)reader.Deserialize(fs);
            fs.Close();

            return users;
        }

        public static void SaveTravelPackages(List<TravelPackage> travelPackages)
        {
            System.Xml.Serialization.XmlSerializer writer =
              new System.Xml.Serialization.XmlSerializer(typeof(List<TravelPackage>));

            var path = @"/App_Data/travel-packages.xml";
            string file = HostingEnvironment.MapPath(path);
            FileStream fs = new FileStream(file, FileMode.Create);
            writer.Serialize(fs, travelPackages);
            fs.Close();

        }

        public static List<TravelPackage> ReadTravelPackages()
        {
            System.Xml.Serialization.XmlSerializer reader =
                 new System.Xml.Serialization.XmlSerializer(typeof(List<TravelPackage>));

            var path = @"/App_Data/travel-packages.xml";
            string file = HostingEnvironment.MapPath(path);

            FileStream fs = new FileStream(file, FileMode.Open);

            List<TravelPackage> packages = (List<TravelPackage>)reader.Deserialize(fs);
            fs.Close();

            return packages;
        }

        public static void SaveAccommodations(List<Accommodation> accommodations)
        {
            System.Xml.Serialization.XmlSerializer writer =
              new System.Xml.Serialization.XmlSerializer(typeof(List<Accommodation>));

            var path = @"/App_Data/accommodations.xml";
            string file = HostingEnvironment.MapPath(path);
            FileStream fs = new FileStream(file, FileMode.Create);
            writer.Serialize(fs, accommodations);
            fs.Close();

        }

        public static List<Accommodation> ReadAccommodations()
        {
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(List<Accommodation>));

            var path = @"/App_Data/accommodations.xml";
            string file = HostingEnvironment.MapPath(path);

            FileStream fs = new FileStream(file, FileMode.Open);

            List<Accommodation> accommodations = (List<Accommodation>)reader.Deserialize(fs);
            fs.Close();

            return accommodations;
        }


        public static void SaveAccommodationUnits(List<AccommodationUnit> units)
        {
            System.Xml.Serialization.XmlSerializer writer =
              new System.Xml.Serialization.XmlSerializer(typeof(List<AccommodationUnit>));

            var path = @"/App_Data/accommodation-units.xml";
            string file = HostingEnvironment.MapPath(path);
            FileStream fs = new FileStream(file, FileMode.Create);
            writer.Serialize(fs, units);
            fs.Close();
        }

        public static List<AccommodationUnit> ReadAccommodationUnits()
        {
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(List<AccommodationUnit>));

            var path = @"/App_Data/accommodation-units.xml";
            string file = HostingEnvironment.MapPath(path);

            FileStream fs = new FileStream(file, FileMode.Open);

            List<AccommodationUnit> units = (List<AccommodationUnit>)reader.Deserialize(fs);
            fs.Close();

            return units;
        }

        public static void SaveComments(List<Comment> comments)
        {
            System.Xml.Serialization.XmlSerializer writer =
              new System.Xml.Serialization.XmlSerializer(typeof(List<Comment>));

            var path = @"/App_Data/comments.xml";
            string file = HostingEnvironment.MapPath(path);
            FileStream fs = new FileStream(file, FileMode.Create);
            writer.Serialize(fs, comments);
            fs.Close();
        }

        public static List<Comment> ReadComments()
        {
            System.Xml.Serialization.XmlSerializer reader =
               new System.Xml.Serialization.XmlSerializer(typeof(List<Comment>));

            var path = @"/App_Data/comments.xml";
            string file = HostingEnvironment.MapPath(path);

            FileStream fs = new FileStream(file, FileMode.Open);

            List<Comment> comments = (List<Comment>)reader.Deserialize(fs);
            fs.Close();

            return comments;
        }

        public static void SaveReservations(List<Reservation> reservations)
        {
            System.Xml.Serialization.XmlSerializer writer =
              new System.Xml.Serialization.XmlSerializer(typeof(List<Reservation>));

            var path = @"/App_Data/reservations.xml";
            string file = HostingEnvironment.MapPath(path);
            FileStream fs = new FileStream(file, FileMode.Create);
            writer.Serialize(fs, reservations);
            fs.Close();
        }

        public static List<Reservation> ReadReservations()
        {
            System.Xml.Serialization.XmlSerializer reader =
               new System.Xml.Serialization.XmlSerializer(typeof(List<Reservation>));

            var path = @"/App_Data/reservations.xml";
            string file = HostingEnvironment.MapPath(path);

            FileStream fs = new FileStream(file, FileMode.Open);

            List<Reservation> reservations = (List<Reservation>)reader.Deserialize(fs);
            fs.Close();

            return reservations;
        }
    }
}