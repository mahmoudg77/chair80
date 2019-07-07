using Chair80.BLL.Security;
using Chair80.Libs;
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

namespace Chair80.Filters
{
    
    public class AuthFilter : ActionFilterAttribute
    {
        public string Title { get; set; }
        public string Method { get; private set; }

        public AuthFilter(string Title, string Method )
        {
            this.Title = Title;
            this.Method = Method;
        }
        public AuthFilter(string Title )
        {
            this.Title = Title;
        }
        public AuthFilter()
        {
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            var loginFilter = new LoginFilter();
            loginFilter.OnActionExecuting(actionContext);


            if(string.IsNullOrEmpty(this.Method))this.Method = actionContext.ActionDescriptor.ActionName; 
            if(string.IsNullOrEmpty(this.Title))this.Title=this.Method ; 

            Users user = new Users();

            var USER_ID = actionContext.Request.Headers.GetValues("AUTH_KEY");
            string key = USER_ID.First();

            Sessions sessions = new Sessions(Guid.Parse(key));
            user = new Users(sessions.Entity.user_id);
            
            string controllername = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            
            if (!user.Allow(controllername, Method))
            {

               throw  new HttpResponseException(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(
                            APIResult<StringContent>.Error(ResponseCode.UserUnauthorized, Locales.Locales.translate("You dont have permission to") + " " + Locales.Locales.translate(Title))
                            ),System.Text.Encoding.UTF8,"application/json"),
                    ReasonPhrase = "Critical Exception",

                });

            }

           
        
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

        }





    }

}