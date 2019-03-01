using Chari80Admin.BLL.Security;
using Chari80Admin.DAL;
using System;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Chari80Admin.Requests
{
    public  class APIRequest
    {
        public  Users CurrentUser(HttpRequestMessage request)
        {
            var AuthKey = request.Headers.GetValues("AUTH_KEY");
            using (MainEntities ctx=new MainEntities())
            {
                var result = ctx.sec_sessions.Find(Guid.Parse(AuthKey.First().ToString()));
                if (result == null || result.end_time != null) return null;
                return new Users(result.user_id);
            }

        }
        public static Users User(HttpRequest request)
        {
            try
            {

            
                var AuthKey = request.Headers.GetValues("AUTH_KEY");
                if (AuthKey == null) return null;
                using (MainEntities ctx = new MainEntities())
                {
                    var result = ctx.sec_sessions.Find(Guid.Parse(AuthKey.First().ToString()));
                    if (result ==null || result.end_time != null) return null;

                    return new Users(result.user_id);
                }

            }
            catch (Exception)
            {

                return null;
            }
        }
      
    }
}
