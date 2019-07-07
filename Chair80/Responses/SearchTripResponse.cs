using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Responses
{
    public class SearchTripResponse
    {
        public string firebase_id { get; set; }

        public DAL.trip_share trip { get; set; }
        public DAL.trip_share_details trip_details { get; set; }

    }
}