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
    public class tbl_citiesController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: tbl_cities
        public ActionResult Index()
        {
            var tbl_cities = db.tbl_cities.Include(t => t.tbl_countries);
            return View(tbl_cities.ToList());
        }

        // GET: tbl_cities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_cities tbl_cities = db.tbl_cities.Find(id);
            if (tbl_cities == null)
            {
                return HttpNotFound();
            }
            return View(tbl_cities);
        }

        // GET: tbl_cities/Create
        public ActionResult Create()
        {
            ViewBag.country_id = new SelectList(db.tbl_countries, "id", "name");
            return View();
        }

        // POST: tbl_cities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,country_id,name")] tbl_cities tbl_cities)
        {
            if (ModelState.IsValid)
            {
                db.tbl_cities.Add(tbl_cities);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.country_id = new SelectList(db.tbl_countries, "id", "name", tbl_cities.country_id);
            return View(tbl_cities);
        }

        // GET: tbl_cities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_cities tbl_cities = db.tbl_cities.Find(id);
            if (tbl_cities == null)
            {
                return HttpNotFound();
            }
            ViewBag.country_id = new SelectList(db.tbl_countries, "id", "name", tbl_cities.country_id);
            return View(tbl_cities);
        }

        // POST: tbl_cities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,country_id,name")] tbl_cities tbl_cities)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_cities).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.country_id = new SelectList(db.tbl_countries, "id", "name", tbl_cities.country_id);
            return View(tbl_cities);
        }

        // GET: tbl_cities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_cities tbl_cities = db.tbl_cities.Find(id);
            if (tbl_cities == null)
            {
                return HttpNotFound();
            }
            return View(tbl_cities);
        }

        // POST: tbl_cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_cities tbl_cities = db.tbl_cities.Find(id);
            db.tbl_cities.Remove(tbl_cities);
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
