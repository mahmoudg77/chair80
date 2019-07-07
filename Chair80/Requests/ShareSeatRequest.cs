using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chair80.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class ShareSeatRequest : APIRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Trip Type is required!")]
        public TripTypes trip_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Seats is required!")]
        public int seats { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MapPlace from { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MapPlace to { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? start_at { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int gender_id { get; set; }
        /// <summary>
        /// If trip type =2
        /// </summary>
        public DateTime? round_at { get; set; }
        /// <summary>
        /// If trip type =3
        /// </summary>
        public DateTime? shuttle_at { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? seat_cost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Vehicle is required!")]
        public int vehicle_id { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TripTypes
    {
        /// <summary>
        /// 
        /// </summary>
        OneWay=1,
        /// <summary>
        /// 
        /// </summary>
        Round=2,
        /// <summary>
        /// 
        /// </summary>
        Shuttle=3

    }

    /// <summary>
    /// 
    /// </summary>
    public class MapPlace
    {
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Long is required!")]
        public decimal lng { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Lat is required!")]
        public decimal lat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Country is required!")]
        public string country { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "City is required!")]
        public string city { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Street is required!")]
        public string street { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Governorate number is required!")]
        public string governorate { get; set; }
    }
}