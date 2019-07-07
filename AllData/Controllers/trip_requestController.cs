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
    public class trip_requestController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: trip_request
        public ActionResult Index()
        {
            var trip_request = db.trip_request.Include(t => t.trip_types);
            return View(trip_request.ToList());
        }

        // GET: trip_request/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_request trip_request = db.trip_request.Find(id);
            if (trip_request == null)
            {
                return HttpNotFound();
            }
            return View(trip_request);
        }

        // GET: trip_request/Create
        public ActionResult Create()
        {
            ViewBag.trip_type_id = new SelectList(db.trip_types, "id", "name");
            return View();
        }

        // POST: trip_request/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,rider_id,seats,start_at_date,end_at_date,trip_type_id,created_at,created_by")] trip_request trip_request)
        {
            if (ModelState.IsValid)
            {
                db.trip_request.Add(trip_request);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.trip_type_id = new SelectList(db.trip_types, "id", "name", trip_request.trip_type_id);
            return View(trip_request);
        }

        // GET: trip_request/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_request trip_request = db.trip_request.Find(id);
            if (trip_request == null)
            {
                return HttpNotFound();
            }
            ViewBag.trip_type_id = new SelectList(db.trip_types, "id", "name", trip_request.trip_type_id);
            return View(trip_request);
        }

        // POST: trip_request/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,rider_id,seats,start_at_date,end_at_date,trip_type_id,created_at,created_by")] trip_request trip_request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trip_request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.trip_type_id = new SelectList(db.trip_types, "id", "name", trip_request.trip_type_id);
            return View(trip_request);
        }

        // GET: trip_request/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_request trip_request = db.trip_request.Find(id);
            if (trip_request == null)
            {
                return HttpNotFound();
            }
            return View(trip_request);
        }

        // POST: trip_request/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            trip_request trip_request = db.trip_request.Find(id);
            db.trip_request.Remove(trip_request);
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
