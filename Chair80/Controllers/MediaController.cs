using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chair80.Controllers
{
    public class MediaController : Controller
    {
        public ActionResult Index()
        {
            return Content("Media Controller");
        }
    }
}
