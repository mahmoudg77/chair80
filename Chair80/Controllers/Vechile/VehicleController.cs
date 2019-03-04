using Chair80.DAL;
using Chair80.Filters;
using Chair80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chair80.Controllers.vehicle
{
    [AppFilter]
    [AuthFilter]
    public class VehicleController : ApiController
    {
        public APIResult<IEnumerable<DAL.tbl_vehicles>> Get()
        {
            try
            {
                using (var ctx=new MainEntities())
                {
                    var data = ctx.Set<DAL.tbl_vehicles>().ToList();
                    if (data == null)
                    {
                        return new APIResult<IEnumerable<DAL.tbl_vehicles>>(ResultType.fail, "Error while getting list !");
                    }
                    return new APIResult<IEnumerable<DAL.tbl_vehicles>>(ResultType.success, data, "API_SUCCESS");

                }
            }
            catch (Exception ex)
            {
                return new APIResult<IEnumerable<DAL.tbl_vehicles>>(ResultType.fail, ex.Message);
            }
        }

        // GET api/values/5
        public APIResult<DAL.tbl_vehicles> Get(int id)
        {
            try
            {
                using (var ctx = new MainEntities())
                {
                    var data = ctx.Set<DAL.tbl_vehicles>().Find(id);
                    if (data == null)
                    {
                        return new APIResult<DAL.tbl_vehicles>(ResultType.fail, "Error while getting list !");
                    }
                    return new APIResult<DAL.tbl_vehicles>(ResultType.success, data, "API_SUCCESS");
                }
            }
            catch (Exception ex)
            {
                return new APIResult<DAL.tbl_vehicles>(ResultType.fail, ex.Message);
            }
        }

        // POST api/values
        public APIResult<DAL.tbl_vehicles> Post(DAL.tbl_vehicles value)
        {
            try
            {
                using (var ctx = new MainEntities())
                {
                    ctx.Set<DAL.tbl_vehicles>().Add(value);
                    ctx.Entry(value).State = System.Data.Entity.EntityState.Added;

                    if (ctx.SaveChanges() <= 0)
                    {
                        return new APIResult<DAL.tbl_vehicles>(ResultType.fail, value, "Error while saving data!");
                    }
                    return new APIResult<DAL.tbl_vehicles>(ResultType.success, value, "API_SUCCESS");
                }
            }
            catch (Exception ex)
            {
                return new APIResult<DAL.tbl_vehicles>(ResultType.fail, value, ex.Message);
            }
        }

        // PUT api/values/5
        public APIResult<DAL.tbl_vehicles> Put(int id, DAL.tbl_vehicles value)
        {
            try
            {
                using (var ctx = new MainEntities())
                {
                    ctx.Set<DAL.tbl_vehicles>().Attach(value);
                    ctx.Entry(value).State = System.Data.Entity.EntityState.Modified;

                    if (ctx.SaveChanges() <= 0)
                    {
                        return new APIResult<DAL.tbl_vehicles>(ResultType.fail, value, "Error while saving data!");

                    }
                    return new APIResult<DAL.tbl_vehicles>(ResultType.success, value, "API_SUCCESS");
                }
            }
            catch (Exception ex)
            {
                return new APIResult<DAL.tbl_vehicles>(ResultType.fail, value, ex.Message);
            }
        }

        // DELETE api/values/5
        public APIResult<bool> Delete(int id)
        {
            try
            {
                using (var ctx = new MainEntities())
                {
                    var obj = ctx.Set<DAL.tbl_vehicles>().Find(id);
                    ctx.Entry(obj).State = System.Data.Entity.EntityState.Deleted;

                    if (ctx.SaveChanges() <= 0)
                    {
                        return new APIResult<bool>(ResultType.fail, false, "Error while saving data!");
                    }
                    return new APIResult<bool>(ResultType.success, true, "API_SUCCESS");
                }
            }
            catch (Exception ex)
            {
                return new APIResult<bool>(ResultType.fail, false, ex.Message);
            }
        }
    }
}
