using Chair80.Filters;
using Chair80.Libs;
using Chair80.Requests;
using Chair80.Responses;
using Newtonsoft.Json;
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
    [LoginFilter]
    [AppFilter]
    [RoutePrefix("{lang}/Seat")]
    public class SeatController : ApiController
    {
        [HttpPost]
        [AuthFilter("Allow Share Seats")]
        [Route("Share")]
        public APIResult<DAL.vwTripsDetails> Share(ShareSeatRequest request)
        {

            var u = APIRequest.User(HttpContext.Current.Request);
            if (request.start_at == null) request.start_at = DateTime.Now;

            using (var ctx=new DAL.MainEntities())
            {


                var t = new Controllers.Trips.TripController();

                checkCurrent:
                var currTrip = t.Current().data;

                if (currTrip != null)
                {
                    if (currTrip.booked_seats == 0)
                    {
                        var trp = ctx.trip_share_details.Find(currTrip.trip_id);
                        trp.is_active = false;
                        ctx.Entry(trp).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        goto checkCurrent;
                    }
                    else
                    {
                        return APIResult<DAL.vwTripsDetails>.Error(ResponseCode.UserNotAcceptable, "You have already trip, You cannot share new trip");
                    }
                }
                var Trip = new DAL.trip_share()
                {
                    created_at = DateTime.Now,
                    created_by = u.Entity.id,
                    driver_id = u.Entity.id,
                    start_at_date = request.start_at,
                   
                    trip_type_id = (int)request.trip_type,
                    vehicle_id = request.vehicle_id,
                };
                ctx.trip_share.Add(Trip);

                //if (request.shuttle_at != null)
                //    Trip.end_at_date = request.shuttle_at.Value.Date;

                var Leave_Details = new DAL.trip_share_details()
                {
                    from_lat = request.from.lat,
                    from_lng = request.from.lng,
                    from_plc = request.from.country + ", " + request.from.governorate + ", " + request.from.city + ", " + request.from.street,
                    to_lat = request.to.lat,
                    to_lng = request.to.lng,
                    to_plc = request.to.country + ", " + request.to.governorate + ", " + request.to.city + ", " + request.to.street,
                    gender_id = request.gender_id,
                    is_active = true,
                    seats = request.seats,
                    seat_cost = request.seat_cost,
                    start_at_date = request.start_at.Value,
                    start_at_time = request.start_at.Value.TimeOfDay,
                    trip_direction = 1,
                    guid = Guid.NewGuid()
                };
                Trip.trip_share_details.Add(Leave_Details);
                ctx.SaveChanges();
                BookIfAvailable(Leave_Details, Trip);
                ctx.Entry(Leave_Details).State = System.Data.Entity.EntityState.Modified;

                if (request.trip_type == TripTypes.Round || request.trip_type == TripTypes.Shuttle)
                {
                    var Round_Details = new DAL.trip_share_details()
                    {
                        to_lat = request.from.lat,
                        to_lng = request.from.lng,
                        to_plc = request.from.country + ", " + request.from.governorate + ", " + request.from.city + ", " + request.from.street,
                        from_lat = request.to.lat,
                        from_lng= request.to.lng,
                        from_plc= request.to.country + ", " + request.to.governorate + ", " + request.to.city + ", " + request.to.street,
                        gender_id = request.gender_id,
                        is_active = true,
                        seats = request.seats,
                        seat_cost = request.seat_cost,
                        start_at_date = request.round_at == null ? DateTime.Now.Date : request.round_at.Value,
                        start_at_time = request.round_at == null ? DateTime.Now.TimeOfDay : request.round_at.Value.TimeOfDay,
                        trip_direction = 2,
                        guid = Guid.NewGuid()
                    };
                    Trip.trip_share_details.Add(Round_Details);
                    ctx.SaveChanges();
                    BookIfAvailable(Round_Details, Trip);
                    ctx.Entry(Round_Details).State = System.Data.Entity.EntityState.Modified;
                }

                if (request.trip_type == TripTypes.Shuttle)
                {
                    var cDate = request.start_at.Value.AddDays(1);
                    var rDate = request.round_at.Value.AddDays(1);
                    while (cDate<=request.shuttle_at.Value)
                    {
                        var Leave = new DAL.trip_share_details()
                        {
                            from_lat = request.from.lat,
                            from_lng = request.from.lng,
                            from_plc = request.from.country + ", " + request.from.governorate + ", " + request.from.city + ", " + request.from.street,
                            to_lat = request.to.lat,
                            to_lng = request.to.lng,
                            to_plc = request.to.country + ", " + request.to.governorate + ", " + request.to.city + ", " + request.to.street,
                            gender_id = request.gender_id,
                            is_active = true,
                            seats = request.seats,
                            seat_cost = request.seat_cost,
                            start_at_date = cDate,
                            start_at_time = request.start_at.Value.TimeOfDay,
                            trip_direction = 1,
                            guid = Guid.NewGuid()
                        };
                        Trip.trip_share_details.Add(Leave);
                        ctx.SaveChanges();
                        BookIfAvailable(Leave, Trip);
                        ctx.Entry(Leave).State = System.Data.Entity.EntityState.Modified;
                        var Round = new DAL.trip_share_details()
                        {
                            to_lat = request.from.lat,
                            to_lng = request.from.lng,
                            to_plc = request.from.country + ", " + request.from.governorate + ", " + request.from.city + ", " + request.from.street,
                            from_lat = request.to.lat,
                            from_lng = request.to.lng,
                            from_plc = request.to.country + ", " + request.to.governorate + ", " + request.to.city + ", " + request.to.street,
                            gender_id = request.gender_id,
                            is_active = true,
                            seats = request.seats,
                            seat_cost = request.seat_cost,
                            start_at_date = rDate,
                            start_at_time = request.round_at.Value.TimeOfDay,
                            trip_direction = 2,
                            guid = Guid.NewGuid()
                        };
                        Trip.trip_share_details.Add(Round);
                        ctx.SaveChanges();
                        BookIfAvailable(Round, Trip);
                        ctx.Entry(Round).State = System.Data.Entity.EntityState.Modified;
                        cDate = cDate.AddDays(1);
                        rDate = rDate.AddDays(1);
                    }

                   
                }
               

                try
                {

                    ctx.SaveChanges();

                    var data = ctx.vwTripsDetails.Where(a => a.trip_share_id == Trip.id).FirstOrDefault();


                    return APIResult<DAL.vwTripsDetails>.Success(data);
                }
                catch (Exception ex)
                {

                    return APIResult<DAL.vwTripsDetails>.Error(ResponseCode.BackendDatabase, ex.Message);
                }
            }
        }

        private bool BookIfAvailable( DAL.trip_share_details details, DAL.trip_share trip)
        {
            using (var ctx = new DAL.MainEntities())
            {

                int searchReduis = int.Parse(Settings.Get("search_redius","500"));

                var current_trips = ctx.trip_request_details.Include("trip_request").AsQueryable();


                //current_trips = current_trips.Where(a => a.from_plc.Contains(details.from_plc));

                //current_trips = current_trips.Where(a => (a.from_plc + ",").Contains(request.from.city + ","));


                //current_trips = current_trips.Where(a => a.from_plc.Contains(request.from.street + ","));

                current_trips = current_trips.Where(a => a.from_lat.Value + searchReduis < details.from_lat && a.from_lat.Value - searchReduis > details.from_lat);

                current_trips = current_trips.Where(a => a.from_lng.Value + searchReduis < details.from_lng && a.from_lng.Value - searchReduis > details.from_lng);



                //current_trips = current_trips.Where(a => a.to_plc.Contains(details.to_plc));



                //current_trips = current_trips.Where(a => a.to_plc.Contains(request.to.street + ","));

                current_trips = current_trips.Where(a => a.to_lat.Value + searchReduis < details.to_lat && a.to_lat.Value - searchReduis > details.to_lat);

                current_trips = current_trips.Where(a => a.to_lng.Value + searchReduis < details.to_lng && a.to_lng.Value - searchReduis > details.to_lng);



                if (details.gender_id > 0)
                {
                    current_trips = current_trips.Where(a => a.gender_id == details.gender_id && a.gender_id == 0);
                }


                current_trips = current_trips.Where(a => a.trip_request.trip_type_id == trip.trip_type_id);
               
                    var sTime = details.start_at_date.Value.AddMinutes(120);
                    var rDate = details.start_at_date.Value.Date;
                    var rTime = details.start_at_date.Value.AddMinutes(-30);
                
                
                current_trips = current_trips.Where(a => a.start_at_date < sTime && a.start_at_date > rTime);

                current_trips = current_trips.Where(a => ((a.seat_cost_from > 0 && a.seat_cost_from <= details.seat_cost) || a.seat_cost_from==0) && ((a.seat_cost_to>0 && a.seat_cost_to >= details.seat_cost) || a.seat_cost_to ==0));
                current_trips = current_trips.Where(a => a.seats <= details.seats);
                current_trips = current_trips.Where(a => a.is_active == true);
                current_trips = current_trips.Where(a => a.booked ==false || a.booked==null);

                if (details.booked_seats == null) details.booked_seats = 0;
                foreach (var trp in current_trips)
                {
                    if (details.booked_seats >= details.seats) break;
                    if (details.booked_seats  + trp.seats>= details.seats) continue;

                    for (var x = 0; x < trp.seats; x++)
                    {
                        var itm = new DAL.trip_book()
                        {
                            trip_request_details_id = trp.id,
                            booked_at = DateTime.Now,
                            seats = 1,
                            trip_share_details_id = details.id,
                            trip_token = Guid.NewGuid(),
                        };

                        ctx.trip_book.Add(itm);
                    }
                    details.booked_seats++;
                    trp.booked = true;
                    ctx.Entry(trp).State = System.Data.Entity.EntityState.Modified;
                    //ctx.Entry(details).State = System.Data.Entity.EntityState.Modified;

                }
                //ctx.Entry(trip)
                ctx.SaveChanges();

                return true;
            }
        }

        [HttpPost]
        [Route("Need")]
        public APIResult<bool> Need(NeedSeatRequest request)
        {

            var u = APIRequest.User(HttpContext.Current.Request);

            if (request.start_at == null) request.start_at = DateTime.Now;
            
            using (var ctx = new DAL.MainEntities())
            {
                var Trip = new DAL.trip_request()
                {
                    created_at = DateTime.Now,
                    created_by = u.Entity.id,
                    start_at_date = request.start_at,
                    trip_type_id = (int)request.trip_type,
                    rider_id=u.Entity.id
                };
                if (request.shuttle_at != null) Trip.end_at_date = request.shuttle_at;

                   var Leave_Details = new DAL.trip_request_details()
                {
                    from_lat = request.from.lat,
                    from_lng = request.from.lng,
                    from_plc = request.from.country + ", " + request.from.governorate + ", " + request.from.city + ", " + request.from.street,
                    to_lat = request.to.lat,
                    to_lng = request.to.lng,
                    to_plc = request.to.country + ", " + request.to.governorate + ", " + request.to.city + ", " + request.to.street,
                    gender_id = request.gender_id,
                    is_active = true,
                    seats = request.seats,
                    seat_cost_from = request.seat_cost_from,
                    seat_cost_to = request.seat_cost_to,
                    start_at_date = request.start_at.Value,
                    start_at_time = request.start_at.Value.TimeOfDay,
                    trip_direction = 1,
                };
                Trip.trip_request_details.Add(Leave_Details);

                if (request.trip_type == TripTypes.Round || request.trip_type == TripTypes.Shuttle)
                {
                    var Round_Details = new DAL.trip_request_details()
                    {
                        to_lat = request.from.lat,
                        to_lng = request.from.lng,
                        to_plc = request.from.country + ", " + request.from.governorate + ", " + request.from.city + ", " + request.from.street,
                        from_lat = request.to.lat,
                        from_lng = request.to.lng,
                        from_plc = request.to.country + ", " + request.to.governorate + ", " + request.to.city + ", " + request.to.street,
                        gender_id = request.gender_id,
                        is_active = true,
                        seats = request.seats,
                        seat_cost_from = request.seat_cost_from,
                        seat_cost_to = request.seat_cost_to,
                        start_at_date = request.round_at.Value,
                        start_at_time = request.round_at.Value.TimeOfDay,
                        trip_direction = 2,
                    };
                    Trip.trip_request_details.Add(Round_Details);
                }

                if (request.trip_type == TripTypes.Shuttle)
                {
                    var cDate = request.start_at.Value.AddDays(1);
                    var rDate = request.round_at.Value.AddDays(1);

                    while (cDate <= request.shuttle_at.Value)
                    {
                        var Leave = new DAL.trip_request_details()
                        {
                            from_lat = request.from.lat,
                            from_lng = request.from.lng,
                            from_plc = request.from.country + ", " + request.from.governorate + ", " + request.from.city + ", " + request.from.street,
                            to_lat = request.to.lat,
                            to_lng = request.to.lng,
                            to_plc = request.to.country + ", " + request.to.governorate + ", " + request.to.city + ", " + request.to.street,
                            gender_id = request.gender_id,
                            is_active = true,
                            seats = request.seats,
                            seat_cost_from = request.seat_cost_from,
                            seat_cost_to = request.seat_cost_to,
                            start_at_date = cDate,
                            start_at_time = request.start_at.Value.TimeOfDay,
                            trip_direction = 1,
                        };
                        Trip.trip_request_details.Add(Leave);

                        var Round = new DAL.trip_request_details()
                        {
                            to_lat = request.from.lat,
                            to_lng = request.from.lng,
                            to_plc = request.from.country + ", " + request.from.governorate + ", " + request.from.city + ", " + request.from.street,
                            from_lat = request.to.lat,
                            from_lng = request.to.lng,
                            from_plc = request.to.country + ", " + request.to.governorate + ", " + request.to.city + ", " + request.to.street,
                            gender_id = request.gender_id,
                            is_active = true,
                            seats = request.seats,
                            seat_cost_from = request.seat_cost_from,
                            seat_cost_to = request.seat_cost_to,
                            start_at_date = rDate,
                            start_at_time = request.round_at.Value.TimeOfDay,
                            trip_direction = 2,
                        };
                        Trip.trip_request_details.Add(Round);

                        cDate = cDate.AddDays(1);
                        rDate = rDate.AddDays(1);
                    }
                }
                ctx.trip_request.Add(Trip);
                try
                {
                    ctx.SaveChanges();
                    return APIResult<bool>.Success(true);
                }
                catch (Exception ex)
                {

                    return APIResult<bool>.Error(ResponseCode.BackendDatabase, ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("Search")]
        public APIResult<IEnumerable<DAL.vwTripsDetails>> Search(SearchTripRequest request)
        {
            int searchReduis = int.Parse(Settings.Get("search_redius", "20000"));

            using (var ctx=new DAL.MainEntities())
            {

                var current_trips = ctx.vwTripsDetails.Where(a=>a.is_active==true).AsQueryable();
               
                if (request.from!=null)
                {
                    //if (!string.IsNullOrEmpty(request.from.country))
                    //{
                    //    current_trips = current_trips.Where(a => (a.from_plc + ",").Contains(request.from.country + ","));
                    //}
                    //if (!string.IsNullOrEmpty(request.from.city))
                    //{
                    //    current_trips = current_trips.Where(a => (a.from_plc + ",").Contains(request.from.city + ","));
                    //}
                    //if (string.IsNullOrEmpty(request.from.street))
                    //{
                    //    current_trips = current_trips.Where(a => (a.from_plc + ",").Contains(request.from.street + ","));
                    //}
                    if (request.from.lat != 0)
                    {
                        decimal lat_from = request.from.lat - ((decimal)searchReduis / (decimal)1000000);
                        decimal lat_to = request.from.lat + ((decimal)searchReduis / (decimal)1000000);

                        current_trips = current_trips.Where(a => a.from_lat  <= lat_to && a.from_lat  >= lat_from);
                    }
                    if (request.from.lng != 0)
                    {
                        decimal lng_from = request.from.lng - ((decimal)searchReduis / (decimal)1000000);
                        decimal lng_to = request.from.lng + ((decimal)searchReduis / (decimal)1000000);

                        current_trips = current_trips.Where(a => a.from_lng <= lng_to && a.from_lng >= lng_from);
                    }
                }
                else
                {
                    if (request.firebase_ids != null && request.firebase_ids.Count() > 0)
                    {
                        current_trips = current_trips.Where(a => request.firebase_ids.Contains(a.acc_firebase_uid));
                    }
                }
                if (request.to != null)
                {
                    //if (!string.IsNullOrEmpty(request.to.country))
                    //{
                    //    current_trips = current_trips.Where(a => (a.to_plc + ",").Contains(request.to.country + ","));
                    //}
                    //if (!string.IsNullOrEmpty(request.to.city))
                    //{
                    //    current_trips = current_trips.Where(a => (a.to_plc + ",").Contains(request.to.city + ","));
                    //}
                    //if (string.IsNullOrEmpty(request.to.street))
                    //{
                    //    current_trips = current_trips.Where(a => (a.to_plc + ",").Contains(request.to.street + ","));
                    //}
                    if (request.to.lat != 0)
                    {
                        decimal lat_from = request.to.lat - ((decimal)searchReduis / (decimal)1000000);
                        decimal lat_to = request.to.lat + ((decimal)searchReduis / (decimal)1000000);

                        current_trips = current_trips.Where(a => a.to_lat <= lat_to && a.to_lat  >= lat_from);
                    }
                    if (request.to.lng != 0)
                    {
                        decimal lng_from = request.to.lng - ((decimal)searchReduis / (decimal)1000000);
                        decimal lng_to = request.to.lng + ((decimal)searchReduis / (decimal)1000000);
                        current_trips = current_trips.Where(a => a.to_lng <= lng_to && a.to_lng  >= lng_from);
                    }
                }

                if (request.gender_id > 0)
                {
                    current_trips = current_trips.Where(a => a.trip_gender_id == request.gender_id || a.trip_gender_id==0);
                }

                if (request.trip_type > 0)
                {
                    current_trips = current_trips.Where(a => a.trip_type_id == (int)request.trip_type);
                }

                if (request.start_at == null)
                {
                    var sTime = DateTime.Now.AddMinutes(120);
                    var cDate = DateTime.Now.Date;
                    var cTime = DateTime.Now.AddMinutes(-30);
                    current_trips = current_trips.Where(a => a.start_at_date < sTime && a.start_at_date > cTime);
                }
                else { 
                    var sTime = request.start_at.Value.AddMinutes(120);
                    var rDate = request.start_at.Value.Date;
                    var rTime = request.start_at.Value.AddMinutes(-30);
                    current_trips = current_trips.Where(a => a.start_at_date < sTime && a.start_at_date > rTime);
                }

                if (request.seat_cost_from != null)
                {
                    current_trips = current_trips.Where(a => a.seat_cost>=request.seat_cost_from && a.seat_cost<= request.seat_cost_to);
                }
                if (request.seat_cost_to != null)
                {
                    current_trips = current_trips.Where(a => a.seat_cost <= request.seat_cost_to);
                }
                


                return APIResult<IEnumerable<DAL.vwTripsDetails>>.Success(current_trips.ToList());
            }
        }


        [HttpGet]
        [Route("Details")]
        public APIResult<IEnumerable<DAL.vwTripSeatDetails>> Details(int trip_id)
        {
            using (var ctx=new DAL.MainEntities())
            {
                return APIResult<IEnumerable<DAL.vwTripSeatDetails>>.Success(ctx.vwTripSeatDetails.Where(a => a.trip_id == trip_id).ToList());
            }
        }

        [HttpPost]
        [Route("Booking")]
        public async Task<APIResult<List<DAL.trip_book>>> Booking(int trip_id,int? seat=1,bool? all_shoots=true)
        {
            var u = Requests.APIRequest.User(HttpContext.Current.Request);

            if (trip_id == 0) return APIResult<List<DAL.trip_book>>.Error(ResponseCode.UserValidationField, "'trip_id' is required!");

            List<DAL.trip_book> lst = new List<DAL.trip_book>();

            using (var ctx=new DAL.MainEntities())
            {
                var selectedTrip = ctx.trip_share_details.Include("trip_share").FirstOrDefault(a => a.id == trip_id);
                var cTime = DateTime.Now.AddMinutes(-30);
                var triplist = ctx.trip_share_details.Include("trip_share").Where(a => a.trip_share_id == selectedTrip.trip_share_id && a.start_at_date >= cTime).ToList();
                if (triplist.Count == 0)
                {
                    return APIResult<List<DAL.trip_book>>.Error(ResponseCode.UserValidationField, "This trip not available for booking now!");
                }
                foreach (var trip in triplist)
                    {
                        
                        //var trip=ctx.trip_share_details.Include("trip_share").Where(a=>a.id==trip_id).FirstOrDefault();

                        if (trip == null) return APIResult<List<DAL.trip_book>>.Error(ResponseCode.UserNotFound, "This trip not found!");

                        if (trip.booked_seats == null) trip.booked_seats = 0;

                        if (trip.seats == trip.booked_seats) return APIResult<List<DAL.trip_book>>.Error(ResponseCode.UserNotAcceptable, "There are no available seat in this trip!");

                        if (trip.seats - trip.booked_seats < seat) return APIResult<List<DAL.trip_book>>.Error(ResponseCode.UserNotAcceptable, "There are only " + (trip.seats - trip.booked_seats) + " available seat/s in this trip");


                        //var requests=ctx.trip_request_details.Include("trip_request").Where(a=>)


                        var req = new DAL.trip_request()
                        {
                            created_at = DateTime.Now,
                            created_by = u.Entity.id,
                            end_at_date = null,
                            rider_id = u.Entity.id,
                            seats = seat,
                            start_at_date = trip.start_at_date,
                            trip_type_id = trip.trip_share.trip_type_id,
                        };

                        var req_details = new DAL.trip_request_details()
                        {
                            from_lat = trip.from_lat,
                            from_lng = trip.from_lng,
                            from_plc = trip.from_plc,
                            gender_id = trip.gender_id,
                            is_active = true,
                            seats = seat,
                            seat_cost_from = trip.seat_cost,
                            seat_cost_to = trip.seat_cost,
                            start_at_date = trip.start_at_date,
                            start_at_time = trip.start_at_time,
                            to_lat = trip.to_lat,
                            to_lng = trip.to_lng,
                            to_plc = trip.to_plc,
                            booked=true
                        };

                        req.trip_request_details.Add(req_details);

                        ctx.trip_request.Add(req);

                        if (ctx.SaveChanges() > 0)
                        {
                            for (int i = 0; i < seat; i++)
                            {
                                var itm = new DAL.trip_book()
                                {
                                    trip_request_details_id = req_details.id,
                                    booked_at = DateTime.Now,
                                    seats = 1,
                                    trip_share_details_id = trip_id,
                                    trip_token = Guid.NewGuid(),
                                };

                                trip.booked_seats++;
                                ctx.Entry(trip).State = System.Data.Entity.EntityState.Modified;


                                ctx.trip_book.Add(itm);
                                lst.Add(itm);
                            }

                            ctx.SaveChanges();
                        }
                        else
                        {
                            return APIResult<List<DAL.trip_book>>.Error(ResponseCode.BackendDatabase, "Error while save trip_request");
                        }


                        
                    if (all_shoots != true) return  APIResult<List<DAL.trip_book>>.Success(lst);


                }
                var driverDeviceID = ctx.sec_sessions.Where(a => a.user_id == selectedTrip.trip_share.driver_id && a.device_id != null).Select(a => a.device_id).Distinct().ToArray();
                //var ddd = ctx.sec_sessions.Where(a => a.user_id == selectedTrip.trip_share.driver_id).ToArray();
               // Logger.log(JsonConvert.SerializeObject( ddd));
                await FirebaseNotifications.Send(driverDeviceID, "Seats Booking", "Someone has booked your shared seats.", new { type=1,screen = "trip_share", id = selectedTrip.id,sender=ctx.vwProfile.FirstOrDefault(a=>a.id == u.Entity.id).firebase_uid });
                 //FirebaseNotifications.Send(new string[] { "rP2tRzF3uKNfYrZu9YypmohBKJX2" }, "Seats Booking", "Someone has booked your shared seats.", new { type = 1, screen = "trip_share", id = 2272, sender = "PF3nbGndlZV99YyPI1J4NOs6XVE2" }); 
                return APIResult<List<DAL.trip_book>>.Success(lst);
            }
        }

        [HttpPost]
        [Route("Reached")]
        public async Task<APIResult<bool>> Reached(string ids)
        {

            int[] book_ids = ids.Split(',').Select(a=>int.Parse(a)).ToArray();
            var u = APIRequest.User(HttpContext.Current.Request);
            if(book_ids.Count()==0) return APIResult<bool>.Error(ResponseCode.UserValidationField, "ids is required !");
            using (var ctx=new DAL.MainEntities())
            {
                var lst = ctx.trip_book.Include("trip_share_details").Include("trip_request_details").Where(a => book_ids.Contains(a.id));
                foreach (var item in lst)
                {
                    item.reached_at = DateTime.Now;
                    ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }


                bool r=ctx.SaveChanges()>0;

                var tripid = lst.FirstOrDefault().trip_share_details.id;

                var riderIDs = ctx.trip_request.Where(a=>lst.Select(s=>s.trip_request_details.trip_request_id).Contains(a.id)).Select(a=>a.rider_id).ToList();


                var driverDeviceID = ctx.sec_sessions.Where(a => riderIDs.Contains( a.user_id)).Select(a => a.device_id).Distinct().ToArray();
               var result= await FirebaseNotifications.Send(driverDeviceID, "Driver Reached You", "The driver just reached you.", new { type = 4, screen = "trip", id = tripid,sender= ctx.vwProfile.FirstOrDefault(a => a.id == u.Entity.id).firebase_uid });

                
                return APIResult<bool>.Success(r, result.ToString());
            }
        }

        [HttpPost]
        [Route("Started")]
        public async Task<APIResult<bool>> Started(string ids)
        {

            int[] book_ids = ids.Split(',').Select(a => int.Parse(a)).ToArray();
            var u = APIRequest.User(HttpContext.Current.Request);
            if (book_ids.Count() == 0) return APIResult<bool>.Error(ResponseCode.UserValidationField, "ids is required !");
            using (var ctx = new DAL.MainEntities())
            {
                var lst = ctx.trip_book.Include("trip_share_details").Include("trip_request_details").Where(a => book_ids.Contains(a.id));
                foreach (var item in lst)
                {
                    item.start_at = DateTime.Now;
                    ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }


                bool r = ctx.SaveChanges() > 0;
                var tripid = lst.FirstOrDefault().trip_share_details.id;

                var riderIDs = ctx.trip_request.Where(a => lst.Select(s => s.trip_request_details.trip_request_id).Contains(a.id)).Select(a => a.rider_id).ToList();


                var driverDeviceID = ctx.sec_sessions.Where(a => riderIDs.Contains(a.user_id)).Select(a => a.device_id).Distinct().ToArray();

                await FirebaseNotifications.Send(driverDeviceID, "Trip Started", "The driver just start your trip.", new { type = 5, screen = "start", id = tripid, sender = ctx.vwProfile.FirstOrDefault(a => a.id == u.Entity.id).firebase_uid });

                return APIResult<bool>.Success(r);
            }
        }

        [HttpPost]
        [Route("Ended")]
        public async Task<APIResult<bool>> Ended(string ids)
        {

            int[] book_ids = ids.Split(',').Select(a => int.Parse(a)).ToArray();
            if (book_ids.Count() == 0) return APIResult<bool>.Error(ResponseCode.UserValidationField, "ids is required !");
            var u = APIRequest.User(HttpContext.Current.Request);
            using (var ctx = new DAL.MainEntities())
            {
                var lst = ctx.trip_book.Include("trip_share_details").Include("trip_request_details").Where(a => book_ids.Contains(a.id));
                foreach (var item in lst)
                {
                    item.end_at = DateTime.Now;
                    ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }


                bool r = ctx.SaveChanges() > 0;

                var tripid = lst.FirstOrDefault().trip_share_details.id;

                var riderIDs = ctx.trip_request.Where(a => lst.Select(s => s.trip_request_details.trip_request_id).Contains(a.id)).Select(a => a.rider_id).ToList();


                var driverDeviceID = ctx.sec_sessions.Where(a => riderIDs.Contains(a.user_id)).Select(a => a.device_id).Distinct().ToArray();

                await FirebaseNotifications.Send(driverDeviceID, "Trip Ended", "The driver just end your trip.", new { type = 6, screen = "trip-rate", id = tripid, sender = ctx.vwProfile.FirstOrDefault(a => a.id == u.Entity.id).firebase_uid });


                return APIResult<bool>.Success(r);
            }
        }

        [HttpPost]
        [Route("Accepted")]
        public async Task<APIResult<bool>> Accepted(string ids)
        {

            int[] book_ids = ids.Split(',').Select(a => int.Parse(a)).ToArray();
            if (book_ids.Count() == 0) return APIResult<bool>.Error(ResponseCode.UserValidationField, "ids is required !");
            var u = APIRequest.User(HttpContext.Current.Request);
            using (var ctx = new DAL.MainEntities())
            {

                var lst = ctx.trip_book.Include("trip_share_details").Include("trip_request_details").Where(a => book_ids.Contains(a.id));
                foreach (var item in lst)
                {
                    item.accepted_at = DateTime.Now;
                    item.accepted_by =u.Entity.id ;
                    ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }


                bool r = ctx.SaveChanges() > 0;

                var tripid = lst.FirstOrDefault().trip_share_details.id;

                var riderIDs = ctx.trip_request.Where(a => lst.Select(s => s.trip_request_details.trip_request_id).Contains(a.id)).Select(a => a.rider_id).ToList();


                var driverDeviceID = ctx.sec_sessions.Where(a => riderIDs.Contains(a.user_id)).Select(a => a.device_id).Distinct().ToArray();

                await FirebaseNotifications.Send(driverDeviceID, "Trip Accepted", "The driver just accept your trip.", new { type = 2, screen = "trip", id = tripid, sender = ctx.vwProfile.FirstOrDefault(a => a.id == u.Entity.id).firebase_uid });


                return APIResult<bool>.Success(r);
            }
        }

        [HttpPost]
        [Route("Canceled")]
        public async Task<APIResult<bool>> Canceled(string ids)
        {

            int[] book_ids = ids.Split(',').Select(a => int.Parse(a)).ToArray();
            if (book_ids.Count() == 0) return APIResult<bool>.Error(ResponseCode.UserValidationField, "ids is required !");
            var u = APIRequest.User(HttpContext.Current.Request);
            using (var ctx = new DAL.MainEntities())
            {
                var lst = ctx.trip_book.Include("trip_share_details").Include("trip_request_details").Where(a => book_ids.Contains(a.id));

                foreach (var item in lst)
                {
                    item.canceled_at = DateTime.Now;
                    item.canceled_by = u.Entity.id;
                    item.trip_share_details.booked_seats++;

                    ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }

                bool r = ctx.SaveChanges() > 0;
                var tripid = lst.FirstOrDefault().trip_share_details.id;

                var riderIDs = ctx.trip_request.Where(a => lst.Select(s => s.trip_request_details.trip_request_id).Contains(a.id)).Select(a => a.rider_id).ToList();


                var driverDeviceID = ctx.sec_sessions.Where(a => riderIDs.Contains(a.user_id)).Select(a => a.device_id).Distinct().ToArray();

                await FirebaseNotifications.Send(driverDeviceID, "Seats Canceled", "The driver just canceled your request seats.", new { type = 3, screen = "trip", id = tripid, sender = ctx.vwProfile.FirstOrDefault(a => a.id == u.Entity.id).firebase_uid });

                return APIResult<bool>.Success(r);
            }
        }

        [HttpGet]
        [Route("Current")]
        [Obsolete("Seat/Current is deprecated, please use Trip/Current instead.")]
        public APIResult<DAL.vwTripsDetails> Current()
        {
            using (var ctx=new DAL.MainEntities())
            {
                var sTime = DateTime.Now.AddMinutes(60);
                var cDate = DateTime.Now.Date;
                var cTime = DateTime.Now.AddMinutes(-30);

                return APIResult<DAL.vwTripsDetails>.Success(ctx.vwTripsDetails.Where(a => a.start_at_date < sTime && a.start_at_date > cTime && a.is_active==true).OrderByDescending(a => a.trip_id).FirstOrDefault());
            }
        }
    }
}
