using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Chair80.Requests
{
    public class FirebaseAuthRequest: APIRequest
    {
        [Required(ErrorMessage = "Network is required!")]
        public string network { get; set; }

        [Required(ErrorMessage = "Firebase Token is required!")]
        public string firebaseToken { get; set; }

        //[Required(ErrorMessage = "Firebase Token is required!")]
        //public string firebase_token { get; set; }


    }
   
}