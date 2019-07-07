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
    public class tbl_countriesController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: tbl_countries
        public ActionResult Index()
        {
            return View(db.tbl_countries.ToList());
        }

        // GET: tbl_countries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_countries tbl_countries = db.tbl_countries.Find(id);
            if (tbl_countries == null)
            {
                return HttpNotFound();
            }
            return View(tbl_countries);
        }

        // GET: tbl_countries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tbl_countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name")] tbl_countries tbl_countries)
        {
            if (ModelState.IsValid)
            {
                db.tbl_countries.Add(tbl_countries);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_countries);
        }

        // GET: tbl_countries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_countries tbl_countries = db.tbl_countries.Find(id);
            if (tbl_countries == null)
            {
                return HttpNotFound();
            }
            return View(tbl_countries);
        }

        // POST: tbl_countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name")] tbl_countries tbl_countries)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_countries).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_countries);
        }

        // GET: tbl_countries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_countries tbl_countries = db.tbl_countries.Find(id);
            if (tbl_countries == null)
            {
                return HttpNotFound();
            }
            return View(tbl_countries);
        }

        // POST: tbl_countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_countries tbl_countries = db.tbl_countries.Find(id);
            db.tbl_countries.Remove(tbl_countries);
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
