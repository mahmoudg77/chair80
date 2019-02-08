using Chari80.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chari80.Controllers
{
    [AppFilter]
    [AuthFilter]
    public class CityController : AdminApiController<DAL.tbl_cities>
    {
       
    }
    [AppFilter]
    [AuthFilter]
    public class CountryController : AdminApiController<DAL.tbl_countries>
    {

    }
    [AppFilter]
    [AuthFilter]
    public class GenderController : AdminApiController<DAL.tbl_genders>
    {

    }
}
