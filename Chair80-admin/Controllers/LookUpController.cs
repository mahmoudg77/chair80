using Chair80Admin.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chair80Admin.Controllers
{
    [AppFilter]
    [AuthFilter]
    [RoutePrefix("AdminApi")]
    public class CityController : CURDController<DAL.tbl_cities>
    {
       
    }
    [AppFilter]
    [AuthFilter]
    [RoutePrefix("AdminApi")]
    public class CountryController : CURDController<DAL.tbl_countries>
    {

    }
    [AppFilter]
    [AuthFilter]
    [RoutePrefix("AdminApi")]
    public class GenderController : CURDController<DAL.tbl_genders>
    {

    }
}
