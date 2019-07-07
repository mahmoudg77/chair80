using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Responses
{
    public class DriverCurrentTrips
    {
        public int ID { get; set; }

        public DAL.vwProfile Rider { get; set; }

        public DateTime booked_at { get; set; }

        public DateTime start_at { get; set; }
        public DateTime end_at { get; set; }

        public Guid trip_token { get; set; }

        public int seats { get; set; }
       

    }
}