using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Chair80Admin.DAL;
using Chair80Admin.Libs;

namespace Chair80Admin.BLL.Security
{
    public class Users 
    {
        public  sec_users Entity { get; set; }
        public string ForceFilter { get; set; }
        public Users(int ID)
        {
            using (MainEntities ctx=new MainEntities())
            {
                this.Entity = ctx.sec_users.Find(ID);
            }
        }
       
        public Users()
        {

        }

        public static Responses.LoginResponse Login(string uName, string pass, NameValueCollection request)
        {
            try
            {
                using (MainEntities ctx = new MainEntities())
                {
                    
                    string md5Pass = General.MD5(pass);
                    var usr = ctx.tbl_accounts.Include("sec_users").FirstOrDefault(f => f.email == uName && f.sec_users.pwd==md5Pass && (f.is_deleted==false || f.is_deleted==null));

                    if (usr != null)
                    {
                        Sessions ses = new Sessions();
                        ses.Entity.user_id = usr.id;
                        ses.Entity.ip = request.Get("REMOTE_ADDR");
                        IPResult iploc = new IPResult();// General.GetResponse("http://ip-api.com/json/" + ses.Entity.ip);

                        ses.Entity.isp = iploc.isp;
                        ses.Entity.lat = iploc.lat;
                        ses.Entity.lon = iploc.lon;
                        ses.Entity.timezone = iploc.timezone;
                        ses.Entity.city = iploc.city;
                        ses.Entity.country = iploc.country;
                        ses.Entity.country_code = iploc.countryCode;
                        ses.Entity.agent = request.Get("HTTP_USER_AGENT");

                        ses.Entity.browser = General.getAgent(ses.Entity.agent).name;
                        ctx.sec_sessions.Add(ses.Entity);

                       ctx.SaveChanges();

                        usr.sec_users.sec_sessions = new List<sec_sessions>() { ses.Entity };
                    }
                    using(DAL.MainEntities itemCtx=new DAL.MainEntities())
                    {
                        if (usr != null)
                        {
                            DAL.tbl_accounts acc = itemCtx.tbl_accounts.FirstOrDefault(a => a.id == usr.id);
                            var c = new Responses.LoginResponse()
                            {
                                account = acc,
                                token = usr.sec_users.sec_sessions.First().id,
                                roles=ctx.sec_users_roles.Include("sec_roles").Where(a=>a.user_id==acc.id).Select(b=>b.sec_roles.role_key).ToArray()
                            };
                            return c;
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {

                return null;
            }                 
        }
       
        public bool Allow(string ControllerName,string ActionName)
        {
            using(MainEntities ctx=new MainEntities())
            {
                var roleIds=ctx.sec_users_roles.Where(a => a.user_id == this.Entity.id).Select(b => b.role_id).ToList();
                var AccessRights = ctx.sec_access_right.Where(a => a.model_name == ControllerName && roleIds.Contains(a.role_id) && a.method_name == ActionName).ToList();

                if (AccessRights.Count > 0)
                {
                    ForceFilter = AccessRights.First().force_filter;
                    //HttpContext.Current.Request.Headers.Add("ForceFilter", ForceFilter);
                    return true;
                }
            }

            return false;
        }

        public bool resetPassword(Guid ResetPwdKey, string NewPassword)
        {
            using (MainEntities ctx = new MainEntities())
            {
                var user = ctx.sec_users.Where(x => x.reset_pwd_token == ResetPwdKey.ToString()).FirstOrDefault();

                if (user != null)
                {
                    user.pwd = General.MD5(NewPassword);
                    user.reset_pwd_token = null;
                    ctx.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool RequestResetPassword_byMail(string Email)
        {
            using (MainEntities ctx = new MainEntities())
            {
                var acc = ctx.tbl_accounts.Where(x => x.email == Email).FirstOrDefault();

                var user = ctx.sec_users.Find(acc.id);


                if (user != null)
                {
                    user.reset_pwd_token = Guid.NewGuid().ToString();

                    ctx.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();

                    StringBuilder mailBody = new StringBuilder();
                    var setting = Libs.Settings.AppSetting.Where(a => a.setting_key == "site_url").FirstOrDefault();
                    string ResetURL = "";
                    if (setting!=null)
                    {
                        ResetURL = setting.setting_value + "/forgot-password/";
                    }
                    else {
                        ResetURL = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
                    }


                    General.SendEmail(Email, "Metookey Reset Password", "Please reset your password by clicking <a href=\"" + ResetURL + user.reset_pwd_token + "\">here</a>");
                    return true;
                }
            }
            return false;
        }

        //public bool RequestResetPassword_bySMS(string Email)
        //{
        //    var user = Model.GetByWhere(x => x.phone == Email).FirstOrDefault();

        //    if (user != null)
        //    {
        //        user.ResetPwdKey = Guid.NewGuid();
        //        Model.Update(user);
                
        //        //send sms with url to reset
        //        return true;
        //    }

        //    return false;
        //}
    }
}
