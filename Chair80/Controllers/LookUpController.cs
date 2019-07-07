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
    [AppFilter]
    //[AuthFilter]
    public class CityController : CURDController<DAL.tbl_cities>{

        public override  APIResult<IEnumerable<DAL.tbl_cities>> Get(bool master = false)
        {

            var d=base.Get(master);
            if (!d.isSuccess) return d;

            var param = Request.GetQueryNameValuePairs().Where(a => a.Key == "country_id");
            if (param == null || param.Count()==0) return d;

            int country_id = int.Parse(param.FirstOrDefault().Value);
            return APIResult<IEnumerable<DAL.tbl_cities>>.Success(d.data.Where(a => a.country_id == country_id).ToList());
        }
    }
    [AppFilter]
    //[AuthFilter]
    public class CountryController : CURDController<DAL.tbl_countries> { }
    [AppFilter]
    //[AuthFilter]
    public class GenderController : CURDController<DAL.tbl_genders> { } //[AuthFilter]
    [AppFilter]
    public class RateReasonController : CURDController<DAL.tbl_rate_reasons> { }
}
