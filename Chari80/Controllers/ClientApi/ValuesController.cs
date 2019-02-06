using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chari80.Controllers
{
    public class CityController : ClientApiController<DAL.tbl_cities>
    {
       
    }
    public class CountryController : ClientApiController<DAL.tbl_countries>
    {

    }
    public class GenderController : ClientApiController<DAL.tbl_genders>
    {

    }
}
