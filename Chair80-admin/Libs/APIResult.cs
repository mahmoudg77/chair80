using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Chair80Admin.Libs
{
    public class APIResult<T> 
    {
        public ResultType type { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public APIResult()
        {

        }
        public APIResult(ResultType type)
        {
            this.type = type;
          
        }
      
        public APIResult(ResultType type, T data)
        {
            this.type = type;
            this.data = data;
        }
        public APIResult(ResultType type, string msg)
        {
            this.type = type;
            this.message = msg;
        }
        public APIResult(ResultType type, string msg, T data)
        {
            this.type = type;
            this.message = msg;
            this.data = data;
        }
        public APIResult(ResultType type, T data, string msg)
        {
            this.type = type;
            this.message = msg;
            this.data = data;
        }


    }

    public enum ResultType
    {
        fail = 0,
        success = 1,
        warning = -1,
    }

}