using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chair80.Controllers
{
    [RoutePrefix("MailBody")]
    public class MailBodyController : Controller
    {
        [Route("RestPassword")]
        [HttpGet]
        // GET: MailBody
        public ActionResult RestPassword(int id,string os="undifined",string browser="undifined")
        {
            using (var ctx=new DAL.MainEntities())
            {
                var user = ctx.sec_users.Include("tbl_accounts").FirstOrDefault(a=>a.id==id);
                if (user == null)
                {
                    return Content("Invalid user data !");
                }
                if (user.reset_pwd_token == null)
                {
                    return Content("Cannot display reset passord email body !");
                }

                ViewBag.name = user.tbl_accounts.first_name;
                ViewBag.url = Libs.General.ApplicationUrl()+ "/Login/ResetPassword?key=" + user.reset_pwd_token;

                ViewBag.os = os;
                ViewBag.browser = browser;
            }

            return View();
        }

        [Route("Invite")]
        [HttpGet]
        public ActionResult Invite(int id, string email,string name,string app_link)
        {

            using (var ctx = new DAL.MainEntities())
            {
                var user = ctx.sec_users.Include("tbl_accounts").FirstOrDefault(a => a.id == id && a.tbl_accounts.active==true);
                if (user == null)
                {
                    return Content("Invalid user data !");
                }
                 

                ViewBag.name = name;
                ViewBag.sender_name = user.tbl_accounts.first_name;

                ViewBag.url = app_link;

               

            }
            return View();
        }

    }
}