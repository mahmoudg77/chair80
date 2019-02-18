using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chari80.Requests
{
    public class ActAsDriverRequest : APIRequest
    {
        [Required(ErrorMessage = "ID is required!")]
        public string ID { get; set; }

        [Required(ErrorMessage = "driver_lisence is required!")]
        public string driver_lisence { get; set; }

    }
}