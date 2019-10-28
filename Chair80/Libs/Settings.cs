using Chair80.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Libs
{
    public class Settings
    {
        public static List<tbl_setting> AppSetting;
        
        public static void Load()
        {
           using(MainEntities ctx=new MainEntities())
            {
                AppSetting = ctx.tbl_setting.ToList();
            }
        }
        public static string Get(string key,string defaultValue=null)
        {
            Load();
            var setting= AppSetting.FirstOrDefault(a => a.setting_key ==key);
            if (setting != null) return setting.setting_value;
            return defaultValue;
        }
    }
}