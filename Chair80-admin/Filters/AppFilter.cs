using Chari80Admin.BLL.Security;
using Chari80Admin.Libs;
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

namespace Chari80Admin.Filters
{
    public class AppFilter : ActionFilterAttribute
    {
        public static readonly string[] AppKeys={
                                                "tVbA82DwfCx2QL3UumZV2z8mTZnmzKMDGcjuikccGc933vpAfx2PDitmZmFdAbVF5LkZ9GWRtkq4j8kT",//web
                                                };
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var APP_ID = HttpContext.Current.Request.Headers.GetValues("APP_KEY");
            bool Auth = true;
            if (APP_ID == null  )
            {
                Auth = false;
                //return;
            }
            else
            {
                string key =APP_ID.First();

                if (!AppKeys.Contains(key ))
                {
                     
                    Auth = false;
                }
            }

            if (!Auth)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new APIResult<object>(ResultType.fail, "502 Bad Geteway !!"))),//Json("{'type':0,'message':'result.error.E403'}"),
                    ReasonPhrase = "Critical Exception",

                });
            }


        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            //Trace.WriteLine(string.Format("Action Method {0} executed at {1}", actionExecutedContext.ActionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString()), "Web API Logs");

        }

        
        
    }

   
}
