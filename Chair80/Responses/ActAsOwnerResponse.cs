using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chair80.DAL;
namespace Chair80.Responses
{
    public class ActAsOwnerResponse
    {
        public string ID { get; set; }
        public string ID_Image { get; set; }
        public List<VehicleResponse> vehicles { get; set; }




    }
}