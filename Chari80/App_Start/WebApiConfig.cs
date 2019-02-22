using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Chari80
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            //config.Routes.MapHttpRoute(
            //    name: "ApiWithAction",
            //    routeTemplate: "{controller}/{Action}/{id}",
            //    defaults: new { id = RouteParameter.Optional , Action = RouteParameter.Optional }
            //);
            //config.Routes.MapHttpRoute(
            //    name: "ImageUrl",
            //    routeTemplate: "{controller}/{model}/{model_id}/{size}/{model_tag}-{index}.jpg",
            //    defaults: new { controller = "Image" , action = "Item" }
            //);
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new {id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data"));


        }
    }
}
