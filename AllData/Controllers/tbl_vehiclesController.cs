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
    public class tbl_vehiclesController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: tbl_vehicles
        public ActionResult Index()
        {
            var tbl_vehicles = db.tbl_vehicles.Include(t => t.tbl_accounts);
            return View(tbl_vehicles.ToList());
        }

        // GET: tbl_vehicles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_vehicles tbl_vehicles = db.tbl_vehicles.Find(id);
            if (tbl_vehicles == null)
            {
                return HttpNotFound();
            }
            return View(tbl_vehicles);
        }

        // GET: tbl_vehicles/Create
        public ActionResult Create()
        {
            ViewBag.owner_id = new SelectList(db.tbl_accounts, "id", "first_name");
            return View();
        }

        // POST: tbl_vehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,model,color,capacity,owner_id,created_at,created_by,license_no")] tbl_vehicles tbl_vehicles)
        {
            if (ModelState.IsValid)
            {
                db.tbl_vehicles.Add(tbl_vehicles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.owner_id = new SelectList(db.tbl_accounts, "id", "first_name", tbl_vehicles.owner_id);
            return View(tbl_vehicles);
        }

        // GET: tbl_vehicles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_vehicles tbl_vehicles = db.tbl_vehicles.Find(id);
            if (tbl_vehicles == null)
            {
                return HttpNotFound();
            }
            ViewBag.owner_id = new SelectList(db.tbl_accounts, "id", "first_name", tbl_vehicles.owner_id);
            return View(tbl_vehicles);
        }

        // POST: tbl_vehicles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,model,color,capacity,owner_id,created_at,created_by,license_no")] tbl_vehicles tbl_vehicles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_vehicles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.owner_id = new SelectList(db.tbl_accounts, "id", "first_name", tbl_vehicles.owner_id);
            return View(tbl_vehicles);
        }

        // GET: tbl_vehicles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_vehicles tbl_vehicles = db.tbl_vehicles.Find(id);
            if (tbl_vehicles == null)
            {
                return HttpNotFound();
            }
            return View(tbl_vehicles);
        }

        // POST: tbl_vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_vehicles tbl_vehicles = db.tbl_vehicles.Find(id);
            db.tbl_vehicles.Remove(tbl_vehicles);
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
