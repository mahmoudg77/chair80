using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseNotificationSender
{
    class Program
    {
        static  void Main(string[] args)
        {

            if (args.Count() == 0) return;

            int tripid = 0;

            int.TryParse(args[0],out tripid);

            if (tripid == 0) return;

            string scope = "share";
            if (args.Count() > 1) scope = args[1];
            

            var db = new DAL.chari80_dbEntities();


            string fbt = "";

            if (scope == "share")
            {
                var trip = db.trip_share_details.Find(tripid);
                fbt = trip.trip_share.tbl_accounts.sec_users.sec_sessions.Where(a => a.end_time == null).OrderByDescending(a => a.start_time).FirstOrDefault().device_id;
            }
            else
            {
                var trip = db.trip_request_details.Find(tripid);
                fbt = trip.trip_request.tbl_accounts.sec_users.sec_sessions.Where(a => a.end_time == null).OrderByDescending(a => a.start_time).FirstOrDefault().device_id;
            }

            Libs.FirebaseNotifications.Send(new string[] { fbt }, "Schedule Trip", "You have a schedule will started within a few minuts.", new { type = 7, screen = "schedule", id = tripid });

        }
    }
}
