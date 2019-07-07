using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chair80.Requests
{
    public class RegisterRequest:APIRequest
    {
      
        [Required(ErrorMessage = "Firebase access_token is required!")]
        public string access_token { get; set; }

        [Required(ErrorMessage = "First name is required!")]
        public string first_name { get; set; }

        [Required(ErrorMessage = "Last name is required!")]
        public string last_name { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string password { get; set; }

        public DateTime? date_of_birth { get; set; }

        [Required(ErrorMessage = "Gender is required!")]
        public int? gender_id { get; set; }

        [Required(ErrorMessage = "Phone Number is required!")]
        public string phoneNumber { get; set; }

        [Required(ErrorMessage = "verification_id is required!")]
        public Guid verification_id { get; set; }

        public int? country { get; set; }
        public int? city { get; set; }

    }
}