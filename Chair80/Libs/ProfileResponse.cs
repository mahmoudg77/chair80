using Chair80.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Libs
{
    public class ProfileResponse:DAL.tbl_accounts
    {
        public ProfileResponse(DAL.tbl_accounts acc)
        {
            foreach(var prop in typeof(DAL.tbl_accounts).GetProperties())
            {
                typeof(ProfileResponse).GetProperty(prop.Name).SetValue(this, prop.GetValue(acc));
            }
        }
        public bool isDriver { get; set; }
        public bool isOwner { get; set; }

        public List<VehicleResponse> Vehicles { get; set; }

        public string ID_Image{ get; set; }
        public string DL_Image{ get; set; }

        
    }
}