using E3111_2014_Web_projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace E3111_2014_Web_projekat
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
       
    }
}
