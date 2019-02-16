using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chari80.Libs
{
    public class IPResult
    {
        public string @as { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string isp { get; set; }
        public decimal lat { get; set; }
        public decimal lon { get; set; }
        public string org { get; set; }
        public string query { get; set; }
        public string region { get; set; }
        public string regionName { get; set; }
        public string status { get; set; }
        public string timezone { get; set; }
        public string zip { get; set; }
       
    }
    public class trans
    {
        public string lang { get; set; }
        public string value { get; set; }
        public trans()
        {

        }
        public trans(string _lang,string _value)
        {
            this.lang = _lang;
            value = _value;
        }
    }
}
