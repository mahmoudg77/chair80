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
    public class AccountController : ApiController
    {
        [LoginFilter]
        [Route("Account/actAsDriver")]
        [HttpPost]
        public APIResult<ActAsDriverResponse> actAsDriver(string DL, string ID="")
        {

            if (!Request.Content.IsMimeMultipartContent())
            {
                return new APIResult<ActAsDriverResponse>(ResultType.fail, null, "Unsupported MediaType !");
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

            if (u.mail_verified!=true) return new APIResult<ActAsDriverResponse>(ResultType.warning, null, "Your email not verified, You must verify your email!");

            if (acc.id_no=="" && ID=="") return new APIResult<ActAsDriverResponse>(ResultType.fail, null, "ID number is required!");
            if (DL =="") return new APIResult<ActAsDriverResponse>(ResultType.fail, null, "Driver Lisence number is required!");


            if (ID_Image == null && ID !="") return new APIResult<ActAsDriverResponse>(ResultType.fail, null, "ID Image file is required !");
            if (DL_Image == null) return new APIResult<ActAsDriverResponse>(ResultType.fail, null, "Driver Lisence Image file is required !");

            if (ID == "") ID = acc.id_no;

            long vID = 0;
            long vDL = 0;

            if (!long.TryParse(ID,out vID)) return new APIResult<ActAsDriverResponse>(ResultType.fail, null, "Invalid ID number!");

            if (!long.TryParse(DL,out vDL)) return new APIResult<ActAsDriverResponse>(ResultType.fail, null, "Invalid Driver Lisence!");
             

            var returned =new ActAsDriverResponse() { DL = vDL.ToString(), ID = vID.ToString() };

            var result = Images.SaveImageFromFile(ID_Image, "tbl_accounts", u.id, "ID");

            if (result.type != ResultType.success) return new APIResult<ActAsDriverResponse>(ResultType.fail, null, result.message);
            returned.ID_Image = string.Format("/img/scale/tbl_accounts/{0}/original/{1}-0.gif",acc.id,"ID");
            result = Images.SaveImageFromFile(DL_Image, "tbl_accounts", u.id, "DL");
            returned.DL_Image = string.Format("/img/scale/tbl_accounts/{0}/original/{1}-0.gif", acc.id, "DL");

            if (result.type != ResultType.success) return new APIResult<ActAsDriverResponse>(ResultType.fail, null, result.message);

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

                if (ctx.SaveChanges() == 0) return new APIResult<ActAsDriverResponse>(ResultType.fail, null, "Error while saveing data!");
                 
            }
            return new APIResult<ActAsDriverResponse>(ResultType.success, returned, "Save Success.");

        }


        [LoginFilter]
        [Route("Account/actAsOwner")]
        [HttpPost]
        public APIResult<ActAsOwnerResponse> actAsOwner( string ID = "")
        {

            tbl_vehicles vehicle = new tbl_vehicles()
            {
                license_no = HttpContext.Current.Request.Form["license_no"],
                model = HttpContext.Current.Request.Form["model"],
                color = HttpContext.Current.Request.Form["color"],
                capacity = int.Parse(HttpContext.Current.Request.Form["capacity"].ToString())
            };
             var returned = new ActAsOwnerResponse();
            
            if (!Request.Content.IsMimeMultipartContent())
            {
                return new APIResult<ActAsOwnerResponse>(ResultType.fail, returned, "Unsupported MediaType !");
            }

            var u = APIRequest.User(HttpContext.Current.Request).Entity;
            tbl_accounts acc;

            using (var ctx = new MainEntities())
            {
                acc = ctx.tbl_accounts.Find(u.id);
            }

            if (u.mail_verified != true) return new APIResult<ActAsOwnerResponse>(ResultType.warning, returned, "Your email not verified, You must verify your email!");

            if (acc.id_no == "" && ID == "") return new APIResult<ActAsOwnerResponse>(ResultType.fail, returned, "ID number is required!");
 
            if (ID == "") ID = acc.id_no;

            long vID = 0;

            if (!long.TryParse(ID, out vID)) return new APIResult<ActAsOwnerResponse>(ResultType.fail, returned, "Invalid ID number!");

            vehicle.created_at = DateTime.Now;
            vehicle.created_by = APIRequest.User(HttpContext.Current.Request).Entity.id;
            vehicle.owner_id = vehicle.created_by;

            using (var ctx=new MainEntities())
            {
                ctx.tbl_vehicles.Add(vehicle);

                acc.id_no = vID.ToString();

                ctx.tbl_accounts.Attach(acc);
                ctx.Entry(acc).State = System.Data.Entity.EntityState.Modified;

                if (ctx.SaveChanges() > 0)
                {
                    var httpRequest = HttpContext.Current.Request;
                    string errors="";

                    foreach (string img in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[img];

                        var result = Images.SaveImageFromFile(postedFile, "tbl_vehicles", vehicle.id, "main");
                        if (result.type != ResultType.success) errors+=(errors==""?"":", ")+ result.message;
                    }

                    using (var imgCtx=new MainEntities())
                    {

                   
                    returned.vehicles = imgCtx.tbl_vehicles.Where(a => a.owner_id == acc.id)
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
                    returned.ID_Image = string.Format("/img/scale/tbl_accounts/{0}/original/{1}-0.gif", acc.id, "ID");

                    }
                    return new APIResult<ActAsOwnerResponse>(ResultType.success, returned, "Save success");
                }
                else
                {
                    return new APIResult<ActAsOwnerResponse>(ResultType.fail, returned, "Error while saving !");
                }
            }
            
        }
    }
}
