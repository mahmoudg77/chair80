using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Chair80.DAL;
using Chair80.Libs;
using Chair80.Responses;

namespace Chair80.BLL.Security
{
    public class Users 
    {
        public  sec_users Entity { get; set; }
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
                    //ForceFilter = AccessRights.First().force_filter;
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

        public static APIResult<LoginResponse> LoginByPhone(string phone, string otp, NameValueCollection request)
        {
            try
            {
                using (var ctx=new MainEntities())
                {
                    var  dbuser = ctx.tbl_accounts.Include("sec_users").Where(a => a.mobile == phone).FirstOrDefault();
                    if (dbuser == null)
                    {
                        return new APIResult<LoginResponse>(ResultType.fail, null, "Invalid login data !");
                    }

                    tbl_accounts acc = null;
                    
                    acc = ctx.tbl_accounts.FirstOrDefault(a => a.id == dbuser.id);
                   
                    var returned = new LoginResponse { account = acc };
                    IPResult s = new IPResult();

                    string ip = "";
                    string agent = "";
                    IPResult iploc = new IPResult();

                    try
                    {
                        ip = request.Get("REMOTE_ADDR");
                        agent = request.Get("HTTP_USER_AGENT");

                        iploc = General.GetResponse("http://ip-api.com/json/" + ip);
                    }
                    catch (Exception ex)
                    {
                        return new APIResult<LoginResponse>(ResultType.fail, null, ex.Message + "get location ip:" + ip + " agent:" + agent);
                    }

                    try
                    {

                        //&& a.ip == ip && a.agent == agent
                        var userSessions = ctx.sec_sessions.Where(a => a.user_id == dbuser.id && a.end_time == null).FirstOrDefault();
                        if (userSessions == null)
                        {
                            Sessions ses = new Sessions();
                            ses.Entity.user_id = dbuser.id;
                            ses.Entity.ip = ip;
                            ses.Entity.isp = iploc.isp;
                            ses.Entity.lat = iploc.lat;
                            ses.Entity.lon = iploc.lon;
                            ses.Entity.timezone = iploc.timezone;
                            ses.Entity.city = iploc.city;
                            ses.Entity.country = iploc.country;
                            ses.Entity.country_code = iploc.countryCode;
                            ses.Entity.agent = agent;


                            ctx.sec_sessions.Add(ses.Entity);
                            ctx.SaveChanges();

                            dbuser.sec_users.sec_sessions = new List<sec_sessions>() { ses.Entity };
                            returned.token = ses.Entity.id;
                        }
                        else
                        {
                            returned.token = userSessions.id;
                        }

                        returned.roles = ctx.sec_users_roles.Include("sec_roles").Where(a => a.user_id == acc.id).Select(b => b.sec_roles.role_key).ToArray();
                        return new APIResult<LoginResponse>(ResultType.success, returned, "Login Success");

                    }
                    catch (DbEntityValidationException e)
                    {

                        return new APIResult<LoginResponse>(ResultType.fail, null, General.fetchEntityError(e));
                    }
                    catch (Exception ex)
                    {

                        return new APIResult<LoginResponse>(ResultType.fail, null, ex.Message + " Save Session");

                    }
                }
            }
            catch (Exception ex)
            {

                return new APIResult<LoginResponse>(ResultType.fail, null,ex.Message); ;
            }
        }


        public static APIResult<LoginResponse> Register(tbl_accounts acc,string password,string FirebaseUID,NameValueCollection request)
        {
            using (var ctx=new MainEntities())
            {

               var  dbuser = ctx.tbl_accounts.Include("sec_users").Where(a => a.sec_users.firebase_uid == FirebaseUID).FirstOrDefault();
                if (dbuser == null)
                {
                    dbuser = acc;
                     
                    ctx.tbl_accounts.Add(dbuser);
                    try
                    {
                        ctx.SaveChanges();
                        sec_users sec_user = new sec_users();

                        sec_user.pwd = password;
                        sec_user.id = dbuser.id;
                        sec_user.mail_verified = true;
                        sec_user.firebase_uid = FirebaseUID;
                        sec_user.phone_verified = true;
                        ctx.sec_users.Add(sec_user);
                        ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return new APIResult<LoginResponse>(ResultType.fail, null, ex.Message + "save changes1");
                    }

                }
                else
                {
                        return new APIResult<LoginResponse>(ResultType.fail, null, "This user already exists !");
                }

                var returned = new LoginResponse { account = acc };

                var session = GetNewSession(dbuser.sec_users, request);

                if (session.type != ResultType.success) return new APIResult<LoginResponse>(ResultType.fail, null, session.message);

                returned.token = session.data.id;
                returned.roles =  ctx.sec_users_roles.Include("sec_roles").Where(a => a.user_id == acc.id).Select(b => b.sec_roles.role_key).ToArray();

                return new APIResult<LoginResponse>(ResultType.success, returned, "Register sucessfuly !");

            }
        }
        public static APIResult<bool> VerifyMobile(string phone,string otp)
        {
            using (MainEntities ctx = new MainEntities())
            {
                var vm = ctx.sec_mobile_verify.Where(a => a.mobile == phone && a.code == otp).OrderByDescending(a => a.id).FirstOrDefault();
                if (vm == null) return new APIResult<bool>(ResultType.fail, false, "Invalid code or mobile number !!");
                if (vm.is_used == true) return new APIResult<bool>(ResultType.fail, false, "This code is already used !!");
                if (vm.created_at < DateTime.Now.Add(new TimeSpan(0, -10, 0))) return new APIResult<bool>(ResultType.fail, false, "This code expired !!");

                vm.is_used = true;

                ctx.Entry(vm).State = System.Data.Entity.EntityState.Modified;

                if (ctx.SaveChanges() == 0)
                {
                    return new APIResult<bool>(ResultType.fail, false, "API_ERORR_SAVE");

                }
                return new APIResult<bool>(ResultType.success, true, "Mobile verified success!");
            }
        }

        public static APIResult<sec_sessions> GetNewSession(sec_users usr,NameValueCollection request)
        {
            using (var ctx =new MainEntities())
            {
                try
                {
                    var userSessions = ctx.sec_sessions.Where(a => a.user_id == usr.id && a.end_time == null).FirstOrDefault();
                    if (userSessions != null) return new APIResult<sec_sessions>(ResultType.success, userSessions, "User already logon!");

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

                    return new APIResult<sec_sessions>(ResultType.success, ses.Entity, "success");
                }
                catch (Exception ex)
                {

                    return new APIResult<sec_sessions>(ResultType.fail, null, ex.Message);
                }
            }
        }
    }
}
