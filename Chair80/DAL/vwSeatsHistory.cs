//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chair80.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class vwSeatsHistory
    {
        public string driver_name { get; set; }
        public string rider_name { get; set; }
        public int driver_id { get; set; }
        public Nullable<int> rider_id { get; set; }
        public Nullable<System.DateTime> started_at { get; set; }
        public Nullable<System.DateTime> ended_at { get; set; }
        public string from_plc { get; set; }
        public Nullable<decimal> from_lng { get; set; }
        public Nullable<decimal> from_lat { get; set; }
        public string to_plc { get; set; }
        public Nullable<decimal> to_lng { get; set; }
        public Nullable<decimal> to_lat { get; set; }
        public Nullable<int> driver_rate { get; set; }
        public Nullable<int> rider_rate { get; set; }
        public Nullable<int> seats { get; set; }
        public Nullable<int> trip_type_id { get; set; }
        public Nullable<System.DateTime> canceled_at { get; set; }
        public Nullable<System.DateTime> start_at_date { get; set; }
        public Nullable<int> book_id { get; set; }
        public Nullable<decimal> seat_cost { get; set; }
        public Nullable<System.Guid> guid { get; set; }
    }
}
