using Chari80Admin.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chari80Admin.Libs
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
        public static string Get(string key)
        {
            var setting= AppSetting.FirstOrDefault(a => a.setting_key ==key);
            if (setting != null) return setting.setting_value;
            return null;
        }
    }
}