using Chair80.BLL;
using Chair80.DAL;
using Chair80.Filters;
using Chair80.Libs;
using Chair80.Requests;
using Chair80.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Chair80.Controllers.Account
{
    [AppFilter]
    [RoutePrefix("{lang}/Account")]
    public class AccountController : ApiController
    {
        [LoginFilter]
        [Route("actAsDriver")]
        [HttpPost]
        public APIResult<ActAsDriverResponse> actAsDriver(string DL, string ID="")
        {

            if (!Request.Content.IsMimeMultipartContent())
            {
                return APIResult<ActAsDriverResponse>.Error(ResponseCode.UserNotAcceptable, "Unsupported MediaType !");
            }

            var u = APIRequest.User(HttpContext.Current.Request).Entity;
            tbl_accounts acc;

            using (var ctx=new MainEntities())
            {
              acc = ctx.tbl_accounts.Find(u.id);
            }

            var httpRequest = HttpContext.Current.Request;
            var ID_Image = httpRequest.Files["ID_Image"];
            var DL_Image = httpRequest.Files["DL_Image"];

            if (u.mail_verified!=true) return APIResult<ActAsDriverResponse>.Error(ResponseCode.UserUnVerified, "Your email not verified, You must verify your email!");

            if (acc.id_no=="" && ID=="") return APIResult<ActAsDriverResponse>.Error(ResponseCode.UserValidationField, "ID number is required!");
            if (DL =="") return APIResult<ActAsDriverResponse>.Error(ResponseCode.UserValidationField, "Driver Lisence number is required!");


            if (ID_Image == null && ID !="") return APIResult<ActAsDriverResponse>.Error(ResponseCode.UserValidationField, "ID Image file is required !");
            if (DL_Image == null) return APIResult<ActAsDriverResponse>.Error(ResponseCode.UserValidationField, "Driver Lisence Image file is required !");

            if (ID == "") ID = acc.id_no;

            long vID = 0;
            long vDL = 0;

            if (!long.TryParse(ID,out vID)) return APIResult<ActAsDriverResponse>.Error(ResponseCode.UserValidationField, "Invalid ID number!");

            if (!long.TryParse(DL,out vDL)) return APIResult<ActAsDriverResponse>.Error(ResponseCode.UserValidationField, "Invalid Driver Lisence!");
             

            var returned =new ActAsDriverResponse() { DL = vDL.ToString(), ID = vID.ToString() };

            var result = Images.SaveImageFromFile(ID_Image, "tbl_accounts", u.id, "ID");

            if (!result.isSuccess) return APIResult<ActAsDriverResponse>.Error(result.code, result.message);
            returned.ID_Image = string.Format("/img/scale/tbl_accounts/{0}/original/{1}-0.gif",acc.id,"ID");
            result = Images.SaveImageFromFile(DL_Image, "tbl_accounts", u.id, "DL");
            returned.DL_Image = string.Format("/img/scale/tbl_accounts/{0}/original/{1}-0.gif", acc.id, "DL");

            if (!result.isSuccess) return APIResult<ActAsDriverResponse>.Error(ResponseCode.UserValidationField, result.message);

            using (var ctx = new MainEntities())
            {
                //var acc = ctx.tbl_accounts.Find(u.id);

                acc.id_no = vID.ToString();
                acc.driver_license_no = vDL.ToString();
                ctx.tbl_accounts.Attach(acc);
                ctx.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                if(ctx.sec_users_roles.Where(a=>a.user_id==u.id && a.role_id == 3).Count() == 0)
                {
                    ctx.sec_users_roles.Add(new sec_users_roles()
                    {
                        user_id = u.id,
                        role_id = 3
                    });
                }

                if (ctx.SaveChanges() == 0) return APIResult<ActAsDriverResponse>.Error(ResponseCode.BackendDatabase, "Error while saveing data!");
                 
            }
            return APIResult<ActAsDriverResponse>.Success(returned, "Save Success.");

        }


        [LoginFilter]
        [Route("actAsOwner")]
        [HttpPost]
        public APIResult<ActAsOwnerResponse> actAsOwner( string ID = "")
        {

            tbl_vehicles vehicle = new tbl_vehicles()
            {
                license_no = HttpContext.Current.Request.Form["license_no"],
                model = HttpContext.Current.Request.Form["model"],
                color = HttpContext.Current.Request.Form["color"],
                capacity = int.Parse(HttpContext.Current.Request.Form["capacity"].FirstOrDefault().ToString())
            };
             var returned = new ActAsOwnerResponse();
            
            if (!Request.Content.IsMimeMultipartContent())
            {
                return APIResult<ActAsOwnerResponse>.Error(ResponseCode.UserNotAcceptable, "Unsupported MediaType !");
            }

            var u = APIRequest.User(HttpContext.Current.Request);
            tbl_accounts acc;

            using (var ctx = new MainEntities())
            {
                acc = ctx.tbl_accounts.Find(u.Entity.id);
            }

            if (u.Entity.mail_verified != true) return APIResult<ActAsOwnerResponse>.Error(ResponseCode.UserUnVerified, "Your email not verified, You must verify your email!");

            if (acc.id_no == "" && ID == "") return APIResult<ActAsOwnerResponse>.Error(ResponseCode.UserValidationField, "ID number is required!");
 
            if (ID == "") ID = acc.id_no;

            long vID = 0;

            if (!long.TryParse(ID, out vID)) return APIResult<ActAsOwnerResponse>.Error(ResponseCode.UserValidationField, "Invalid ID number!");

            vehicle.created_at = DateTime.Now;
            vehicle.created_by = APIRequest.User(HttpContext.Current.Request).Entity.id;
            vehicle.owner_id = vehicle.created_by;

            using (var ctx=new MainEntities())
            {
                ctx.tbl_vehicles.Add(vehicle);

                acc.id_no = vID.ToString();

                ctx.tbl_accounts.Attach(acc);
                ctx.Entry(acc).State = System.Data.Entity.EntityState.Modified;

                if(u.hasRole("owner"))
                    ctx.sec_users_roles.Add(new sec_users_roles() { user_id = u.Entity.id, role_id = 2 });

               

                if (ctx.SaveChanges() > 0)
                {
                    if (u.hasRole("driver"))
                    {
                        ctx.tbl_drivers_vehicles_rel.Add(new tbl_drivers_vehicles_rel()
                        {
                            vehicle_id = vehicle.id,
                            created_at = DateTime.Now,
                            created_by = u.Entity.id,
                            driver_id = u.Entity.id,
                            status = 1
                        });

                        ctx.SaveChanges();
                    }
                    var httpRequest = HttpContext.Current.Request;
                    string errors="";

                    foreach (string img in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[img];

                        APIResult<tbl_images> result;
                        if (img == "ID_Image")
                        {
                            result = Images.SaveImageFromFile(postedFile, "tbl_accounts", u.Entity.id, "ID");
                        }
                        else
                        {
                            result = Images.SaveImageFromFile(postedFile, "tbl_vehicles", vehicle.id, "main");
                        }
                        if (!result.isSuccess) errors+=(errors==""?"":", ")+ result.message;
                    }


                    
                    using (var imgCtx=new MainEntities())
                    {
                        
                   
                    returned.vehicles = imgCtx.tbl_vehicles.Where(a => a.owner_id == acc.id && a.is_delete != true)
                                        .Select(
                                                c => new VehicleResponse()
                                                {
                                                    data = c,

                                                    images =new ImagesResponse()
                                                    {
                                                        Count = imgCtx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == c.id && d.model_tag == "main").Count(),
                                                        Url= (imgCtx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == c.id && d.model_tag == "main").Count() == 0)?"": "/img/scale/tbl_vehicles/" + c.id + "/original/main-{index}.gif"
                                                    }
                                                                           // .Select(b => "/img/scale/tbl_vehicles/"+ b.model_id + "/original/main-"++".gif").ToList(),
                                                }).ToList();

                    returned.ID = ID;                    
                    returned.ID_Image = string.Format("/img/scale/tbl_accounts/{0}/original/{1}-last.gif", acc.id, "ID");

                    }
                    return APIResult<ActAsOwnerResponse>.Success( returned, "Save success");
                }
                else
                {
                    return APIResult<ActAsOwnerResponse>.Error(ResponseCode.BackendDatabase, "Error while saving !");
                }
            }
            
        }

        [LoginFilter]
        //[Route("Account/Get")]
        [HttpGet]
        public APIResult<ProfileResponse> Get(int id)
        {

            using (var ctx = new DAL.MainEntities())
            {
                ProfileResponse profile = new ProfileResponse();

                profile.Account = ctx.vwProfile.Where(a => a.id == id).FirstOrDefault();

                if (profile.Account == null) return APIResult<ProfileResponse>.Error(ResponseCode.UserNotFound, "This account not found!");

               profile.Vehicles = ctx.tbl_vehicles.Where(a => a.owner_id == profile.Account.id && a.is_delete != true)
                                        .Select(
                                                c => new VehicleResponse()
                                                {
                                                    data = c,
                                                    
                                                    images = new ImagesResponse()
                                                    {
                                                        Count = ctx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == c.id && d.model_tag == "main").Count(),
                                                        Url = (ctx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == c.id && d.model_tag == "main").Count() == 0) ? "" : "/img/scale/tbl_vehicles/" + c.id + "/original/main-{index}.gif"
                                                    }
                                                    // .Select(b => "/img/scale/tbl_vehicles/"+ b.model_id + "/original/main-"++".gif").ToList(),
                                                }).ToList();
                var vchiclesIDs = profile.Vehicles.Select(b => b.data.id).ToList();

                var driversIDs = ctx.tbl_drivers_vehicles_rel.Where(a => vchiclesIDs.Contains((int)a.vehicle_id)).Select(c => c.driver_id).ToList();
                profile.Drivers = ctx.vwProfile.Where(a => driversIDs.Contains(a.id)).ToList();

            return APIResult<ProfileResponse>.Success(profile,"Data getted success");
            }
        }

        [HttpPost]
        [LoginFilter]
        [Route("EditMyProfile")]
        public APIResult<bool> EditMyProfile(tbl_accounts request)
        {
            using (var ctx=new MainEntities())
            {
                var u = APIRequest.User(HttpContext.Current.Request);
                var acc = ctx.tbl_accounts.Find(u.Entity.id);
                if (acc == null) return APIResult<bool>.Error(ResponseCode.DevNotFound, "This account not found!",false);

                acc.first_name = request.first_name;
                acc.last_name = request.last_name;
                acc.gender_id = request.gender_id;
                acc.city_id = request.city_id;
                acc.driver_license_no = request.driver_license_no;
                acc.id_no = request.id_no;
                acc.date_of_birth = request.date_of_birth;
                acc.country_id = request.country_id;

                ctx.Entry(acc).State = System.Data.Entity.EntityState.Modified;

                try
                {
                    ctx.SaveChanges();
                    return APIResult<bool>.Success(true);
                }
                catch (Exception ex)
                {

                    return APIResult<bool>.Error(ResponseCode.BackendDatabase, ex.Message,false);
                }

            }
         }

        [LoginFilter]
        [HttpPost]
        [Route("UploadDocs")]
        public APIResult<vwProfile> UploadDocs(string ID="", string DL="")
        {
            var u = APIRequest.User(HttpContext.Current.Request);
            var httpRequest = HttpContext.Current.Request;
            //if (!httpRequest.Files.AllKeys.Contains("ID_Image")) return APIResult<vwProfile>.Error(ResponseCode.UserValidationField, "File 'ID_Image' is required !");


            //if (!string.IsNullOrEmpty(DL))
            //{
            //    if (!httpRequest.Files.AllKeys.Contains("DL_Image")) return APIResult<vwProfile>.Error(ResponseCode.UserValidationField, "File 'DL_Image' is required !");
            //}

            //if(string.IsNullOrEmpty(ID))  return APIResult<vwProfile>.Error(ResponseCode.UserValidationField, "Field 'ID' is required !");

            if (!u.hasRole("driver"))
            {
                if (string.IsNullOrEmpty(ID)) return APIResult<vwProfile>.Error(ResponseCode.UserValidationField,"Field 'ID' is required !");

            }


            using (var ctx=new MainEntities())
            {

                var acc = ctx.tbl_accounts.Find(u.Entity.id);

                if (!string.IsNullOrEmpty(ID))
                    acc.id_no = ID;

                if (!string.IsNullOrEmpty(DL))
                    acc.driver_license_no = DL;
                string errors = "";
                foreach (string img in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[img];
                    string tag = "";
                    APIResult<tbl_images> result;
                    if (img == "ID_Image")
                    {
                        tag = "ID";
                    }
                    if (img == "DL_Image")
                    {
                        tag = "DL";
                    }

                    result = Images.SaveImageFromFile(postedFile, "tbl_accounts", acc.id, tag);

                    if (!result.isSuccess) errors += (errors == "" ? "" : ", ") + result.message;

                }
                if(!string.IsNullOrEmpty(errors)) return APIResult<vwProfile>.Error(ResponseCode.UserValidationField, errors);

                ctx.Entry(acc).State = System.Data.Entity.EntityState.Modified;


                if (ctx.sec_users_roles.Where(a => a.user_id == acc.id && a.role_id == 2).Count() == 0)
                    ctx.sec_users_roles.Add(new sec_users_roles() { user_id = acc.id, role_id = 2 });

                if(!string.IsNullOrEmpty(DL))
                if (ctx.sec_users_roles.Where(a => a.user_id == acc.id && a.role_id == 3).Count() == 0)
                    ctx.sec_users_roles.Add(new sec_users_roles() { user_id = acc.id, role_id = 3 });


                if (ctx.SaveChanges() == 0)
                {
                    return APIResult<vwProfile>.Error(ResponseCode.BackendDatabase, "UnKnow exception happend while saveing data !");
                }

                return APIResult<vwProfile>.Success(ctx.vwProfile.FirstOrDefault(a => a.id == acc.id));

            }

        }

        [LoginFilter]
        [Route("NeedRoles")]
        [HttpPost]
        public APIResult<vwProfile> NeedRoles(string roles, string ID = "", string DL = "")
        {
            var u = APIRequest.User(HttpContext.Current.Request);
            var httpRequest = HttpContext.Current.Request;
            
            using (var ctx=new MainEntities())
            {
                var acc = ctx.tbl_accounts.Find(u.Entity.id);

                if (acc == null) return APIResult<vwProfile>.Error(ResponseCode.UserNotFound, "Account not found");

                if (string.IsNullOrEmpty(acc.id_no)) {
                    if (string.IsNullOrEmpty(ID))
                    {
                        return APIResult<vwProfile>.Error(ResponseCode.UserValidationField, "Field 'ID' is required!");
                    }
                    else
                    {
                        acc.id_no = ID;
                    }
                }
                
                foreach (string role in roles.Split(','))
                {
                    if (role.Trim().ToLower() == "owner")
                    {
                        if (ctx.sec_users_roles.Where(a => a.user_id == acc.id && a.role_id == 2).Count() == 0)
                            ctx.sec_users_roles.Add(new sec_users_roles() { user_id = acc.id, role_id = 2 });
                        acc.id_no = ID;
                    }

                    if (role.Trim().ToLower() == "driver")
                    {
                        if (string.IsNullOrEmpty(acc.driver_license_no) && string.IsNullOrEmpty(DL))
                             return APIResult<vwProfile>.Error(ResponseCode.UserValidationField, "Field 'DL' is required!");
                        if (ctx.sec_users_roles.Where(a => a.user_id == acc.id && a.role_id == 3).Count() == 0)
                            ctx.sec_users_roles.Add(new sec_users_roles() { user_id = acc.id, role_id = 3 });
                        acc.driver_license_no = DL;
                    }


                }

                string errors = "";
                foreach (string img in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[img];
                    string tag = "";
                    APIResult<tbl_images> result;
                    if (img == "ID_Image")
                    {
                        tag = "ID";
                    }
                    if (img == "DL_Image")
                    {
                        tag = "DL";
                    }

                    result = Images.SaveImageFromFile(postedFile, "tbl_accounts", acc.id, tag);

                    if (!result.isSuccess) errors += (errors == "" ? "" : ", ") + result.message;

                }
                ctx.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                ctx.SaveChanges();
                return APIResult<vwProfile>.Success(ctx.vwProfile.FirstOrDefault(a => a.id == acc.id));

            }
        }

        [LoginFilter]
        [Route("MyDrivers")]
        [HttpGet]
        public APIResult<List<vwProfile>> MyDrivers()
        {
            using (var ctx=new MainEntities())
            {
                var u = APIRequest.User(HttpContext.Current.Request);
                List<int?> myDriverIDs = ctx.tbl_drivers_vehicles_rel.Where(a => ctx.tbl_vehicles.Where(b => b.owner_id == u.Entity.id && b.is_delete != true).Select(c => c.id).Contains((int)a.vehicle_id)).Select(a => a.driver_id).Distinct().ToList();

                return APIResult<List<vwProfile>>.Success(ctx.vwProfile.Where(a => myDriverIDs.Contains(a.id)).ToList());
            }
        }


        [LoginFilter]
        [Route("CameraStatus")]
        [HttpGet]
        public APIResult<bool> CameraStatus()
        {
            var u = APIRequest.User(HttpContext.Current.Request);
            return APIResult<bool>.Success(u.Entity.camera_status==null?false:(bool)u.Entity.camera_status);
        }
        [LoginFilter]
        [Route("CameraStatus")]
        [HttpPost]
        public APIResult<bool> CameraStatus(bool status)
        {
            var u = APIRequest.User(HttpContext.Current.Request);
            try
            {
                using (var ctx=new DAL.MainEntities())
                {
                    u.Entity.camera_status = status;
                    ctx.Entry(u.Entity).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();
                }
                return APIResult<bool>.Success(true);

            }
            catch (Exception ex)
            {
                return APIResult<bool>.Error(ResponseCode.BackendDatabase, ex.Message);
            }

              
           
        }
    }
}
