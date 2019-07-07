using Chair80.BLL.Security;
using Chair80.DAL;
using Chair80.Libs;
using System;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Chair80.Requests
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
        public APIResult<bool> isValid()
        {
            var props=this.GetType().GetProperties().Where(a => a.CustomAttributes.Where(at => at.AttributeType.Name == "RequiredAttribute").Count() > 0);
            foreach (var item in props)
            {
                if (item.GetValue(this) == null) return APIResult<bool>.Error(ResponseCode.BackendInternalServer, item.CustomAttributes.Where(at =>at.AttributeType.Name == "RequiredAttribute").FirstOrDefault().NamedArguments.First(a => a.MemberName == "ErrorMessage").TypedValue.Value.ToString());
            }
            return APIResult<bool>.Success(true);
        }
      
    }
}
