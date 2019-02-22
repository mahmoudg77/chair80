using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Chari80Admin.Libs
{
    public class Functions
    {
        public static List<string> NameSpaceClasses()
        {

            List<string> items = new List<string>();
            Assembly asm = Assembly.GetExecutingAssembly();
            var lst = asm.GetTypes()
                 .Where(type => typeof(ApiController).IsAssignableFrom(type));

            foreach (Type item in lst)
            {
                items.Add(item.Name.Replace("Controller", ""));
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
                   .Where(method => method.ReturnType.Name.Contains("APIResult") && (method.CustomAttributes.Where(a => a.AttributeType.Name == "AuthFilter").Count() > 0 || method.DeclaringType.CustomAttributes.Where(a => a.AttributeType.Name == "AuthFilter").Count() > 0));//
            foreach (MethodInfo item in lst)
            {
                items.Add(item.Name);
            }

            return items;
        }

    }
}