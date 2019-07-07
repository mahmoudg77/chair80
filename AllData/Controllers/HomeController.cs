using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace AllData.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            List<string> items = new List<string>();
            Assembly asm = Assembly.GetExecutingAssembly();
            var lst = asm.GetTypes()
                 .Where(type => typeof(Controller).IsAssignableFrom(type));

            foreach (Type item in lst)
            {
                items.Add(item.Name.Replace("Controller", ""));
            }
            ViewBag.items = items;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}