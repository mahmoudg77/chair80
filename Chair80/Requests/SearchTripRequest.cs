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
    public class SearchTripRequest : APIRequest
    {

        /// <summary>
        /// 
        /// </summary>
        public string[] firebase_ids { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TripTypes trip_type { get; set; }
      
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
        public decimal? seat_cost_from { get; set; }
        public decimal? seat_cost_to { get; set; }




    }

}