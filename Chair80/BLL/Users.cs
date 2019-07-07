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

        public static LoginResponse Login(string uName, string pass, NameValueCollection request)
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
                                password=acc.sec_users.pwd,
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


        public static APIResult<LoginResponse> LoginByPhone(string phone, string otp, NameValueCollection request)
        {
            try
            {
                using (var ctx=new MainEntities())
                {
                    var  dbuser = ctx.tbl_accounts.Include("sec_users").Where(a => a.mobile == phone).FirstOrDefault();
                    if (dbuser == null)
                    {
                        return APIResult<LoginResponse>.Error(ResponseCode.UserNotFound, "Invalid login data !");
                    }

                    //tbl_accounts acc = null;
                    
                    //acc = ctx.tbl_accounts.FirstOrDefault(a => a.id == dbuser.id);
                   
                    var returned = new LoginResponse { account = dbuser };
                   
                    try
                    {

                        var session =GetNewSession(dbuser.sec_users, request, 1);
                        if (!session.isSuccess)
                        {
                            return APIResult<LoginResponse>.Error(session.code, session.message);
                        }
                        var userSessions = session.data;

                        returned.token = userSessions.id;
                        returned.password = dbuser.sec_users.pwd;
                        returned.roles = ctx.sec_users_roles.Include("sec_roles").Where(a => a.user_id == dbuser.id).Select(b => b.sec_roles.role_key).ToArray();
                        return  APIResult<LoginResponse>.Success( returned, "Login Success");

                    }
                    catch (DbEntityValidationException e)
                    {

                        return  APIResult<LoginResponse>.Error(ResponseCode.BackendInternalServer,General.fetchEntityError(e));
                    }
                    catch (Exception ex)
                    {

                        return  APIResult<LoginResponse>.Error(ResponseCode.BackendInternalServer, ex.Message + " Save Session");

                    }
                }
            }
            catch (Exception ex)
            {

                return APIResult<LoginResponse>.Error(ResponseCode.BackendInternalServer, ex.Message); ;
            }
        }

        internal static APIResult<LoginResponse> LoginByEmail(string email, string password, NameValueCollection serverVariables)
        {
            try
            {
                using (var ctx = new MainEntities())
                {
                    var dbuser = ctx.tbl_accounts.Include("sec_users").Where(a => a.email == email).FirstOrDefault();
                    if (dbuser == null || dbuser.sec_users.pwd!=password)
                    {
                        return APIResult<LoginResponse>.Error(ResponseCode.UserNotFound, "Invalid login data !");
                    }

                    //tbl_accounts acc = null;

                    //acc = ctx.tbl_accounts.FirstOrDefault(a => a.id == dbuser.id);

                    var returned = new LoginResponse { account = dbuser };

                    try
                    {

                        var session = GetNewSession(dbuser.sec_users, serverVariables, 2);
                        if (!session.isSuccess)
                        {
                            return APIResult<LoginResponse>.Error(session.code, session.message);
                        }
                        var userSessions = session.data;

                        returned.token = userSessions.id;
                        returned.password = password;
                        returned.roles = ctx.sec_users_roles.Include("sec_roles").Where(a => a.user_id == dbuser.id).Select(b => b.sec_roles.role_key).ToArray();
                        return APIResult<LoginResponse>.Success(returned, "Login Success");

                    }
                    catch (DbEntityValidationException e)
                    {

                        return APIResult<LoginResponse>.Error(ResponseCode.BackendInternalServer, General.fetchEntityError(e));
                    }
                    catch (Exception ex)
                    {

                        return APIResult<LoginResponse>.Error(ResponseCode.BackendInternalServer, ex.Message + " Save Session");

                    }
                }
            }
            catch (Exception ex)
            {

                return APIResult<LoginResponse>.Error(ResponseCode.BackendInternalServer, ex.Message); ;
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
                        return APIResult<LoginResponse>.Error(ResponseCode.BackendDatabase, ex.Message + "save changes1");
                    }

                }
                else
                {
                        return APIResult<LoginResponse>.Error(ResponseCode.BackendDatabase, "This user already exists !");
                }

                var returned = new LoginResponse { account = acc };

                var session = GetNewSession(dbuser.sec_users, request,1);

                if (session.code != ResponseCode.Success) return APIResult<LoginResponse>.Error(session.code, session.message);

                returned.token = session.data.id;
                returned.roles =  ctx.sec_users_roles.Include("sec_roles").Where(a => a.user_id == acc.id).Select(b => b.sec_roles.role_key).ToArray();

                return APIResult<LoginResponse>.Success(returned, "Register sucessfuly !");

            }
        }
        public static APIResult<MobileVerifyResponse> VerifyMobile(string phone,string otp)
        {
            using (MainEntities ctx = new MainEntities())
            {
                var vm = ctx.sec_mobile_verify.Where(a => a.mobile == phone && a.code == otp).OrderByDescending(a => a.id).FirstOrDefault();
                if (vm == null) return APIResult<MobileVerifyResponse>.Error(ResponseCode.UserNotFound, "Invalid code or mobile number !!",new MobileVerifyResponse() { is_verified=false});
                if (vm.is_used == true) return APIResult<MobileVerifyResponse>.Error(ResponseCode.UserNotFound, "This code is already used !!", new MobileVerifyResponse() { is_verified = false });
                if (vm.created_at < DateTime.Now.Add(new TimeSpan(0, -10, 0))) return APIResult<MobileVerifyResponse>.Error(ResponseCode.UserNotFound, "This code expired !!", new MobileVerifyResponse() { is_verified = false });

                vm.is_used = true;
                Guid guid= Guid.NewGuid();
                vm.verification_id = guid;
                ctx.Entry(vm).State = System.Data.Entity.EntityState.Modified;

                if (ctx.SaveChanges() == 0)
                {
                    return APIResult<MobileVerifyResponse>.Error(ResponseCode.BackendDatabase, "API_ERORR_SAVE", new MobileVerifyResponse() { is_verified = false });

                }
                return APIResult<MobileVerifyResponse>.Success(new MobileVerifyResponse() { is_verified = true,verification_id= guid }, "Mobile verified success!");
            }
        }

        public static APIResult<bool> MobileVerified(string phone,Guid verification_id)
        {
            using (MainEntities ctx = new MainEntities())
            {

                var expiredDate = DateTime.Now.Add(new TimeSpan(0, -10, 0));

                var vm = ctx.sec_mobile_verify.Where(a => a.mobile == phone  && a.verification_id==verification_id && a.created_at> expiredDate && a.is_used==true );
               
                if(vm.Count()>0)
                    return APIResult<bool>.Success(true);
                else
                    return APIResult<bool>.Success(false);


            }
        }


        public static APIResult<sec_sessions> GetNewSession(sec_users usr,NameValueCollection request,int platform=1)
        {
            using (var ctx =new MainEntities())
            {
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
                   // return APIResult<sec_sessions>.Error(ResponseCode.BackendServerRequest, ex.Message + "get location ip:" + ip + " agent:" + agent);
                }
                try
                {
                    var userSessions = ctx.sec_sessions.Where(a => a.user_id == usr.id && a.end_time == null && a.paltform==platform).FirstOrDefault();
                    if (userSessions != null) return APIResult<sec_sessions>.Success(userSessions, "User already logon!");

                    Sessions ses = new Sessions();
                    ses.Entity.user_id = usr.id;
                    ses.Entity.ip = request.Get("REMOTE_ADDR");
                    //IPResult iploc = new IPResult();// General.GetResponse("http://ip-api.com/json/" + ses.Entity.ip);

                    ses.Entity.isp = iploc.isp;
                    ses.Entity.lat = iploc.lat;
                    ses.Entity.lon = iploc.lon;
                    ses.Entity.timezone = iploc.timezone;
                    ses.Entity.city = iploc.city;
                    ses.Entity.country = iploc.country;
                    ses.Entity.country_code = iploc.countryCode;
                    ses.Entity.agent = request.Get("HTTP_USER_AGENT");
                    ses.Entity.paltform = platform;
                    ses.Entity.browser = General.getAgent(ses.Entity.agent).name;
                    ctx.sec_sessions.Add(ses.Entity);

                    ctx.SaveChanges();

                    return APIResult<sec_sessions>.Success( ses.Entity, "success");
                }
                catch (Exception ex)
                {

                    return APIResult<sec_sessions>.Error(ResponseCode.BackendDatabase, ex.Message);
                }
            }
        }

        public bool hasRole(string role)
        {
            using (var ctx=new MainEntities())
            {
                return ctx.sec_users_roles.Include("sec_roles").Where(a => a.user_id == this.Entity.id && a.sec_roles.name == role).Count() > 0;
            }
        }
    }
}
