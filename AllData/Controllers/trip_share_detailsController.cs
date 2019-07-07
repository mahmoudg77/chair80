using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AllData.Models;

namespace AllData.Controllers
{
    public class trip_share_detailsController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: trip_share_details
        public ActionResult Index()
        {
            var trip_share_details = db.trip_share_details.Include(t => t.trip_share);
            return View(trip_share_details.ToList());
        }

        // GET: trip_share_details/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_share_details trip_share_details = db.trip_share_details.Find(id);
            if (trip_share_details == null)
            {
                return HttpNotFound();
            }
            return View(trip_share_details);
        }

        // GET: trip_share_details/Create
        public ActionResult Create()
        {
            ViewBag.trip_share_id = new SelectList(db.trip_share, "id", "id");
            return View();
        }

        // POST: trip_share_details/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,trip_share_id,seats,start_at_date,start_at_time,from_lat,from_lng,from_plc,to_lat,to_lng,to_plc,gender_id,booked_seats,seat_cost,trip_direction,is_active")] trip_share_details trip_share_details)
        {
            if (ModelState.IsValid)
            {
                db.trip_share_details.Add(trip_share_details);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.trip_share_id = new SelectList(db.trip_share, "id", "id", trip_share_details.trip_share_id);
            return View(trip_share_details);
        }

        // GET: trip_share_details/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_share_details trip_share_details = db.trip_share_details.Find(id);
            if (trip_share_details == null)
            {
                return HttpNotFound();
            }
            ViewBag.trip_share_id = new SelectList(db.trip_share, "id", "id", trip_share_details.trip_share_id);
            return View(trip_share_details);
        }

        // POST: trip_share_details/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,trip_share_id,seats,start_at_date,start_at_time,from_lat,from_lng,from_plc,to_lat,to_lng,to_plc,gender_id,booked_seats,seat_cost,trip_direction,is_active")] trip_share_details trip_share_details)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trip_share_details).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.trip_share_id = new SelectList(db.trip_share, "id", "id", trip_share_details.trip_share_id);
            return View(trip_share_details);
        }

        // GET: trip_share_details/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_share_details trip_share_details = db.trip_share_details.Find(id);
            if (trip_share_details == null)
            {
                return HttpNotFound();
            }
            return View(trip_share_details);
        }

        // POST: trip_share_details/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            trip_share_details trip_share_details = db.trip_share_details.Find(id);
            db.trip_share_details.Remove(trip_share_details);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
