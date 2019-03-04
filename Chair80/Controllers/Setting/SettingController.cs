using Chair80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Chair80.DAL;
using System.Data;
using Chair80.Filters;

namespace Chair80.Controllers.Setting
{
    [AppFilter]
    [RoutePrefix("Setting")]
    public class SettingController : ApiController
    {
       
        /// <summary>
        /// Get general setting
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("General")]
        public APIResult<Dictionary<string,object>> General()
        {
                var settings = DataAccess.getData("select setting_key, setting_value from  tbl_setting where display=1");
                
                if (settings == null)
                {
                    return new APIResult<Dictionary<string, object>>(ResultType.fail, null, "API_ERROR_BAD");
                }

            Dictionary<string, object> rows = new Dictionary<string, object>();
            foreach (DataRow item in settings.Rows)
            {
                    rows.Add((string)item["setting_key"], (string)item["setting_value"]);    
            }
            return new APIResult<Dictionary<string, object>>(ResultType.success, rows, "API_SUCCESS");
             
        }

        [HttpPost]
        [Route("ByGroup")]
        public APIResult<Dictionary<string, object>> ByGroup(string group)
        {

            using(MainEntities ctx=new MainEntities())
            {
                var settings = Libs.Settings.AppSetting.Where(a => a.setting_group == group).ToList();
                if (settings == null)
                {
                    return new APIResult<Dictionary<string, object>>(ResultType.fail, null, "API_ERROR_BAD");
                }

                Dictionary<string, object> rows = new Dictionary<string, object>();
                foreach (var item in settings)
                {
                    rows.Add((string)item.setting_key, item.setting_value);
                }
                return new APIResult<Dictionary<string, object>>(ResultType.success, rows, "API_SUCCESS");
            }
        }



        [AuthFilter("Open Setting for edit")]
        [HttpPost]
        [Route("AllForEdit")]
        public APIResult<List<IGrouping<string, tbl_setting>>> AllForEdit()
        {
            using(MainEntities ctx=new MainEntities())
            {
                var settings = ctx.tbl_setting.OrderBy(a=>a.sequance).GroupBy(a=> a.setting_group).ToList();

                if (settings == null)
                {
                    return new APIResult<List<IGrouping<string, tbl_setting>>>(ResultType.fail, null, "API_ERROR_BAD");
                }
                return new APIResult<List<IGrouping<string, tbl_setting>>>(ResultType.success, settings);
            }
        }

        [AuthFilter("Edit Setting")]
        [HttpPost]
        [Route("Edit")]
        public APIResult<List<tbl_setting>> Edit(List<tbl_setting> request)
        {
            using (MainEntities ctx = new MainEntities())
            {
                //var settings = ctx.tbl_setting.Where(a => a.display == true).OrderBy(a => a.setting_group).OrderBy(a => a.setting_key).ToList();
                foreach(var item in request)
                {
                    var entity = ctx.tbl_setting.Find(item.id);

                    entity.setting_value = item.setting_value;
                    ctx.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }

                if (ctx.SaveChanges() ==0)
                {
                    return new APIResult<List<tbl_setting>>(ResultType.fail, null, "API_ERROR_BAD");
                }
                Libs.Settings.Load();

                return new APIResult<List<tbl_setting>>(ResultType.success, request, "API_SUCCESS");
            }
        }
    }
}
