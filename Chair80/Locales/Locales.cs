using Chair80.BLL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Locales
{
    public class Locales
    {
        public static Dictionary<string,Dictionary<string,string>> Datasource { get; set; }

        public static void Load()
        {
           var files= System.IO.Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Locales/"), "*.json");
            Datasource = new Dictionary<string, Dictionary<string, string>>();
           foreach (string path in files)
            {
                var file = new System.IO.FileInfo(path);
                Datasource.Add(file.Name.Replace(".json",""), JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(file.FullName)));

            }
        }

        public static string translate(string strKey,string lang="")
        {
            if (lang == "") lang = GlobalRequestData.lang;

            try
            {
                var locale = Datasource[lang];

                if (locale == null) return strKey;

                string value = locale[strKey];

                if (string.IsNullOrEmpty(value)) return strKey;
                return value;
            }
            catch (Exception)
            {
                return strKey;
            }
        }

    }
}