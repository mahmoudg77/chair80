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
    [RoutePrefix("User")]
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
        public async Task<APIResult<LoginResponse>> Register(RegisterRequest request, string otpcode)
        {
            var c = HttpContext.Current;
            var v = request.isValid();
            if (v.data == false) return new APIResult<LoginResponse>(ResultType.fail, null, v.message);



            string trueMobile = "";

            if (General.ValidateMobile(request.phoneNumber, out trueMobile))
            {
                request.phoneNumber = trueMobile;
            }
            else
            {
                return new APIResult<LoginResponse>(ResultType.fail, null, "Invalid mobile number!");
            }


            #region Verify Mobile
            
            var verify = Users.VerifyMobile(request.phoneNumber, otpcode);

            if (verify.type == ResultType.fail) return new APIResult<LoginResponse>(ResultType.fail, null, verify.message);
            
            #endregion

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
                return new APIResult<LoginResponse>(ResultType.fail, null, "Email is required!");
            if (phone == "")
                return new APIResult<LoginResponse>(ResultType.fail, null, "Phone is required!");
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
            if (v.data == false) return new APIResult<LoginResponse>(ResultType.fail, null, v.message);



            string trueMobile = "";

            if (General.ValidateMobile(login.phoneNumber, out trueMobile))
            {
                login.phoneNumber = trueMobile;
            }
            else
            {
                return new APIResult<LoginResponse>(ResultType.fail, null, "Invalid mobile number!");
            }


            #region Verify Mobile

            var verify = Users.VerifyMobile(login.phoneNumber, login.otpcode);

            if (verify.type == ResultType.fail) return new APIResult<LoginResponse>(ResultType.fail, null, verify.message);

            #endregion

            return Users.LoginByPhone(login.phoneNumber, login.otpcode,c.Request.ServerVariables);
            
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
        //            return new APIResult<bool>(ResultType.success, true, "API_SUCCESS_CONFIRM");
        //        }
        //        else
        //        {
        //            return new APIResult<bool>(ResultType.fail, false, "API_ERROR_CONFIRM");
        //        }
        //    }
        //    //if (result != null)
        //    //{
        //    //    LoginResponse l = new LoginResponse();
        //    //    l.account = result;
        //    //    l.token = result.sec_users.sec_sessions.First().id;
        //    //    l.account.sec_users = null;
        //    //    return new APIResult<LoginResponse>(ResultType.success, l, "API_SUCCESS");

        //    //}
        //    //return new APIResult<LoginResponse>(ResultType.fail, null, "API_ERROR_LOGIN");
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
                    return new APIResult<LoginResponse>(ResultType.fail, null, "API_ERROR_LOGIN");

                var AuthKey = HttpContext.Current.Request.Headers.GetValues("AUTH_KEY");
                LoginResponse l = new LoginResponse();
                l.account = acc;
                l.token = Guid.Parse(AuthKey.First().ToString());

                using (MainEntities dal = new MainEntities()) {
                    l.roles = dal.sec_users_roles.Include("sec_roles").Where(a => a.user_id == acc.id).Select(b => b.sec_roles.role_key).ToArray();
                }

                return new APIResult<LoginResponse>(ResultType.success, l, "API_SUCCESS");
            }
        }

        [HttpPost]
        [Route("CheckPhone")]
        public async Task<APIResult<bool>> CheckPhone(string phone, string countryCode = "20")
        {
            string validPhone = "";
            if (!General.ValidateMobile(phone, out validPhone, countryCode)) return new APIResult<bool>(ResultType.fail, false, "Invalid mobile number !");

            using (MainEntities ctx = new MainEntities())
            {
                int countAcc = ctx.tbl_accounts.Count(a => a.mobile==validPhone);

                if (countAcc > 0)
                {
                    var v = await SendVerifyCode(validPhone);
                    if (v.type == ResultType.success)
                        return new APIResult<bool>(ResultType.success, true, "Phone already exists, You will receive access code by SMS.");
                    else
                        return v;
                }

                return new APIResult<bool>(ResultType.success, false, "Phone not found !");

            }
        }
        //[HttpPost]
        //[Route("EditPhone")]
        //[LoginFilter]
        //public APIResult<bool> EditPhone(string phone, string countryCode = "20")
        //{
        //    string validPhone = "";
        //    if (!General.ValidateMobile(phone, out validPhone, countryCode)) return new APIResult<bool>(ResultType.fail, false, "Invalid mobile number !");



        //    using (MainEntities ctx = new MainEntities())
        //    {
        //        var u = APIRequest.User(HttpContext.Current.Request).Entity;
        //        var acc =ctx.tbl_accounts.Find(u.id);
        //        acc.mobile = validPhone;

        //        ctx.Entry(acc).State = System.Data.Entity.EntityState.Modified;


        //        if (ctx.SaveChanges() > 0)
        //            return new APIResult<bool>(ResultType.success, true, "Phone saved success");

        //        return new APIResult<bool>(ResultType.success, false, "Error while saving!");

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
        //            return new APIResult<Libs.DataTableResponse<sec_users>>(ResultType.fail, null, "API_ERROR_BAD");
        //        }
        //        Libs.DataTableResponse<sec_users> dt = new Libs.DataTableResponse<sec_users>(0, users.Count(), users.Count(), users);

        //        return new APIResult<Libs.DataTableResponse<sec_users>>(ResultType.success, dt, "API_SUCCESS");

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
                ctx.Entry(ses).State = System.Data.Entity.EntityState.Modified;

                if (ctx.SaveChanges() == 0)
                    return new APIResult<bool>(ResultType.fail, false, "API_ERROR_BAD");
            }

            return new APIResult<bool>(ResultType.success, true, "API_SUCCESS");
        }
        //[HttpPost]
        //[LoginFilter]
        //public APIResult<bool> HasAccess(string Screen,string Method)
        //{
        //    if (string.IsNullOrEmpty(Screen) || string.IsNullOrEmpty(Method))
        //    {
        //        return new APIResult<bool>(ResultType.fail, false, "API_ERROR_BAD");
        //    }

        //    var AuthKey = Request.Headers.FirstOrDefault(a => a.Key == "AUTH_KEY");
        //    using (MainEntities ctx = new MainEntities())
        //    {

        //        sec_sessions user = ctx.sec_sessions.Find(Guid.Parse(AuthKey.Value.First().ToString()));
        //        if (user == null || user.end_time != null)
        //            return new APIResult<bool>(ResultType.fail, false, "API_ERROR_BAD");

        //        Users users = new Users(user.user_id);

        //        if (users.Entity == null)
        //            return new APIResult<bool>(ResultType.fail, false, "API_ERROR_BAD");

        //        return new APIResult<bool>(ResultType.success, users.Allow(Screen, Method), "API_SUCCESS");

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
        //            return new APIResult<sec_users>(ResultType.success, rec, "API_SUCCESS");
        //        }



        //    }


        //    return new APIResult<sec_users>(ResultType.fail, null, "");
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
                        return new APIResult<Dictionary<string, Dictionary<string, bool>>>(ResultType.fail, null, "API_ERROR_BAD");


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


                    return new APIResult<Dictionary<string, Dictionary<string, bool>>>(ResultType.success, rows, "API_SUCCESS");
                }
                catch (DbEntityValidationException e)
                {
                    Dictionary<string, Dictionary<string, bool>> i = new Dictionary<string, Dictionary<string, bool>>();
                    i.Add(General.fetchEntityError(e), null);
                    return new APIResult<Dictionary<string, Dictionary<string, bool>>>(ResultType.fail, i, "API_EXECPTION");
                }
                catch (Exception ex)
                {
                    Dictionary<string, Dictionary<string, bool>> i = new Dictionary<string, Dictionary<string, bool>>();
                    i.Add(ex.Message, null);
                    return new APIResult<Dictionary<string, Dictionary<string, bool>>>(ResultType.fail, i, "API_EXECPTION");
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
        //                return new APIResult<UserRoleResponse>(ResultType.success, r, "API_SUCCESS");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return new APIResult<UserRoleResponse>(ResultType.fail, null,ex.Message);
        //    }


        //    return new APIResult<UserRoleResponse>(ResultType.fail, null, "");
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

        //            return new APIResult<UserRoleResponse>(ResultType.success, user, "API_SUCCESS");
        //        }
        //    }

        //    return new APIResult<UserRoleResponse>(ResultType.fail, user, "");
        //}

        //[HttpPost]
        //[Route("User/ResetPassword")]
        //public APIResult<bool> ResetPassword(Guid ResetPwdKey, string NewPassword)
        //{
        //    Users usr = new Users();
        //    bool result = usr.resetPassword(ResetPwdKey, NewPassword);
        //    if (result==false)
        //    {
        //        return new APIResult<bool>(ResultType.fail, false, "API_ERROR_BAD");
        //    }
        //    else
        //    {
        //        return new APIResult<bool>(ResultType.success, true, "API_SUCCESS");
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
        //        if (user == null) return new APIResult<LoginResponse>(ResultType.fail,null,"API_ERROR_BAD");

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
        //        if (user == null) return new APIResult<LoginResponse>(ResultType.fail,null,"API_ERROR_BAD");

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

        //     //return new APIResult<LoginResponse>(ResultType.fail, null, "API_ERROR_BAD");

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
            using (MainEntities ctx = new MainEntities())
            {
                string trueMobile = Mobile;
                if (General.ValidateMobile(Mobile, out trueMobile))
                {
                    Mobile = trueMobile;
                }
                else
                {
                    return new APIResult<bool>(ResultType.fail, false, "Invalid mobile number!");
                }


                var dublicated = ctx.tbl_accounts.Include("sec_users").Where(a => a.mobile == Mobile && a.sec_users.phone_verified == true ).Count();

                if (dublicated > 0)
                    return new APIResult<bool>(ResultType.fail, false, "This mobile is already exists in our database!");
            }

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
                                return new APIResult<bool>(ResultType.success, true, "API_SUCCESS");
                            }
                            else
                            {
                                Logger.log(string.Format("SMSErorr: Code={0},Mobile={1} \r\n {2}", code, Mobile, smsResult));
                            }
                        }
                    }
                    return new APIResult<bool>(ResultType.fail, false, "Bad Request!");
                }
            }
            return new APIResult<bool>(ResultType.fail, false, "API_SUCCESS");
        }

        //[SettingFilter("verify_mobile")]
        //[LoginFilter]
        //[HttpPost]
        //[Route("User/VerifyMobile")]
        //public async Task<APIResult<bool>> VerifyMobile(string Mobile, string Code)
        //{
        //    string trueMobile = Mobile;
        //    if (General.ValidateMobile(Mobile, out trueMobile))
        //    {
        //        Mobile = trueMobile;
        //    }
        //    else
        //    {
        //        return new APIResult<bool>(ResultType.fail, false, "Invalid mobile number!");
        //    }
        //    using (MainEntities ctx = new MainEntities())
        //    {

        //        var vm = ctx.sec_mobile_verify.Where(a => a.mobile == Mobile && a.code == Code ).OrderByDescending(a => a.id).FirstOrDefault();
        //        if (vm == null) return new APIResult<bool>(ResultType.fail, false, "Invalid code or mobile number !!");
        //        if (vm.is_used == true) return new APIResult<bool>(ResultType.fail, false, "This code is already used !!");
        //        if (vm.created_at < DateTime.Now.Add(new TimeSpan(0, -10, 0))) return new APIResult<bool>(ResultType.fail, false, "This code expired !!");

        //        vm.is_used = true;


        //        ctx.Entry(vm).State = System.Data.Entity.EntityState.Modified;

        //        if (ctx.SaveChanges() > 0)
        //        {
        //            return new APIResult<bool>(ResultType.success, true, "API_SUCCESS");

        //        }
        //        else
        //        {
        //            return new APIResult<bool>(ResultType.fail, false, "API_ERORR_SAVE");

        //        }

        //    }

        //}

        //[LoginFilter]
        //[HttpPost]
        //[Route("User/ChangePassword")]
        //public APIResult<bool> ChangePassword(PasswordEditRequest request)
        //{
        //    var u = APIRequest.User(HttpContext.Current.Request).Entity;

        //    if (request.password != request.cpassword) return new APIResult<bool>(ResultType.fail, false, "Password and confirm not matches !!");
        //    if (request.password.Length<5) return new APIResult<bool>(ResultType.fail, false, "Very short password min (5 chars) !!");
        //    if (General.MD5(request.current)!=u.pwd) return new APIResult<bool>(ResultType.fail, false, "Invalid current password !!");

        //    using(MainEntities ctx=new MainEntities())
        //    {
        //        var user=ctx.sec_users.Find(u.id);
        //        if(General.MD5(request.password)==user.pwd) return new APIResult<bool>(ResultType.fail, false, "Cannot save new password, It is the same 'Current Password'!!");
        //        user.pwd = General.MD5(request.password);

        //        ctx.Entry(user).State = System.Data.Entity.EntityState.Modified;
        //        if (ctx.SaveChanges()>0)
        //        {
        //            return new APIResult<bool>(ResultType.success, true, "API_SUCCESS");
        //        }
        //        return new APIResult<bool>(ResultType.fail, false, "Cannot save password !!");
        //    }
        //}


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
   

       
    