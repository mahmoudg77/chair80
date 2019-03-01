using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Chari80.Libs
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
        public static List<KeyValuePair<string,string>> ClassMethods(string ControllerName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var items = new List<KeyValuePair<string, string>>();

            string attrName = "AuthFilter";

            var lst = asm.GetTypes()
                   .Where(type => typeof(ApiController).IsAssignableFrom(type) && type.Name == ControllerName + "Controller") //filter controllers
                   .SelectMany(type => type.GetMethods())
                   .Where(method => method.ReturnType.Name.Contains("APIResult") && (method.CustomAttributes.Where(a => a.AttributeType.Name == attrName).Count() > 0 || method.DeclaringType.CustomAttributes.Where(a => a.AttributeType.Name == attrName).Count() > 0));//
            foreach (MethodInfo item in lst)
            {
                var attr = item.CustomAttributes.Where(a => a.AttributeType.Name == attrName).FirstOrDefault();
                if( attr ==null) attr=item.DeclaringType.CustomAttributes.Where(a => a.AttributeType.Name == attrName).FirstOrDefault();


                string v = attr.ConstructorArguments.Count()>0 ? attr.ConstructorArguments.First().Value.ToString(): item.Name;
                string k = attr.ConstructorArguments.Count()>2 ? attr.ConstructorArguments.Last().Value.ToString(): item.Name ;
                items.Add(new KeyValuePair<string, string>( k,v));
            }

            return items;
        }

    }
    
}