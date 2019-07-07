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
    public class trip_shareController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: trip_share
        public ActionResult Index()
        {
            var trip_share = db.trip_share.Include(t => t.tbl_accounts).Include(t => t.tbl_vehicles).Include(t => t.trip_types);
            return View(trip_share.ToList());
        }

        // GET: trip_share/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_share trip_share = db.trip_share.Find(id);
            if (trip_share == null)
            {
                return HttpNotFound();
            }
            return View(trip_share);
        }

        // GET: trip_share/Create
        public ActionResult Create()
        {
            ViewBag.driver_id = new SelectList(db.tbl_accounts, "id", "first_name");
            ViewBag.vehicle_id = new SelectList(db.tbl_vehicles, "id", "model");
            ViewBag.trip_type_id = new SelectList(db.trip_types, "id", "name");
            return View();
        }

        // POST: trip_share/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,driver_id,start_at_date,end_at_date,vehicle_id,created_at,created_by,trip_type_id")] trip_share trip_share)
        {
            if (ModelState.IsValid)
            {
                db.trip_share.Add(trip_share);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.driver_id = new SelectList(db.tbl_accounts, "id", "first_name", trip_share.driver_id);
            ViewBag.vehicle_id = new SelectList(db.tbl_vehicles, "id", "model", trip_share.vehicle_id);
            ViewBag.trip_type_id = new SelectList(db.trip_types, "id", "name", trip_share.trip_type_id);
            return View(trip_share);
        }

        // GET: trip_share/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_share trip_share = db.trip_share.Find(id);
            if (trip_share == null)
            {
                return HttpNotFound();
            }
            ViewBag.driver_id = new SelectList(db.tbl_accounts, "id", "first_name", trip_share.driver_id);
            ViewBag.vehicle_id = new SelectList(db.tbl_vehicles, "id", "model", trip_share.vehicle_id);
            ViewBag.trip_type_id = new SelectList(db.trip_types, "id", "name", trip_share.trip_type_id);
            return View(trip_share);
        }

        // POST: trip_share/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,driver_id,start_at_date,end_at_date,vehicle_id,created_at,created_by,trip_type_id")] trip_share trip_share)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trip_share).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.driver_id = new SelectList(db.tbl_accounts, "id", "first_name", trip_share.driver_id);
            ViewBag.vehicle_id = new SelectList(db.tbl_vehicles, "id", "model", trip_share.vehicle_id);
            ViewBag.trip_type_id = new SelectList(db.trip_types, "id", "name", trip_share.trip_type_id);
            return View(trip_share);
        }

        // GET: trip_share/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_share trip_share = db.trip_share.Find(id);
            if (trip_share == null)
            {
                return HttpNotFound();
            }
            return View(trip_share);
        }

        // POST: trip_share/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            trip_share trip_share = db.trip_share.Find(id);
            db.trip_share.Remove(trip_share);
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
