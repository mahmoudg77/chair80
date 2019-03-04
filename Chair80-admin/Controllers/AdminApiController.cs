using Chair80Admin.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chair80Admin.Controllers
{
    public abstract class AdminApiController<T> : ApiController  where T : class
    {
        DAL.MainEntities ctx = new DAL.MainEntities();
        // GET api/values
        public APIResult<IEnumerable<T>> Get()
        {
            try
            {
                var data = ctx.Set<T>().ToList();
                if (data == null)
                {
                    return new APIResult<IEnumerable<T>>(ResultType.fail, "Error while getting list !");
                }
                return new APIResult<IEnumerable<T>>(ResultType.success, data, "API_SUCCESS");
            }
            catch (Exception ex)
            {
                return new APIResult<IEnumerable<T>>(ResultType.fail, ex.Message);
            }
        }

        // GET api/values/5
        public APIResult<T> Get(int id)
        {
            var data = ctx.Set<T>().Find(id);
            try
            {
                if (data == null)
                {
                    return new APIResult<T>(ResultType.fail, "Error while getting list !");
                }
                return new APIResult<T>(ResultType.success, data, "API_SUCCESS");
            }
            catch (Exception ex)
            {
                return new APIResult<T>(ResultType.fail, ex.Message);
            }
        }

        // POST api/values
        public APIResult<T> Post(T value)
        {
            try
            {
                ctx.Set<T>().Add(value);
                ctx.Entry(value).State = System.Data.Entity.EntityState.Added;

                if (ctx.SaveChanges() <= 0)
                {
                    return new APIResult<T>(ResultType.fail, value, "Error while saving data!");
                }
                return new APIResult<T>(ResultType.success, value, "API_SUCCESS");
            }
            catch (Exception ex)
            {
                return new APIResult<T>(ResultType.fail, value, ex.Message);
            }
        }

        // PUT api/values/5
        public APIResult<T> Put(int id, T value)
        {
            try
            {
                ctx.Set<T>().Attach(value);
                ctx.Entry(value).State = System.Data.Entity.EntityState.Modified;

                if (ctx.SaveChanges() <= 0)
                {
                    return new APIResult<T>(ResultType.fail, value, "Error while saving data!");

                }
                return new APIResult<T>(ResultType.success, value, "API_SUCCESS");
            }
            catch (Exception ex)
            {
                return new APIResult<T>(ResultType.fail, value, ex.Message);
            }
        }

        // DELETE api/values/5
        public APIResult<bool> Delete(int id)
        {
            try
            {
                var obj = ctx.Set<T>().Find(id);
                ctx.Entry(obj).State = System.Data.Entity.EntityState.Deleted;

                if (ctx.SaveChanges() <= 0)
                {
                    return new APIResult<bool>(ResultType.fail, false, "Error while saving data!");
                }
                return new APIResult<bool>(ResultType.success, true, "API_SUCCESS");
            }
            catch (Exception ex)
            {
                return new APIResult<bool>(ResultType.fail, false, ex.Message);
            }
        }
    }
}
