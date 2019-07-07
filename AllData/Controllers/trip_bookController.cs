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
    public class trip_bookController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: trip_book
        public ActionResult Index()
        {
            var trip_book = db.trip_book.Include(t => t.trip_request_details).Include(t => t.trip_share_details);
            return View(trip_book.ToList());
        }

        // GET: trip_book/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_book trip_book = db.trip_book.Find(id);
            if (trip_book == null)
            {
                return HttpNotFound();
            }
            return View(trip_book);
        }

        // GET: trip_book/Create
        public ActionResult Create()
        {
            ViewBag.trip_request_details_id = new SelectList(db.trip_request_details, "id", "from_plc");
            ViewBag.trip_share_details_id = new SelectList(db.trip_share_details, "id", "from_plc");
            return View();
        }

        // POST: trip_book/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,trip_share_details_id,trip_request_details_id,booked_at,start_at,end_at,driver_rate,rider_rate,trip_token,seats")] trip_book trip_book)
        {
            if (ModelState.IsValid)
            {
                db.trip_book.Add(trip_book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.trip_request_details_id = new SelectList(db.trip_request_details, "id", "from_plc", trip_book.trip_request_details_id);
            ViewBag.trip_share_details_id = new SelectList(db.trip_share_details, "id", "from_plc", trip_book.trip_share_details_id);
            return View(trip_book);
        }

        // GET: trip_book/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_book trip_book = db.trip_book.Find(id);
            if (trip_book == null)
            {
                return HttpNotFound();
            }
            ViewBag.trip_request_details_id = new SelectList(db.trip_request_details, "id", "from_plc", trip_book.trip_request_details_id);
            ViewBag.trip_share_details_id = new SelectList(db.trip_share_details, "id", "from_plc", trip_book.trip_share_details_id);
            return View(trip_book);
        }

        // POST: trip_book/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,trip_share_details_id,trip_request_details_id,booked_at,start_at,end_at,driver_rate,rider_rate,trip_token,seats")] trip_book trip_book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trip_book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.trip_request_details_id = new SelectList(db.trip_request_details, "id", "from_plc", trip_book.trip_request_details_id);
            ViewBag.trip_share_details_id = new SelectList(db.trip_share_details, "id", "from_plc", trip_book.trip_share_details_id);
            return View(trip_book);
        }

        // GET: trip_book/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_book trip_book = db.trip_book.Find(id);
            if (trip_book == null)
            {
                return HttpNotFound();
            }
            return View(trip_book);
        }

        // POST: trip_book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            trip_book trip_book = db.trip_book.Find(id);
            db.trip_book.Remove(trip_book);
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
