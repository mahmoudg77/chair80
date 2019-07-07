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

namespace Chair80.Controllers.vehicle
{
    [AppFilter]
    [RoutePrefix("{lang}/Vehicle")]
    public class VehicleController : ApiController
    {
        //public APIResult<IEnumerable<VehicleResponse>> Get()
        //{
        //    try
        //    {
        //        using (var ctx=new MainEntities())
        //        {
        //            var data = ctx.Set<DAL.tbl_vehicles>()
        //                .Select(img => new VehicleResponse()
        //                {
        //                    data = img,
        //                    images = new ImagesResponse()
        //                    {
        //                        Count = ctx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == img.id && d.model_tag == "main").Count(),
        //                        Url = (ctx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == img.id && d.model_tag == "main").Count() == 0) ? "" : "/img/scale/tbl_vehicles/" + img.id + "/original/main-{index}.gif"
        //                    }
        //                }).ToList();
        //            if (data == null)
        //            {
        //                return APIResult<IEnumerable<VehicleResponse>>.Error(ResponseCode.BackendDatabase, "Error while getting list !");
        //            }
        //            return APIResult<IEnumerable<VehicleResponse>>.Success( data, "API_SUCCESS");

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return APIResult<IEnumerable<VehicleResponse>>.Error(ResponseCode.BackendDatabase, ex.Message);
        //    }
        //}

