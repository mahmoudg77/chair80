using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chair80.Requests
{
    public class ActAsDriverRequest : APIRequest
    {
        [Required(ErrorMessage = "ID is required!")]
        public string ID { get; set; }

        [Required(ErrorMessage = "driver_lisence is required!")]
        public string DL { get; set; }
        [Required(ErrorMessage = "ID_Image is required!")]
        public HttpPostedFileBase ID_Image { get; set; }
        [Required(ErrorMessage = "DL_Image is required!")]
        public HttpPostedFileBase DL_Image { get; set; }
    }
}