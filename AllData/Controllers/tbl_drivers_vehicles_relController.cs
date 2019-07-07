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
    public class tbl_drivers_vehicles_relController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: tbl_drivers_vehicles_rel
        public ActionResult Index()
        {
            var tbl_drivers_vehicles_rel = db.tbl_drivers_vehicles_rel.Include(t => t.tbl_accounts).Include(t => t.tbl_vehicles);
            return View(tbl_drivers_vehicles_rel.ToList());
        }

        // GET: tbl_drivers_vehicles_rel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_drivers_vehicles_rel tbl_drivers_vehicles_rel = db.tbl_drivers_vehicles_rel.Find(id);
            if (tbl_drivers_vehicles_rel == null)
            {
                return HttpNotFound();
            }
            return View(tbl_drivers_vehicles_rel);
        }

        // GET: tbl_drivers_vehicles_rel/Create
        public ActionResult Create()
        {
            ViewBag.driver_id = new SelectList(db.tbl_accounts, "id", "first_name");
            ViewBag.vehicle_id = new SelectList(db.tbl_vehicles, "id", "model");
            return View();
        }

        // POST: tbl_drivers_vehicles_rel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,driver_id,vehicle_id,created_at,created_by")] tbl_drivers_vehicles_rel tbl_drivers_vehicles_rel)
        {
            if (ModelState.IsValid)
            {
                db.tbl_drivers_vehicles_rel.Add(tbl_drivers_vehicles_rel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.driver_id = new SelectList(db.tbl_accounts, "id", "first_name", tbl_drivers_vehicles_rel.driver_id);
            ViewBag.vehicle_id = new SelectList(db.tbl_vehicles, "id", "model", tbl_drivers_vehicles_rel.vehicle_id);
            return View(tbl_drivers_vehicles_rel);
        }

        // GET: tbl_drivers_vehicles_rel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_drivers_vehicles_rel tbl_drivers_vehicles_rel = db.tbl_drivers_vehicles_rel.Find(id);
            if (tbl_drivers_vehicles_rel == null)
            {
                return HttpNotFound();
            }
            ViewBag.driver_id = new SelectList(db.tbl_accounts, "id", "first_name", tbl_drivers_vehicles_rel.driver_id);
            ViewBag.vehicle_id = new SelectList(db.tbl_vehicles, "id", "model", tbl_drivers_vehicles_rel.vehicle_id);
            return View(tbl_drivers_vehicles_rel);
        }

        // POST: tbl_drivers_vehicles_rel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,driver_id,vehicle_id,created_at,created_by")] tbl_drivers_vehicles_rel tbl_drivers_vehicles_rel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_drivers_vehicles_rel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.driver_id = new SelectList(db.tbl_accounts, "id", "first_name", tbl_drivers_vehicles_rel.driver_id);
            ViewBag.vehicle_id = new SelectList(db.tbl_vehicles, "id", "model", tbl_drivers_vehicles_rel.vehicle_id);
            return View(tbl_drivers_vehicles_rel);
        }

        // GET: tbl_drivers_vehicles_rel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_drivers_vehicles_rel tbl_drivers_vehicles_rel = db.tbl_drivers_vehicles_rel.Find(id);
            if (tbl_drivers_vehicles_rel == null)
            {
                return HttpNotFound();
            }
            return View(tbl_drivers_vehicles_rel);
        }

        // POST: tbl_drivers_vehicles_rel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_drivers_vehicles_rel tbl_drivers_vehicles_rel = db.tbl_drivers_vehicles_rel.Find(id);
            db.tbl_drivers_vehicles_rel.Remove(tbl_drivers_vehicles_rel);
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