        [AuthFilter("Get My Vehciles")]
        public APIResult<IEnumerable<VehicleResponse>> Get()
        {
            try
            {
                var u = APIRequest.User(HttpContext.Current.Request);
                using (var ctx = new MainEntities())
                {
                    var data = ctx.Set<DAL.tbl_vehicles>().Where(a=>a.owner_id==u.Entity.id && a.is_delete!=true)
                        .Select(img=>new VehicleResponse() {data=img,images=new ImagesResponse() {
                            Count = ctx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == img.id && d.model_tag == "main").Count(),
                            Url = (ctx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == img.id && d.model_tag == "main").Count() == 0) ? "" : "/img/scale/tbl_vehicles/" + img.id + "/original/main-{index}.gif"
                        }
                        }).ToList();
                    if (data == null)
                    {
                        return APIResult<IEnumerable<VehicleResponse>>.Error(ResponseCode.BackendDatabase, "Error while getting list !");
                    }
                    return APIResult<IEnumerable<VehicleResponse>>.Success(data, "API_SUCCESS");

                }
            }
            catch (Exception ex)
            {
                return APIResult<IEnumerable<VehicleResponse>>.Error(ResponseCode.BackendDatabase, ex.Message);
            }
        }

        [Route("Drive")]
        [HttpGet]
        [AuthFilter("Get Vehicle I Drive")]
        public APIResult<IEnumerable<VehicleResponse>> Drive()
        {
            try
            {
                var u = APIRequest.User(HttpContext.Current.Request);


                using (var ctx = new MainEntities())
                {
                    var ids = ctx.tbl_drivers_vehicles_rel.Where(a => a.driver_id == u.Entity.id).Select(a => a.vehicle_id).ToList();
                    var data = ctx.Set<DAL.tbl_vehicles>().Where(a => ids.Contains(a.id) && a.is_delete!=true)
                        .Select(img => new VehicleResponse()
                        {
                            data = img,
                            images = new ImagesResponse()
                            {
                                Count = ctx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == img.id && d.model_tag == "main").Count(),
                                Url = (ctx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == img.id && d.model_tag == "main").Count() == 0) ? "" : "/img/scale/tbl_vehicles/" + img.id + "/original/main-{index}.gif"
                            }
                        }).ToList();
                    if (data == null)
                    {
                        return APIResult<IEnumerable<VehicleResponse>>.Error(ResponseCode.BackendDatabase, "Error while getting list !");
                    }
                    return APIResult<IEnumerable<VehicleResponse>>.Success(data, "API_SUCCESS");

                }
            }
            catch (Exception ex)
            {
                return APIResult<IEnumerable<VehicleResponse>>.Error(ResponseCode.BackendDatabase, ex.Message);
            }
        }

        // GET api/values/5

        public APIResult<VehicleResponse> Get(int id)
        {
            try
            {
                using (var ctx = new MainEntities())
                {
                    var data = ctx.Set<DAL.tbl_vehicles>().Find(id);
                    var car=new VehicleResponse()
                     {
                         data = data,
                         images = new ImagesResponse()
                         {
                             Count = ctx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == data.id && d.model_tag == "main").Count(),
                             Url = (ctx.tbl_images.Where(d => d.model_name == "tbl_vehicles" && d.model_id == data.id && d.model_tag == "main").Count() == 0) ? "" : "/img/scale/tbl_vehicles/" + data.id + "/original/main-{index}.gif"
                         }
                     };
                    if (data == null)
                    {
                        return APIResult<VehicleResponse>.Error(ResponseCode.BackendDatabase, "Error while getting the vehicle data !");
                    }
                    return APIResult<VehicleResponse>.Success( car, "API_SUCCESS");
                }
            }
            catch (Exception ex)
            {
                return APIResult<VehicleResponse>.Error(ResponseCode.BackendDatabase, ex.Message);
            }
        }

        // POST api/values
        [AuthFilter("Add New Vehicle")]
        public APIResult<DAL.tbl_vehicles> Post(DAL.tbl_vehicles value)
        {
            try
            {
                var u = APIRequest.User(HttpContext.Current.Request);

                value.owner_id = u.Entity.id;
                value.created_at = DateTime.Now;
                value.created_by = u.Entity.id;
                
                using (var ctx = new MainEntities())
                {
                    ctx.Set<DAL.tbl_vehicles>().Add(value);
                    ctx.Entry(value).State = System.Data.Entity.EntityState.Added;

                    if (ctx.SaveChanges() <= 0)
                    {
                        if (u.hasRole("driver"))
                        {
                            ctx.tbl_drivers_vehicles_rel.Add(new tbl_drivers_vehicles_rel()
                            {
                                vehicle_id = value.id,
                                created_at = DateTime.Now,
                                created_by = u.Entity.id,
                                driver_id = u.Entity.id,
                                status = 1
                            });

                            ctx.SaveChanges();
                        }
                        return APIResult<DAL.tbl_vehicles>.Error(ResponseCode.BackendDatabase, "Error while saving data!");
                    }
                    return APIResult<DAL.tbl_vehicles>.Success(value, "API_SUCCESS");
                }
            }
            catch (Exception ex)
            {
                return APIResult<DAL.tbl_vehicles>.Error(ResponseCode.BackendDatabase, ex.Message);
            }
        }

        // PUT api/values/5
        [AuthFilter("Update Vehicle")]
        public APIResult<DAL.tbl_vehicles> Put(int id, DAL.tbl_vehicles value)
        {
            try
            {
                using (var ctx = new MainEntities())
                {
                    var u = APIRequest.User(HttpContext.Current.Request);

                    var v = ctx.tbl_vehicles.Find(id);

                    //v.id = id;
                    v.capacity = value.capacity;
                    v.color = value.color;
                    v.model = value.model;
                    v.license_no = value.license_no;
                    

                    //ctx.Set<DAL.tbl_vehicles>().Attach(v);
                    ctx.Entry(v).State = System.Data.Entity.EntityState.Modified;
                    var result = ctx.SaveChanges();
                    if (result<=0)
                    {
                        return APIResult<DAL.tbl_vehicles>.Error(ResponseCode.BackendDatabase, "Error while saving data!");

                    }
                    return APIResult<DAL.tbl_vehicles>.Success(value, "API_SUCCESS");
                }
            }
            catch (Exception ex)
            {
                return APIResult<DAL.tbl_vehicles>.Error(ResponseCode.BackendDatabase, ex.Message);
            }
        }

        // DELETE api/values/5
        [AuthFilter("Delete Vehicle")]
        public APIResult<bool> Delete(int id)
        {
            try
            {
                using (var ctx = new MainEntities())
                {
                    var obj = ctx.Set<DAL.tbl_vehicles>().Find(id);
                    obj.is_delete = true;
                    ctx.Entry(obj).State = System.Data.Entity.EntityState.Modified;

                    if (ctx.SaveChanges() <= 0)
                    {
                        return APIResult<bool>.Error(ResponseCode.BackendDatabase, "Error while saving data!",false);
                    }
                    return APIResult<bool>.Success(true, "API_SUCCESS");
                }
            }
            catch (Exception ex)
            {
                return APIResult<bool>.Error(ResponseCode.BackendDatabase, ex.Message);
            }
        }
    }
}
