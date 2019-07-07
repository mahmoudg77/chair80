using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Chair80.Requests
{
    public class WebLoginRequest: APIRequest
    {
        [Required(ErrorMessage = "Email is required!")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string password { get; set; }

        //[Required(ErrorMessage = "Firebase Token is required!")]
        //public string firebase_token { get; set; }


    }
   
}