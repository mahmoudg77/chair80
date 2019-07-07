using Chair80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Chair80.Controllers.Trips
{
    [RoutePrefix("{lang}/Google")]
    public class GoogleController : ApiController
    {
        // GET: Google
        [HttpGet]
        [Route("Direction")]
        public async Task<APIResult<object>> Direction(string origin,string destination, string mode="driving")
        {
            string url = "https://maps.googleapis.com/maps/api/directions/json?origin=" + origin + "&destination=" + destination + "&mode=" + mode + "&key=" + Settings.Get("go_dir_key");

            Uri uri = new Uri(url);
            using (HttpClient w = new HttpClient())
            {

                var response = await w.PostAsJsonAsync(uri,"");

                var smsResult = await response.Content.ReadAsStringAsync();

                var s= w.GetStringAsync(url);
           

                return APIResult<object>.Success(Newtonsoft.Json.JsonConvert.DeserializeObject( s.Result));
            }
        }
    }
}