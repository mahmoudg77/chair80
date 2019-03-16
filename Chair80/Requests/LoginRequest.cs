using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chair80.Requests
{
    public class LoginRequest: APIRequest
    {
        [Required(ErrorMessage = "OTP Code is required!")]
        public string otpcode { get; set; }

        [Required(ErrorMessage = "Phone Number is required!")]
        public string phoneNumber { get; set; }

    }
}