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
    
    public partial class vwTripsDetails:BLL.vwTripsDetailsTranslate
    {
        public Nullable<int> seats { get; set; }
        public Nullable<System.DateTime> start_at_date { get; set; }
        public Nullable<System.TimeSpan> start_at_time { get; set; }
        public Nullable<decimal> from_lat { get; set; }
        public Nullable<decimal> from_lng { get; set; }
        public string from_plc { get; set; }
        public Nullable<decimal> to_lat { get; set; }
        public Nullable<decimal> to_lng { get; set; }
        public string to_plc { get; set; }
        public Nullable<int> trip_gender_id { get; set; }
        public Nullable<int> booked_seats { get; set; }
        public Nullable<decimal> seat_cost { get; set; }
        public Nullable<int> trip_direction { get; set; }
        public Nullable<bool> is_active { get; set; }
        public int trip_share_id { get; set; }
        public int driver_id { get; set; }
        public Nullable<System.DateTime> trip_start_date { get; set; }
        public Nullable<System.DateTime> trip_end_date { get; set; }
        public Nullable<int> car_id { get; set; }
        public string car_model { get; set; }
        public string car_color { get; set; }
        public int car_capacity { get; set; }
        public string car_license_no { get; set; }
        public Nullable<int> trip_type_id { get; set; }
        //public string trip_type { get; set; }
        public string acc_firebase_uid { get; set; }
        public string acc_first_name { get; set; }
        public string acc_last_name { get; set; }
        public Nullable<System.DateTime> acc_date_of_birth { get; set; }
        public string acc_mobile { get; set; }
        public string acc_email { get; set; }
        public Nullable<System.DateTime> acc_register_time { get; set; }
        public Nullable<int> acc_gender_id { get; set; }
        public string acc_id_no { get; set; }
        public string acc_driver_license_no { get; set; }
        public Nullable<int> acc_city_id { get; set; }
        public Nullable<int> acc_country_id { get; set; }
        //public string acc_gender { get; set; }
        //public string acc_city { get; set; }
        //public string acc_country { get; set; }
        public int trip_id { get; set; }
        public string trip_gender_name { get; set; }
        public Nullable<System.Guid> guid { get; set; }
        public Nullable<int> started_seats { get; set; }
        public Nullable<int> ended_seats { get; set; }
        public Nullable<int> reached_seats { get; set; }
    }
}
