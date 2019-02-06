using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chari80.Controllers
{
    public abstract class ClientApiController<T> : ApiController where T:class
    {
        DAL.MainEntities ctx = new DAL.MainEntities();
        // GET api/values
        public IEnumerable<T> Get()
        {
             return ctx.Set<T>().ToList();
        }

        // GET api/values/5
        public T Get(int id)
        {
           return ctx.Set<T>().Find(id);
        }

        // POST api/values
        public void Post(T value)
        {
            ctx.Set<T>().Add(value);
            ctx.Entry(value).State = System.Data.Entity.EntityState.Added;
            ctx.SaveChanges();
        }

        // PUT api/values/5
        public void Put(int id, T value)
        {
            ctx.Set<T>().Attach(value);
            ctx.Entry(value).State = System.Data.Entity.EntityState.Modified;
            ctx.SaveChanges();
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            var obj=ctx.Set<T>().Find(id);
            ctx.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
            ctx.SaveChanges();
        }
    }
}
