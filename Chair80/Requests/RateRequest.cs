using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chair80.Requests
{
    public class RateRequest: APIRequest
    {
        [Required(ErrorMessage = "OTP Code is required!")]
        public int rate { get; set; }

        public string comment { get; set; }

        public int? reason_id { get; set; }

    }
}