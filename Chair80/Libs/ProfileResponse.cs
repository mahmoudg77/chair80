using Chair80.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Libs
{
    public class ProfileResponse 
    {
       
        public DAL.vwProfile Account { get; set; }

        public List<VehicleResponse> Vehicles { get; set; }
        
        public List<DAL.vwProfile> Drivers { get; set; }


    }
}