using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chari80.Requests
{
    public class LoginRequest: APIRequest
    {
        [Required(ErrorMessage = "Firebase access_token is required!")]
        public string access_token { get; set; }
        [Required(ErrorMessage ="Password is required!")]
        public string password { get; set; }

    }
}