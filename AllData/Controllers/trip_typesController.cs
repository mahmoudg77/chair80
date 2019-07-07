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
    public class trip_typesController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: trip_types
        public ActionResult Index()
        {
            return View(db.trip_types.ToList());
        }

        // GET: trip_types/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_types trip_types = db.trip_types.Find(id);
            if (trip_types == null)
            {
                return HttpNotFound();
            }
            return View(trip_types);
        }

        // GET: trip_types/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: trip_types/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name")] trip_types trip_types)
        {
            if (ModelState.IsValid)
            {
                db.trip_types.Add(trip_types);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trip_types);
        }

        // GET: trip_types/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_types trip_types = db.trip_types.Find(id);
            if (trip_types == null)
            {
                return HttpNotFound();
            }
            return View(trip_types);
        }

        // POST: trip_types/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name")] trip_types trip_types)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trip_types).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trip_types);
        }

        // GET: trip_types/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            trip_types trip_types = db.trip_types.Find(id);
            if (trip_types == null)
            {
                return HttpNotFound();
            }
            return View(trip_types);
        }

        // POST: trip_types/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            trip_types trip_types = db.trip_types.Find(id);
            db.trip_types.Remove(trip_types);
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
