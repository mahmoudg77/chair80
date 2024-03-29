//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AllData.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class trip_book
    {
        public int id { get; set; }
        public int trip_share_details_id { get; set; }
        public int trip_request_details_id { get; set; }
        public Nullable<System.DateTime> booked_at { get; set; }
        public Nullable<System.DateTime> start_at { get; set; }
        public Nullable<System.DateTime> end_at { get; set; }
        public Nullable<int> driver_rate { get; set; }
        public Nullable<int> rider_rate { get; set; }
        public string trip_token { get; set; }
        public Nullable<int> seats { get; set; }
    
        public virtual trip_request_details trip_request_details { get; set; }
        public virtual trip_share_details trip_share_details { get; set; }
    }
}
