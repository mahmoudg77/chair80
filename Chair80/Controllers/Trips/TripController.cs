using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Chair80.Libs;
using Chair80.DAL;
using Chair80.Filters;
using System.Web;
using Chair80.Requests;

namespace Chair80.Controllers.Trips
{
    [LoginFilter]
    [AppFilter]
    [RoutePrefix("{lang}/Trip")]
    public class TripController : ApiController
    {
        [HttpGet]
        [Route("Details")]
        public APIResult<DAL.vwTripsDetails> Details(int trip_id)
        {
            using (var ctx = new DAL.MainEntities())
            {
                return APIResult<DAL.vwTripsDetails>.Success(ctx.vwTripsDetails.Where(a => a.trip_id == trip_id && a.is_active == true).FirstOrDefault());
            }
        }

        [HttpGet]
        [Route("Current")]
        public APIResult<DAL.vwTripsDetails> Current()
        {
            var u = APIRequest.User(HttpContext.Current.Request);
            using (var ctx = new DAL.MainEntities())
            {
                var sTime = DateTime.Now.AddMinutes(60);
                var cDate = DateTime.Now.Date;
                var cTime = DateTime.Now.AddMinutes(-30);
                return APIResult<DAL.vwTripsDetails>.Success(ctx.vwTripsDetails.Where(a => a.start_at_date < sTime && a.start_at_date > cTime && a.is_active == true && a.driver_id==u.Entity.id).FirstOrDefault());
            }
        }

        [HttpGet]
        [Route("History")]
        public APIResult<List<DAL.vwSeatsHistory>> History(int trip_type_id=0)
        {
            var u = Requests.APIRequest.User(HttpContext.Current.Request);

            using (var ctx = new DAL.MainEntities())
            {
                
                return APIResult<List<DAL.vwSeatsHistory>>.Success(ctx.vwSeatsHistory.Where(a => a.ended_at!=null && ( a.driver_id == u.Entity.id || a.rider_id==u.Entity.id) && (trip_type_id==0?true:a.trip_type_id== trip_type_id)).OrderByDescending(a=>a.ended_at).ToList());

            }



        }

        [HttpGet]
        [Route("Schedule")]
        public APIResult<List<DAL.vwSeatsHistory>> Schedule(int trip_type_id=0)
        {
            var u = Requests.APIRequest.User(HttpContext.Current.Request);

            using (var ctx = new DAL.MainEntities())
            {
                return APIResult<List<DAL.vwSeatsHistory>>.Success(ctx.vwSeatsHistory.Where(a => a.start_at_date > DateTime.Now && (a.driver_id == u.Entity.id || a.rider_id == u.Entity.id) && (trip_type_id == 0 ? true : a.trip_type_id == trip_type_id)).ToList());

            }



        }

        [HttpGet]
        [Route("Pending")]
        public APIResult<List<DAL.vwSeatsPending>> Pending(int trip_type_id=0)
        {
            var u = Requests.APIRequest.User(HttpContext.Current.Request);

            using (var ctx = new DAL.MainEntities())
            {

                return APIResult<List<DAL.vwSeatsPending>>.Success(ctx.vwSeatsPending.Where(a => a.start_at_date>DateTime.Now &&  a.acc_id == u.Entity.id && (trip_type_id == 0 ? true : a.trip_type_id == trip_type_id)).OrderBy(a=>a.start_at_date).ToList());

            }



        }

        [HttpPost]
        [Route("Rate")]
        public APIResult<bool> Rate(int id,RateRequest request)
        {
            var u = APIRequest.User(HttpContext.Current.Request);
            using (var ctx=new MainEntities())
            {
                var book=ctx.trip_book.Include("trip_request_details").Include("trip_request_details.trip_request").FirstOrDefault(a=>a.id==id);
                if (book == null)
                {
                    return APIResult<bool>.Error(ResponseCode.UserValidationField, "This trip not found !");
                }
                if (book.trip_request_details.trip_request.rider_id == u.Entity.id)
                {
                    return APIResult<bool>.Error(ResponseCode.UserValidationField, "You cannot rate this trip!");
                }

                book.driver_rate = request.rate;
                book.rate_comment = request.comment;
                book.rate_reason_id = request.reason_id;


                ctx.Entry(book).State = System.Data.Entity.EntityState.Modified;

                ctx.SaveChanges();

                return APIResult<bool>.Success(true);


            }
        }

        [HttpGet]
        [Route("RecentPlaces")]
        public APIResult<List<MapPlace>> RecentPlaces()
        {
            var u = APIRequest.User(HttpContext.Current.Request);
            List<MapPlace> lst = new List<MapPlace>();
            using (var ctx=new MainEntities())
            {
                var linq = ctx.trip_book.Include("trip_request_details").Include("trip_request").Where(a => a.trip_request_details.trip_request.rider_id == u.Entity.id).Select(c => new {
                    place = c.trip_request_details.to_plc,
                    lat = (decimal)c.trip_request_details.to_lat,
                    lng = (decimal)c.trip_request_details.to_lng,
                }).Distinct().ToList();

                linq.ForEach(a => lst.Add(new MapPlace() {city= getCity(a.place),country= getCountry(a.place), governorate= getGov(a.place), lat=a.lat,lng=a.lng,street= getStreet(a.place) }));
                return APIResult<List<MapPlace>>.Success(lst);
            }
            
        }


        [HttpGet]
        [Route("GetShareUrl")]
        public APIResult<string> GetShareUrl(int? trip_id=0, int? booked_id=0)
        {
            using(var ctx=new MainEntities())
            {

                Guid token=Guid.Empty ;
                if (trip_id != 0)
                {
                    var trip = ctx.trip_share_details.FirstOrDefault(a => a.id == trip_id);
                    if (trip != null) token = (Guid) trip.guid;
                }else if (booked_id != 0)
                {
                    var trip = ctx.trip_book.Find(booked_id);
                    if (trip != null) if (trip.trip_request_details != null) token = (Guid)trip.trip_share_details.guid;
                }
                else
                {
                    var trip = Current();
                    if (trip.data != null)  token = (Guid)trip.data.guid;
                  
                }

                if (token == null) return APIResult<string>.Error(ResponseCode.UserValidationField, "There is no trip to share it!");

                return APIResult<string>.Success(Settings.Get("site_url") + "/Trip/Tracking/" + token);
            }
        }
       
        [HttpGet]
        [Route("GetShareUrl")]
        public APIResult<string> GetShareUrl()
        {
            var trip = Current();
            if (trip.data == null) return APIResult<string>.Error(ResponseCode.UserValidationField, "There is no trip to share it!");

            return APIResult<string>.Success(Settings.Get("site_url") + "/Trip/Tracking/" + trip.data.guid.ToString());
        }
        private string getCountry(string place)
        {
            string[] elements = place.Split(',');
            return (elements.Length > 0) ?  elements[0].Trim() : "";
        }
        private string getGov(string place)
        {
            string[] elements = place.Split(',');
            return (elements.Length > 1) ? elements[1].Trim() : "";
        }
        private string getCity(string place)
        {
            string[] elements = place.Split(',');
            return (elements.Length > 2) ? elements[2].Trim() : "";
        }
        private string getStreet(string place)
        {
            string[] elements = place.Split(',');
            return (elements.Length > 3) ? elements[3].Trim() : "";
        }


    }
}
