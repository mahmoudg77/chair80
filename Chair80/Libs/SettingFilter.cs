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
    public class SettingFilter : ActionFilterAttribute
    {
        public string setting { get; set; }
        public SettingFilter(string _setting)
        {
            setting = _setting;
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            if (Libs.Settings.AppSetting.Where(a => a.setting_key == this.setting).FirstOrDefault().setting_value != "1")
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable)
                {
                    Content = new StringContent("API_SETTING_NotAcceptable"),
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
