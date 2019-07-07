using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Chair80.BLL
{
    public static class GlobalRequestData
    {
        public static string lang {
            get {
                try
                {

                    var RouteData=RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
                    string lang = (string)RouteData.Values["lang"];// ntext.Current.Request.QueryString.GetValues("lang").FirstOrDefault();
                    if (lang == null) lang = "en";
                    return lang;
                }
                catch (Exception)
                {

                    return "en";
                }
            }
        }
    }
}