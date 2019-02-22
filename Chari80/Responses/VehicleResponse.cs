using Chari80.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chari80.Responses
{
    public class VehicleResponse
    {
        public tbl_vehicles data { get; set; }
        public ImagesResponse images { get; set; }
    }

    public class ImagesResponse
    {
        public int Count { get; set; }
        public string Url { get; set; }
    }
}