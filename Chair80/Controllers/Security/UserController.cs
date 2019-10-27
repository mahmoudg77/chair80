using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Chair80.BLL.Security;
using Chair80.Libs;
using Chair80.Requests;
using Chair80.Responses;
using Newtonsoft.Json;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Chair80.DAL;
using System.Configuration;
using Chair80.Filters;

namespace Chair80.Controllers
{
    [AppFilter]
    [RoutePrefix("{lang}/User")]
    public class UserController : ApiController
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="otpcode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        public async Task<APIResult<LoginResponse>> Register(RegisterRequest request)
        {
            var c = HttpContext.Current;
            var v = request.isValid();
            if (v.data == false) return APIResult<LoginResponse>.Error(v.code, v.message);


            string trueMobile = "";

            if (General.ValidateMobile(request.phoneNumber, out trueMobile))
            {
                request.phoneNumber = trueMobile;
            }
            else
            {
                return APIResult<LoginResponse>.Error(ResponseCode.UserValidationField, "Invalid mobile number!");
            }

            var verified = Users.MobileVerified(request.phoneNumber,request.verification_id);
            if (!verified.data)
                return APIResult<LoginResponse>.Error(ResponseCode.UserUnVerified, "Your phone still not verified!");
            
            #region Get User Data From Firebase

            try
            {

                if (FirebaseApp.DefaultInstance != null)
                    FirebaseApp.DefaultInstance.Delete();

                //{

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(c.Server.MapPath("~/App_Data/firebase-config.json")),

                }
                );


            }
            catch (Exception ex)
            {

                throw new Exception(ErrorHandler.Message(ex));
            }
            FirebaseToken decodedToken;
            try
            {


                decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.access_token);
            }
            catch (Exception ex)
            {

                throw new Exception(ErrorHandler.Message(ex));
            }

            string uid = decodedToken.Uid;
            string email = "";
            string phone = request.phoneNumber;

            try
            {

                if (decodedToken.Claims.Keys.Contains("email")) email = decodedToken.Claims.FirstOrDefault(a => a.Key == "email").Value.ToString();


            }
            catch (Exception ex)
            {

                throw new Exception(ErrorHandler.Message(ex));
            }

            #endregion

            var f_name = request.first_name;

            var l_name = request.last_name;



            if (email == "")
                return APIResult<LoginResponse>.Error(ResponseCode.UserValidationField, "Email is required!");
            if (phone == "")
                return APIResult<LoginResponse>.Error(ResponseCode.UserValidationField, "Phone is required!");
            tbl_accounts acc = new tbl_accounts();
            acc.city_id = request.city==0?null:request.city;
            acc.country_id = request.country;
            acc.date_of_birth = request.date_of_birth;
            acc.email = email;
            acc.first_name = request.first_name;
            acc.gender_id = request.gender_id;
            acc.last_name = request.last_name;
            acc.mobile = phone;
            acc.register_time = DateTime.Now;


            return Users.Register(acc, request.password, uid, c.Request.ServerVariables);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public APIResult<LoginResponse> Login(LoginRequest login)
        {
            var c = HttpContext.Current;
            var v = login.isValid();
            if (v.data == false) return APIResult<LoginResponse>.Error(v.code, v.message);



            string trueMobile = "";

            if (General.ValidateMobile(login.phoneNumber, out trueMobile))
            {
                login.phoneNumber = trueMobile;
            }
            else
            {
                return APIResult<LoginResponse>.Error(ResponseCode.UserValidationField, "Invalid mobile number!");
            }


            #region Verify Mobile

            var verify = Users.VerifyMobile(login.phoneNumber, login.otpcode);

            if (!verify.isSuccess) return APIResult<LoginResponse>.Error(ResponseCode.UserValidationField, verify.message);

            #endregion

            return Users.LoginByPhone(login.phoneNumber, login.otpcode,c.Request.ServerVariables);
            
        }

        [AppFilter]
        [HttpPost]
        [Route("WebLogin")]
        public APIResult<LoginResponse> Login(WebLoginRequest login)
        {
            var c = HttpContext.Current;
            var v = login.isValid();
            if (v.data == false) return APIResult<LoginResponse>.Error(v.code, v.message);



            if (!General.EmailIsValid(login.email))
                return APIResult<LoginResponse>.Error(ResponseCode.UserValidationField, "Invalid Email Address!");
             



            return Users.LoginByEmail(login.email, login.password, c.Request.ServerVariables);

        }

        //[HttpPost]
        //[Route("User/ConfirmEmail")]
        //public APIResult<bool> ConfirmEmail(Guid key)
        //{
        //    NameValueCollection arr = HttpContext.Current.Request.ServerVariables;

        //    using(MainEntities ctx=new MainEntities())
        //    {
        //        var u = ctx.sec_users.Where(a => a.confirm_mail_token == key.ToString()).FirstOrDefault();
        //        if (u!=null)
        //        {
        //            u.confirm_mail_token = null;
        //            u.mail_verified = true;
        //            ctx.Entry(u).State = System.Data.Entity.EntityState.Modified;
        //            ctx.SaveChanges();
        //            return APIResult<bool>(ResultType.success, true, "API_SUCCESS_CONFIRM");
        //        }
        //        else
        //        {
        //            return APIResult<bool>(ResultType.fail, false, "API_ERROR_CONFIRM");
        //        }
        //    }
        //    //if (result != null)
        //    //{
        //    //    LoginResponse l = new LoginResponse();
        //    //    l.account = result;
        //    //    l.token = result.sec_users.sec_sessions.First().id;
        //    //    l.account.sec_users = null;
        //    //    return APIResult<LoginResponse>(ResultType.success, l, "API_SUCCESS");

        //    //}
        //    //return APIResult<LoginResponse>(ResultType.fail, null, "API_ERROR_LOGIN");
        //}

        [HttpPost]
        [LoginFilter]
        [Route("Current")]
        public APIResult<LoginResponse> Current()
        {

          
            var u = APIRequest.User(HttpContext.Current.Request);

           
            using (MainEntities ctx = new MainEntities())
            {
                tbl_accounts acc = ctx.tbl_accounts.FirstOrDefault(a => a.id == u.Entity.id);

                if (u == null || acc == null)
                    return APIResult<LoginResponse>.Error(ResponseCode.UserForbidden, "API_ERROR_LOGIN");

                var AuthKey = HttpContext.Current.Request.Headers.GetValues("AUTH_KEY");
                LoginResponse l = new LoginResponse();
                l.account = acc;
                l.token = Guid.Parse(AuthKey.First().ToString());

                using (MainEntities dal = new MainEntities()) {
                    l.roles = dal.sec_users_roles.Include("sec_roles").Where(a => a.user_id == acc.id).Select(b => b.sec_roles.role_key).ToArray();
                }

                return APIResult<LoginResponse>.Success(l, "API_SUCCESS");
            }
        }

        [HttpPost]
        [Route("CheckPhone")]
        public async Task<APIResult<bool>> CheckPhone(string phone, string countryCode = "20")
        {
            string validPhone = "";
            if (!General.ValidateMobile(phone, out validPhone, countryCode)) return APIResult<bool>.Error(ResponseCode.UserValidationField, "Invalid mobile number !");

            using (MainEntities ctx = new MainEntities())
            {
                int countAcc = ctx.tbl_accounts.Count(a => a.mobile==validPhone);

                if (countAcc > 0)
                {
                    //var v = await SendVerifyCode(validPhone);
                    //if (v.isSuccess)
                        return APIResult<bool>.Success(true, "Phone already exists");
                    //else
                    //    return v;
                }

                return APIResult<bool>.Error(ResponseCode.DevNotFound, "Phone not found !",false);

            }
        }
        //[HttpPost]
        //[Route("EditPhone")]
        //[LoginFilter]
        //public APIResult<bool> EditPhone(string phone, string countryCode = "20")
        //{
        //    string validPhone = "";
        //    if (!General.ValidateMobile(phone, out validPhone, countryCode)) return APIResult<bool>(ResultType.fail, false, "Invalid mobile number !");



        //    using (MainEntities ctx = new MainEntities())
        //    {
        //        var u = APIRequest.User(HttpContext.Current.Request).Entity;
        //        var acc =ctx.tbl_accounts.Find(u.id);
        //        acc.mobile = validPhone;

        //        ctx.Entry(acc).State = System.Data.Entity.EntityState.Modified;


        //        if (ctx.SaveChanges() > 0)
        //            return APIResult<bool>(ResultType.success, true, "Phone saved success");

        //        return APIResult<bool>(ResultType.success, false, "Error while saving!");

        //    }
        //}

        //[AuthFilter]
        //[Route("AdminApi/User")]
        //public APIResult<Libs.DataTableResponse<sec_users>> Get()
        //{
        //    using(MainEntities ctx=new MainEntities())
        //    {
        //        var users = ctx.sec_users.Include("tbl_accounts");
        //        if (users == null)
        //        {
        //            return APIResult<Libs.DataTableResponse<sec_users>>(ResultType.fail, null, "API_ERROR_BAD");
        //        }
        //        Libs.DataTableResponse<sec_users> dt = new Libs.DataTableResponse<sec_users>(0, users.Count(), users.Count(), users);

        //        return APIResult<Libs.DataTableResponse<sec_users>>(ResultType.success, dt, "API_SUCCESS");

        //    }

        //}

        [HttpGet]
        [LoginFilter]
        [Route("Logout")]
        public APIResult<bool> Logout()
        {

            var AuthKey = HttpContext.Current.Request.Headers.GetValues("AUTH_KEY");

            using (MainEntities ctx = new MainEntities())
            {
                var ses = ctx.sec_sessions.Find(Guid.Parse(AuthKey.First().ToString()));
                ses.end_time = DateTime.Now;
                ses.device_id = null;
                ctx.Entry(ses).State = System.Data.Entity.EntityState.Modified;

                if (ctx.SaveChanges() == 0)
                    return APIResult<bool>.Error(ResponseCode.BackendDatabase, "API_ERROR_BAD",false);
            }

            return APIResult<bool>.Success( true, "API_SUCCESS");
        }
        //[HttpPost]
        //[LoginFilter]
        //public APIResult<bool> HasAccess(string Screen,string Method)
        //{
        //    if (string.IsNullOrEmpty(Screen) || string.IsNullOrEmpty(Method))
        //    {
        //        return APIResult<bool>(ResultType.fail, false, "API_ERROR_BAD");
        //    }

        //    var AuthKey = Request.Headers.FirstOrDefault(a => a.Key == "AUTH_KEY");
        //    using (MainEntities ctx = new MainEntities())
        //    {

        //        sec_sessions user = ctx.sec_sessions.Find(Guid.Parse(AuthKey.Value.First().ToString()));
        //        if (user == null || user.end_time != null)
        //            return APIResult<bool>(ResultType.fail, false, "API_ERROR_BAD");

        //        Users users = new Users(user.user_id);

        //        if (users.Entity == null)
        //            return APIResult<bool>(ResultType.fail, false, "API_ERROR_BAD");

        //        return APIResult<bool>(ResultType.success, users.Allow(Screen, Method), "API_SUCCESS");

        //    }



        //}


        //[AuthFilter]
        //public APIResult<sec_users> Get(int id)
        //{


        //    using (MainEntities ctx = new MainEntities())
        //    {
        //        sec_users rec = ctx.sec_users.Find(id);
        //        if (rec != null)
        //        {
        //            return APIResult<sec_users>(ResultType.success, rec, "API_SUCCESS");
        //        }



        //    }


        //    return APIResult<sec_users>(ResultType.fail, null, "");
        //}

        [HttpPost]
        [LoginFilter]
        [Route("HasAccess")]
        public APIResult<Dictionary<string, Dictionary<string, bool>>> HasAccess(IEnumerable<HasAccessResponse> request)
        {



            //var AuthKey = Request.Headers.FirstOrDefault(a => a.Key == "AUTH_KEY");
            using (MainEntities ctx = new MainEntities())
            {

                try
                {
                    var u = APIRequest.User(HttpContext.Current.Request);


                    if (u == null || u.Entity == null)
                        return APIResult<Dictionary<string, Dictionary<string, bool>>>.Error(ResponseCode.UserForbidden, "API_ERROR_BAD");


                    var roleIds = ctx.sec_users_roles.Where(a => a.user_id == u.Entity.id).Select(b => b.role_id);



                    Dictionary<string, Dictionary<string, bool>> rows = new Dictionary<string, Dictionary<string, bool>>();


                    foreach (var item in request)
                    {
                        //Dictionary<string, bool> row;
                        var AccessRights = ctx.sec_access_right.Where(a => a.model_name == item.Screen && roleIds.Contains(a.role_id) && a.method_name == item.Method);


                        bool allow = false;
                        if (AccessRights.Count() > 0) allow = true;

                        if (rows.Keys != null && rows.Keys.Contains(item.Screen))
                            rows[item.Screen].Add(item.Method, allow);
                        else
                        {
                            Dictionary<string, bool> row = new Dictionary<string, bool>();
                            row.Add(item.Method, allow);
                            rows.Add(item.Screen, row);
                        }
                    }


                    return APIResult<Dictionary<string, Dictionary<string, bool>>>.Success( rows, "API_SUCCESS");
                }
                catch (DbEntityValidationException e)
                {
                    Dictionary<string, Dictionary<string, bool>> i = new Dictionary<string, Dictionary<string, bool>>();
                    i.Add(General.fetchEntityError(e), null);
                    return APIResult<Dictionary<string, Dictionary<string, bool>>>.Error(ResponseCode.BackendDatabase, "API_EXECPTION");
                }
                catch (Exception ex)
                {
                    Dictionary<string, Dictionary<string, bool>> i = new Dictionary<string, Dictionary<string, bool>>();
                    i.Add(ex.Message, null);
                    return APIResult<Dictionary<string, Dictionary<string, bool>>>.Error(ResponseCode.BackendInternalServer, "API_EXECPTION");
                }



            }


        }
        //[AuthFilter]
        //[HttpPost]
        //public APIResult<UserRoleResponse> ItemForEdit(int id)
        //{
        //    try
        //    {
        //        using (MainEntities ctx = new MainEntities())
        //        {
        //            sec_users rec = ctx.sec_users.Find(id);
        //            UserRoleResponse r = new UserRoleResponse();
        //            r.id = id;
        //            r.roles = ctx.sec_users_roles.Where(a => a.user_id == rec.id).ToList();
        //           foreach (var i in r.roles) i.sec_users = null;
        //            r.email = ctx.tbl_accounts.Find(rec.id).email;
        //            //rec.sec_users_roles = ctx.sec_users_roles.Where(a => a.user_id == rec.id).ToList();

        //            if (r != null)
        //            {
        //                return APIResult<UserRoleResponse>(ResultType.success, r, "API_SUCCESS");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return APIResult<UserRoleResponse>(ResultType.fail, null,ex.Message);
        //    }


        //    return APIResult<UserRoleResponse>(ResultType.fail, null, "");
        //}

        //[AuthFilter]
        //[Route("User")]
        //public APIResult<UserRoleResponse> Put(UserRoleResponse user)
        //{
        //    using (MainEntities ctx = new MainEntities())
        //    {
        //        sec_users rec = ctx.sec_users.Find(user.id);

        //        foreach(var i in ctx.sec_users_roles.Where(a => a.user_id == rec.id))
        //            ctx.Entry(i).State = System.Data.Entity.EntityState.Deleted;

        //        foreach (var r in user.roles) {
        //            ctx.sec_users_roles.Add(r);
        //            ctx.Entry(r).State = System.Data.Entity.EntityState.Added;
        //        }
        //        if (ctx.SaveChanges()>0)
        //        {

        //            return APIResult<UserRoleResponse>(ResultType.success, user, "API_SUCCESS");
        //        }
        //    }

        //    return APIResult<UserRoleResponse>(ResultType.fail, user, "");
        //}

        //[HttpPost]
        //[Route("User/ResetPassword")]
        //public APIResult<bool> ResetPassword(Guid ResetPwdKey, string NewPassword)
        //{
        //    Users usr = new Users();
        //    bool result = usr.resetPassword(ResetPwdKey, NewPassword);
        //    if (result==false)
        //    {
        //        return APIResult<bool>(ResultType.fail, false, "API_ERROR_BAD");
        //    }
        //    else
        //    {
        //        return APIResult<bool>(ResultType.success, true, "API_SUCCESS");
        //    }

        //}



        //[HttpPost]
        //public async Task<APIResult<LoginResponse>> Facebook(string AccessToken)
        //{
        //    using (var client = new HttpClient())
        //    {

        //        var uri = new Uri("https://graph.facebook.com/v2.10/me?access_token=" + AccessToken + "&fields=name,email,picture&method=get&pretty=0&sdk=joey&suppress_http_code=1");

        //        var response = await client.GetAsync(uri);

        //        string textResult = await response.Content.ReadAsStringAsync();

        //        var user = JsonConvert.DeserializeObject<FacebookUser>(textResult);
        //        if (user == null) return APIResult<LoginResponse>(ResultType.fail,null,"API_ERROR_BAD");

        //        return await  this.Auth(user.email, AccessToken, user.name, user.name, HttpContext.Current, user.picture.data.url);
        //    }
        //}

        //[HttpPost]
        //public async Task<APIResult<LoginResponse>> Google(string AccessToken)
        //{
        //    using (var client = new HttpClient())
        //    {

        //        var uri = new Uri("https://www.googleapis.com/plus/v1/people/me?access_token=" + AccessToken);

        //        var response = await client.GetAsync(uri);

        //        string textResult = await response.Content.ReadAsStringAsync();

        //        var user = JsonConvert.DeserializeObject<GoogleUser>(textResult);
        //        if (user == null) return APIResult<LoginResponse>(ResultType.fail,null,"API_ERROR_BAD");

        //        return await this.Auth(user.emails.First().value, AccessToken, user.name.givenName, user.name.familyName, HttpContext.Current);
        //    }
        //}
        //[HttpPost]
        // public async Task<APIResult<LoginResponse>> FirebaseOAuth(string network, string AccessToken)
        // {
        //     var c = HttpContext.Current;
        //     try
        //     {

        //         if (FirebaseApp.DefaultInstance != null)
        //             FirebaseApp.DefaultInstance.Delete();

        //         //{

        //         FirebaseApp.Create(new AppOptions()
        //         {
        //             Credential = GoogleCredential.FromFile(HttpContext.Current.Server.MapPath("~/App_Data/metookey-219517-de58b500e4bb.json")),

        //         }
        //             );


        //     }
        //     catch (Exception ex)
        //     {

        //         throw new Exception(ex.Message + " Line : 412");
        //     }
        //     FirebaseToken decodedToken;
        //     try
        //     {


        //         decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(AccessToken);
        //     }
        //     catch (Exception ex)
        //     {

        //         throw new Exception(ex.Message + " Line : 424");
        //     }
        //     string uid = decodedToken.Uid;
        //     string email = "";
        //     string name = "";
        //     string picture = "";
        //     try
        //     {



        //         if (decodedToken.Claims.Keys.Contains("email")) email = decodedToken.Claims.FirstOrDefault(a => a.Key == "email").Value.ToString();

        //         if (decodedToken.Claims.Keys.Contains("name")) name = decodedToken.Claims.FirstOrDefault(a => a.Key == "name").Value.ToString();

        //         if (decodedToken.Claims.Keys.Contains("picture")) picture = decodedToken.Claims.FirstOrDefault(a => a.Key == "picture").Value.ToString();

        //     }
        //     catch (Exception ex)
        //     {

        //         throw new Exception(ex.Message + " Line : 445");
        //     }

        //     //try
        //     //{

        //     //return APIResult<LoginResponse>(ResultType.fail, null, "API_ERROR_BAD");

        //     var f_name = name.Split(' ')[0];

        //     var l_name = "";

        //     if (name.Split(' ').Length > 1)
        //         l_name = name.Substring(f_name.Length);


        //     return await this.Auth(email, uid, f_name, l_name, c, picture, network, uid);
        //     //}
        //     //catch (Exception ex)
        //     //{

        //     //    throw new Exception(ex.Message +  " Name:" + name + " Email:" + email + " uid:"+uid);
        //     //}
        // }

     
        //[SettingFilter("verify_mobile")]
        //[LoginFilter]
        [HttpPost]
        [Route("SendVerifyCode")]
        public async Task<APIResult<bool>> SendVerifyCode(string Mobile)
        {
            //int uid = APIRequest.User(HttpContext.Current.Request).Entity.id;
            //using (MainEntities ctx = new MainEntities())
            //{
                string trueMobile = Mobile;
                if (General.ValidateMobile(Mobile, out trueMobile))
                {
                    Mobile = trueMobile;
                }
                else
                {
                    return APIResult<bool>.Error(ResponseCode.UserValidationField, "Invalid mobile number!");
                }


                //var dublicated = ctx.tbl_accounts.Include("sec_users").Where(a => a.mobile == Mobile && a.sec_users.phone_verified == true ).Count();

                //if (dublicated > 0)
                //    return APIResult<bool>.Error(ResponseCode.UserDoublicate, "This mobile is already exists in our database!");
            //}

            var sms_url = Settings.AppSetting.FirstOrDefault(a => a.setting_key == "sms_url").setting_value;
            if (sms_url != null && sms_url != "")
            {
                using (var client = new HttpClient())
                {
                    Random random = new Random();
                    var code = random.Next(100000, 999999);

                    using (MainEntities ctx = new MainEntities())
                    {
                        DateTime expiredTime = DateTime.Now.Add(new TimeSpan(0, -10, 0));

                        sec_mobile_verify vm = ctx.sec_mobile_verify.Where(a => a.mobile == Mobile  && a.is_used == false && a.created_at > expiredTime).OrderByDescending(a => a.id).FirstOrDefault();
                        if (vm != null)
                        {
                            code = int.Parse(vm.code);
                            vm.created_at = DateTime.Now;
                            ctx.Entry(vm).State = System.Data.Entity.EntityState.Modified;

                        }
                        else
                        {
                            vm = new sec_mobile_verify();
                            vm.mobile = Mobile;
                            vm.code = code.ToString();
                            vm.created_at = DateTime.Now;
                            ctx.sec_mobile_verify.Add(vm);
                        }

                       
                        if (ctx.SaveChanges() > 0)
                        {
                            var uri = new Uri(sms_url.Replace("##mobile##", Mobile).Replace("##code##", code.ToString()));
                            var response = await client.PostAsJsonAsync(uri, "");

                            var smsResult = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode && smsResult.Contains("success"))
                            {
                                return APIResult<bool>.Success( true, "API_SUCCESS");
                            }
                            else
                            {
                                Logger.log(string.Format("SMSErorr: Code={0},Mobile={1} \r\n {2}", code, Mobile, smsResult));
                            }
                        }
                    }
                    return APIResult<bool>.Error(ResponseCode.BackendInternalServer, "Bad Request!");
                }
            }
            return APIResult<bool>.Error(ResponseCode.BackendInternalServer, "API_SUCCESS");
        }

        //[SettingFilter("verify_mobile")]
        //[LoginFilter]
        [HttpPost]
        [Route("VerifyMobile")]
        public async Task<APIResult<MobileVerifyResponse>> VerifyMobile(string Mobile, string Code)
        {
            string trueMobile = Mobile;
            if (General.ValidateMobile(Mobile, out trueMobile))
            {
                Mobile = trueMobile;
            }
            else
            {
                return APIResult<MobileVerifyResponse>.Error(ResponseCode.UserValidationField, "Invalid mobile number!");
            }

            var verify = Users.VerifyMobile(Mobile, Code);
            return verify;
 
        }

        //[LoginFilter]
        //[HttpPost]
        //[Route("User/ChangePassword")]
        //public APIResult<bool> ChangePassword(PasswordEditRequest request)
        //{
        //    var u = APIRequest.User(HttpContext.Current.Request).Entity;

        //    if (request.password != request.cpassword) return APIResult<bool>(ResultType.fail, false, "Password and confirm not matches !!");
        //    if (request.password.Length<5) return APIResult<bool>(ResultType.fail, false, "Very short password min (5 chars) !!");
        //    if (General.MD5(request.current)!=u.pwd) return APIResult<bool>(ResultType.fail, false, "Invalid current password !!");

        //    using(MainEntities ctx=new MainEntities())
        //    {
        //        var user=ctx.sec_users.Find(u.id);
        //        if(General.MD5(request.password)==user.pwd) return APIResult<bool>(ResultType.fail, false, "Cannot save new password, It is the same 'Current Password'!!");
        //        user.pwd = General.MD5(request.password);

        //        ctx.Entry(user).State = System.Data.Entity.EntityState.Modified;
        //        if (ctx.SaveChanges()>0)
        //        {
        //            return APIResult<bool>(ResultType.success, true, "API_SUCCESS");
        //        }
        //        return APIResult<bool>(ResultType.fail, false, "Cannot save password !!");
        //    }
        //}


        [HttpPost]
        [Route("Delete")]
        public APIResult<bool> Delete(string phone)
            {
            string trueMobile = phone;
            if (General.ValidateMobile(phone, out trueMobile))
            {
                phone = trueMobile;
            }
            else
            {
                return APIResult<bool>.Error(ResponseCode.UserValidationField, "Invalid mobile number!");
            }

            using(var ctx=new MainEntities())
            {
                var acc = ctx.tbl_accounts.Where(a => a.mobile == phone).FirstOrDefault();
                if(acc ==null)  return APIResult<bool>.Error(ResponseCode.DevNotFound, "Account not found!");
                //roles=ctx.sec_users_roles.Where(a => a.user_id == acc.id);

                ctx.tbl_accounts.Remove(acc);
                try
                {

                    ctx.SaveChanges();

                    return APIResult<bool>.Success(true);

                }
                catch (DbEntityValidationException e)
                {
                    return APIResult<bool>.Error(ResponseCode.BackendInternalServer, General.fetchEntityError(e));
                }
                catch (Exception ex)
                {
                    return APIResult<bool>.Error(ResponseCode.UserValidationField, "Invalid mobile number!");

                }
            }
        }

        [HttpPost]
        [LoginFilter]
        [Route("SaveNewDeviceID")]
        public APIResult<bool> SaveNewDeviceID(String device_id)
        {


            var log = new Sessions(Guid.Parse(HttpContext.Current.Request.Headers.Get("AUTH_KEY")));



            log.Entity.device_id = device_id;
            using (var ctx = new MainEntities())
            {
                ctx.Entry(log.Entity).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {

                    return APIResult<bool>.Error(ResponseCode.BackendDatabase, ex.Message);

                }
            }
            return APIResult<bool>.Success(true);
        }

        [HttpGet]
        [LoginFilter]
        [Route("Invite")]
        public APIResult<bool> Invite(string email, string name)
        {
            using (var ctx=new DAL.MainEntities())
            {

                var u = APIRequest.User(HttpContext.Current.Request);
                var acc = ctx.tbl_accounts.Find(u.Entity.id);
                System.Net.WebClient webClient = new System.Net.WebClient();

                webClient.Encoding = System.Text.Encoding.UTF8;

                var html = webClient.DownloadString(Settings.Get("site_url") + "/MailBody/Invite?id=" + acc.id + "&email=" + email + "&name=" + name);


                Libs.General.SendMail(email, "Chari80 Invitaion from your frind", html, "Chari80", "", "", "", true);

                return APIResult<bool>.Success(true);
            }

        }
        
        [HttpPost]
        [Route("ChangePassword")]
        public APIResult<bool> ChangePassword(PasswordEditRequest request)
        {
            var u = APIRequest.User(HttpContext.Current.Request).Entity;

            //if (request.password != request.cpassword) return APIResult<bool>.Error(ResponseCode.UserValidationField, "Password and confirm not matches !!");
            if (request.password.Length < 6) return APIResult<bool>.Error(ResponseCode.UserValidationField, "Very short password min (6 chars) !!");
            if (request.current != u.pwd) return APIResult<bool>.Error(ResponseCode.UserValidationField, "Invalid current password !!");

            using (var ctx = new DAL.MainEntities())
            {
                var user = ctx.sec_users.Find(u.id);
                if (request.password == user.pwd) return APIResult<bool>.Error(ResponseCode.UserValidationField, "Cannot save new password, It is the same 'Current Password'!!");
                user.pwd = request.password;

                ctx.Entry(user).State = System.Data.Entity.EntityState.Modified;
                if (ctx.SaveChanges() > 0)
                {
                    return APIResult<bool>.Error(ResponseCode.UserValidationField, "API_SUCCESS");
                }
                return  APIResult<bool>.Error(ResponseCode.UserValidationField, "Cannot save password !!");
            }
        }



        [HttpPost]
        [Route("FirebaseOAuth")]
        public async Task<APIResult<LoginResponse>> FirebaseOAuth(FirebaseAuthRequest request)
        {
            string network = request.network;
            string AccessToken = request.firebaseToken;
            var c = HttpContext.Current;
            try
            {

                if (FirebaseApp.DefaultInstance != null)
                    FirebaseApp.DefaultInstance.Delete();

                //{

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(HttpContext.Current.Server.MapPath("~/App_Data/firebase-config.json")),

                }
                    );


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message + " Line : 412");
            }
            FirebaseToken decodedToken;
            try
            {


                decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(AccessToken);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message + " Line : 424");
            }
            string uid = decodedToken.Uid;
            string email = "";
            string name = "";
            string picture = "";
            try
            {

                Logger.log(JsonConvert.SerializeObject(decodedToken.Claims));

                if (decodedToken.Claims.Keys.Contains("email")) email = decodedToken.Claims.FirstOrDefault(a => a.Key == "email").Value.ToString();

                if (decodedToken.Claims.Keys.Contains("name")) name = decodedToken.Claims.FirstOrDefault(a => a.Key == "name").Value.ToString();

                if (decodedToken.Claims.Keys.Contains("picture")) picture = decodedToken.Claims.FirstOrDefault(a => a.Key == "picture").Value.ToString();

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message + " Line : 445");
            }

            //try
            //{

            //return new APIResult<LoginResponse>(ResultType.fail, null, "API_ERROR_BAD");

            var f_name = name.Split(' ')[0];

            var l_name = "";

            if (name.Split(' ').Length>1)l_name= name.Split(' ')[1];

            if (name.Split(' ').Length > 1)
                l_name = name.Substring(f_name.Length);


            return await this.Auth(email, uid, f_name, l_name, c, picture, network, uid);
            //}
            //catch (Exception ex)
            //{

            //    throw new Exception(ex.Message +  " Name:" + name + " Email:" + email + " uid:"+uid);
            //}
        }

        public async Task<APIResult<LoginResponse>> Auth(string email, string password, string first_name, string last_name, HttpContext http, string pic = "", string network = "", string FirebaseUID = "")
        {
            using (DAL.MainEntities ctx = new DAL.MainEntities())
            {
                //try
                //{


                tbl_accounts dbuser = null;
                try
                {


                    if (email != "")
                    {
                        dbuser = ctx.tbl_accounts.Include("sec_users").Where(a => a.email == email).FirstOrDefault();
                    }
                    else
                    if (FirebaseUID != "")
                    {
                        dbuser = ctx.tbl_accounts.Include("sec_users").Where(a => a.sec_users.firebase_uid == FirebaseUID).FirstOrDefault();
                    }

                    if (dbuser == null)
                    {
                        dbuser = new tbl_accounts();
                        dbuser.email = email;
                        dbuser.first_name = first_name;
                        dbuser.last_name = last_name;
                        dbuser.register_time = DateTime.Now;


                        ctx.tbl_accounts.Add(dbuser);
                        try
                        {
                            ctx.SaveChanges();
                            sec_users sec_user = new sec_users();

                            sec_user.pwd = password;
                            sec_user.id = dbuser.id;
                            sec_user.mail_verified = true;

                            if (FirebaseUID != "")
                                sec_user.firebase_uid = FirebaseUID;

                            ctx.sec_users.Add(sec_user);
                            ctx.SaveChanges();
                        }
                        //catch (DbEntityValidationException e)
                        //{
                        //    return new APIResult<LoginResponse>(ResultType.fail, null, General.fetchEntityError(e));
                        //}
                        catch (Exception ex)
                        {
                            return APIResult<LoginResponse>.Error(ResponseCode.BackendDatabase, ex.Message + "save changes1");
                        }

                    }

                }
                catch (Exception ex)
                {

                    return APIResult<LoginResponse>.Error(ResponseCode.BackendDatabase, ex.Message + "get dbuser");

                }
                tbl_images img = ctx.tbl_images.Where(a => a.model_name == "tbl_accounts" && a.model_id == dbuser.id && a.model_tag == "main").FirstOrDefault();
                if (pic != "" && img == null)
                {

                    img = new tbl_images();

                    try
                    {

                        img.original = "/Storage/Original/" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + network + ".jpg";
                        string imgPath = ConfigurationManager.AppSettings["mediaServer_Path"] + img.original.Replace("/", "\\");
                        img.large = img.original;
                        img.thumb = img.original;
                        img.meduim = img.original;
                        img.model_id = dbuser.id;
                        img.model_name = "tbl_accounts";
                        img.model_tag = "main";
                        System.Net.WebClient webClient = new System.Net.WebClient();

                        webClient.Encoding = System.Text.Encoding.UTF8;


                        webClient.DownloadFile(pic, imgPath);
                        ctx.tbl_images.Add(img);

                    }
                    catch (Exception ex)
                    {
                       // return APIResult<LoginResponse>.(ResultType.fail, null, ex.Message + "Save Image");
                    }
                    try
                    {

                        ctx.SaveChanges();
                    }
                    //catch (DbEntityValidationException e)
                    //{

                    //    return new APIResult<LoginResponse>(ResultType.fail, null, General.fetchEntityError(e));
                    //}
                    catch (Exception ex)
                    {
                       // return new APIResult<LoginResponse>(ResultType.fail, null, ex.Message + "save changes2");
                    }

                }



                var returned = new LoginResponse { account = dbuser };
                IPResult s = new IPResult();

                string ip = "";
                string agent = "";
                IPResult iploc = new IPResult();


                //if(HttpContext.Current == null) return new APIResult<LoginResponse>(ResultType.fail, null, "Null HTTPContext");
                //if (http.Request == null) return  APIResult<LoginResponse>.Error(ResponseCode., null, "Null HTTPRequest");
                //if (http.Request.ServerVariables == null) return new APIResult<LoginResponse>(ResultType.fail, null, "Null ServerVariables");
                //if (http.Request.ServerVariables.Count == 0) return new APIResult<LoginResponse>(ResultType.fail, null, "Empty ServerVariables");
                //if (!http.Request.ServerVariables.AllKeys.Contains("REMOTE_ADDR")) return new APIResult<LoginResponse>(ResultType.fail, null, "REMOTE_ADDR Not in ServerVariables");
                //if (!http.Request.ServerVariables.AllKeys.Contains("HTTP_USER_AGENT")) return new APIResult<LoginResponse>(ResultType.fail, null, "HTTP_USER_AGENT No in ServerVariables");
                try
                {
                    ip = http.Request.ServerVariables.Get("REMOTE_ADDR");
                    agent = http.Request.ServerVariables.Get("HTTP_USER_AGENT");

                    iploc = General.GetResponse("http://ip-api.com/json/" + ip);
                }
                catch (Exception ex)
                {
                    return  APIResult<LoginResponse>.Error(ResponseCode.BackendServerRequest, ex.Message + "get location ip:" + ip + " agent:" + agent);
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
                    
                    returned.roles = ctx.sec_users_roles.Include("sec_roles").Where(a => a.user_id == dbuser.id).Select(b => b.sec_roles.role_key).ToArray();
                    return  APIResult<LoginResponse>.Success(returned, "Login Success");

                }
                catch (DbEntityValidationException e)
                {

                    return APIResult<LoginResponse>.Error(ResponseCode.BackendDatabase, General.fetchEntityError(e));
                }
                catch (Exception ex)
                {

                    return  APIResult<LoginResponse>.Error(ResponseCode.BackendDatabase, ex.Message + " Save Session");

                }

                //}
                //catch (Exception ex)
                //{

                //    throw new Exception( ex.Message + "Auth");
                //}

            }
        }
    }

    //class FacebookUser
    //{
    //    public string name { get; set; }
    //    public string email { get; set; }
    //    public string id { get; set; }
    //    public fbPicture picture { get; set; }

    //}
    //class fbPicture
    //{
    //    public fbData data { get; set; }
    //}
    //class fbData
    //{
    //    public string url { get; set; }
    //}
    //class GoogleUser
    //{
    //    public GoogleUserName name { get; set; }
    //    public string id { get; set; }
    //    public string gender { get; set; }
    //    public List<GoogleUserEmail> emails { get; set; }

    //}
    //class GoogleUserName
    //{
    //    public string familyName { get; set; }
    //    public string givenName { get; set; }


    //}

    //class GoogleUserEmail
    //{
    //    public string value { get; set; }
    //    public string type { get; set; }


    //}


}
   

       
    