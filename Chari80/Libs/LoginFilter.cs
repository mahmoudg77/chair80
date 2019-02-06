using Chari80.BLL.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Chari80.Libs
{
    public class LoginFilter : ActionFilterAttribute
    {
       
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var USER_ID = HttpContext.Current.Request.Headers.GetValues("AUTH_KEY");

            //// For Debug
            //if (actionContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == "user" && actionContext.ActionDescriptor.ActionName.ToLower()=="logout")
            //{
            //    var i = "";
            //}
            bool Auth = true;
            if (USER_ID == null  )
            {
                Auth = false;
                //return;
            }
            else
            {
                string key =USER_ID.First();
                Guid token;
                if (!Guid.TryParse(key, out token))
                {
                    Auth = false;
                }
                else
                {

                    Sessions sessions = new Sessions(Guid.Parse(key));

                    if (
                        (sessions == null || sessions.Entity == null || sessions.Entity.end_time != null ||
                        sessions.Entity.sec_users == null) ||
                        sessions.Entity.sec_users.tbl_accounts.is_deleted == true)
                    {

                        Auth = false;
                    }
                }
            }

            if (!Auth)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new APIResult<object>(ResultType.fail, USER_ID, "API_ERROR_FORBIDDEN403"))),//Json("{'type':0,'message':'result.error.E403'}"),
                    ReasonPhrase = "Critical Exception",

                });
            }


        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            //Trace.WriteLine(string.Format("Action Method {0} executed at {1}", actionExecutedContext.ActionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString()), "Web API Logs");
        }

        
        
    }

    
    public class AuthFilter: ActionFilterAttribute
    {
        private string _ForceFilterTable = "";
        public AuthFilter(string ForceFilterTable)
        {

            this._ForceFilterTable = ForceFilterTable;

        }
        public AuthFilter()
        {

        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                if (HttpContext.Current.Request.Headers.AllKeys.Contains("ForceFilterTable"))
                {
                    HttpContext.Current.Request.Headers["ForceFilterTable"] = this._ForceFilterTable;
                }
                else
                {
                    HttpContext.Current.Request.Headers.Add("ForceFilterTable", this._ForceFilterTable);
                }
            }
            catch (Exception)
            {


            }

            var USER_ID = actionContext.Request.Headers.GetValues("AUTH_KEY");
            Users user = new Users();

            bool Auth = true;
            if (USER_ID.First() == null)
            {
                Auth = false;
                //return;
            }
            else
            {
                string key = USER_ID.First();
                Guid token;
                if (!Guid.TryParse(key, out token))
                {
                    Auth = false;
                }
                else
                {

                    Sessions sessions = new Sessions(Guid.Parse(key));
                    user = new Users(sessions.Entity.user_id);
                    if (
                        (sessions == null || sessions.Entity == null || sessions.Entity.end_time != null ||
                        sessions.Entity.sec_users == null) ||
                        sessions.Entity.sec_users.tbl_accounts.is_deleted == true)
                    {
                        Auth = false;
                    }
                }
                
            }

            if (!Auth)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new APIResult<int>(ResultType.fail, "API_ERROR_FORBIDDEN404"))),//Json("{'type':0,'message':'result.error.E403'}"),
                    ReasonPhrase = "Critical Exception",

                });
               
            }

            string actionName = actionContext.ActionDescriptor.ActionName;
            //if (actionContext.Request.Method.Method == "POST")
            //{
            //    actionName += "-post";
            //}
            string controllername = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;


            
            if (!user.Allow(controllername, actionName))
            {

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new APIResult<int>(ResultType.fail, "API_ERROR_UNAUTHORIZED"))),//Json("{'type':0,'message':'result.error.E403'}"),
                    ReasonPhrase = "Critical Exception",

                });
                
            }

            try
            {
                //if (HttpContext.Current.Request.Headers.AllKeys.Contains("ForceFilter"))
                //{
                //    HttpContext.Current.Request.Headers["ForceFilter"] = user.ForceFilter;
                //}
                //else
                //{
                //    HttpContext.Current.Request.Headers.Add("ForceFilter", user.ForceFilter);
                //}
            }
            catch (Exception)
            {


            }

           // HttpContext.Current.Request.Headers.Add("ForceFilter", user.ForceFilter);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            //Trace.WriteLine(string.Format("Action Method {0} executed at {1}", actionExecutedContext.ActionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString()), "Web API Logs");
        }



            
       
    }

    public class Functions
    {
        public static List<string> NameSpaceClasses()
        {

            List<string> items = new List<string>();
            Assembly asm = Assembly.GetExecutingAssembly();
            var lst = asm.GetTypes()
                 .Where(type => typeof(ApiController).IsAssignableFrom(type)) ;

            foreach (Type item in lst)
            {
                items.Add( item.Name.Replace("Controller",""));
            }

            return items;

        }
        public static List<string> ClassMethods(string ControllerName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            List<string> items = new List<string>();

            var lst = asm.GetTypes()
                   .Where(type => typeof(ApiController).IsAssignableFrom(type) && type.Name == ControllerName + "Controller") //filter controllers
                   .SelectMany(type => type.GetMethods())
                   .Where(method => method.ReturnType.Name.Contains( "APIResult") && (method.CustomAttributes.Where(a=>a.AttributeType.Name=="AuthFilter").Count()>0 || method.DeclaringType.CustomAttributes.Where(a=>a.AttributeType.Name == "AuthFilter").Count() > 0));//
            foreach (MethodInfo item in lst)
            {
                items.Add(item.Name );
            }

            return items;
        }

    }
}
