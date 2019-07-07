using Chair80.Filters;
using Chair80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chair80.Controllers
{
    [RoutePrefix("AdminApi")]
    public abstract class CURDController<T> : ApiController  where T : class
    {
        DAL.MainEntities ctx = new DAL.MainEntities();
        // GET api/values
        public virtual APIResult<IEnumerable<T>> Get(bool master=false)
        {
            try
            {

                var data = ctx.Set<T>().ToList();
                if (data == null)
                {
                    return APIResult<IEnumerable<T>>.Error(ResponseCode.UserNotFound, "Object not found !");
                }

                if(master)
                data.ForEach(a =>
                {
                    if (typeof(T).BaseType.Name.Contains("Translate"))
                    {
                        typeof(T).BaseType.GetProperty("GetMasterField").SetValue(a, true);
                    }
                });

                return APIResult<IEnumerable<T>>.Success(data, "API_SUCCESS");
            }
            catch (Exception ex)
            {
                return APIResult<IEnumerable<T>>.Error(ResponseCode.BackendDatabase, ex.Message);
            }
        }

        // GET api/values/5
        public virtual APIResult<T> Get(int id,bool master=false)
        {
            var data = ctx.Set<T>().Find(id);
            if (master)
                if(typeof(T).BaseType.Name.Contains("Translate"))
                    {
                        typeof(T).BaseType.GetProperty("GetMasterField").SetValue(data, true);
                    }
            try
            {
                if (data == null)
                {
                    return APIResult<T>.Error(ResponseCode.BackendDatabase, "Error while getting list !");
                }
                return APIResult<T>.Success(data, "API_SUCCESS");
            }
            catch (Exception ex)
            {
                return APIResult<T>.Error(ResponseCode.BackendDatabase, ex.Message);
            }
        }

        // POST api/values
        [AuthFilter("Add")]
        public virtual APIResult<T> Post(T value)
        {
            try
            {
                if (typeof(T).BaseType.Name.Contains("Translate"))
                {
                    typeof(T).BaseType.GetProperty("GetMasterField").SetValue(value, true);
                }

                ctx.Set<T>().Add(value);
                ctx.Entry(value).State = System.Data.Entity.EntityState.Added;

                if (ctx.SaveChanges() <= 0)
                {
                    return APIResult<T>.Error(ResponseCode.BackendDatabase, "Error while saving data!");
                }

                if (typeof(T).BaseType.Name.Contains("Translate"))
                {
                    typeof(T).BaseType.GetProperty("GetMasterField").SetValue(value, false);
                }

                return APIResult<T>.Success(value, "API_SUCCESS");
            }
            catch (Exception ex)
            {
                return APIResult<T>.Error(ResponseCode.BackendDatabase, ex.Message);
            }
        }

        // PUT api/values/5
        [AuthFilter("Edit")]
        public virtual APIResult<T> Put(int id, T value)
        {
            try
            {
                if (typeof(T).BaseType.Name.Contains("Translate"))
                {
                    typeof(T).BaseType.GetProperty("GetMasterField").SetValue(value, true);
                }


                typeof(T).GetProperty("id").SetValue(value, id);
              
                
                ctx.Set<T>().Attach(value);
                ctx.Entry(value).State = System.Data.Entity.EntityState.Modified;

                if (ctx.SaveChanges() <= 0)
                {
                    return APIResult<T>.Error(ResponseCode.BackendDatabase, "Error while saving data!");
                }
                if (typeof(T).BaseType.Name.Contains("Translate"))
                {
                    typeof(T).BaseType.GetProperty("GetMasterField").SetValue(value, false);
                }
                return APIResult<T>.Success(value, "API_SUCCESS");
            }
            catch (Exception ex)
            {
                return APIResult<T>.Error(ResponseCode.BackendDatabase, ex.Message);
            }
        }

        // DELETE api/values/5
        [AuthFilter("Delete")]
        public virtual APIResult<bool> Delete(int id)
        {
            try
            {
                var obj = ctx.Set<T>().Find(id);
                ctx.Entry(obj).State = System.Data.Entity.EntityState.Deleted;

                if (ctx.SaveChanges() <= 0)
                {
                    return APIResult<bool>.Error(ResponseCode.BackendDatabase, "Error while saving data!");
                }
                return APIResult<bool>.Success(true, "API_SUCCESS");
            }
            catch (Exception ex)
            {
                return APIResult<bool>.Error(ResponseCode.BackendDatabase, ex.Message);
            }
        }
    }
}
