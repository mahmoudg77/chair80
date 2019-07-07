using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
namespace Chair80.Libs
{
    public class APIResult<T> 
    {
        public bool isSuccess { get; set; }
        public ResponseCode code { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public APIResult()
        {

        }


        public static APIResult<T> Error(ResponseCode code, string message)
        {
            return new APIResult<T>() { isSuccess = false, code = code, message = Locales.Locales.translate(message) };
        }

        public static APIResult<T> Error(ResponseCode code, string message,T data)
        {
            return new APIResult<T>() { isSuccess = false, code = code, message = Locales.Locales.translate(message), data=data };
        }
        public static APIResult<T> Success(T Data, string message="")
        {
            return new APIResult<T>() { isSuccess = true, code = ResponseCode.Success, message = Locales.Locales.translate( message),data=Data };
        }




    }

    /// <summary>
    /// 
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// 
        /// </summary>
        Success=200,
        /// <summary>
        /// 
        /// </summary>
        UserForbidden=1403,
        /// <summary>
        /// 
        /// </summary>
        UserUnauthorized = 1401,
        /// <summary>
        /// 
        /// </summary>
        UserNotFound = 1404,
        /// <summary>
        /// 
        /// </summary>
        UserRequestTimeout = 1408,
        /// <summary>
        /// 
        /// </summary>
        UserNotAcceptable =1406,
        /// <summary>
        /// 
        /// </summary>
        UserUnVerified=1407,
        /// <summary>
        /// 
        /// </summary>
        UserValidationField=1408,
        /// <summary>
        /// 
        /// </summary>
        UserDoublicate=1409,


        /// <summary>
        /// 
        /// </summary>
        DevBadGeteway=2503,
        /// <summary>
        /// 
        /// </summary>
        DevNotFound = 2404,

        /// <summary>
        /// 
        /// </summary>
        BackendServerRequest = 3400,
        /// <summary>
        /// 
        /// </summary>
        BackendInternalServer =3500,
        /// <summary>
        /// 
        /// </summary>
        BackendDatabase=3600,

    }

  
}