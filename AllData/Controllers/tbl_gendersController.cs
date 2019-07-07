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
    public class tbl_gendersController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: tbl_genders
        public ActionResult Index()
        {
            return View(db.tbl_genders.ToList());
        }

        // GET: tbl_genders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_genders tbl_genders = db.tbl_genders.Find(id);
            if (tbl_genders == null)
            {
                return HttpNotFound();
            }
            return View(tbl_genders);
        }

        // GET: tbl_genders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tbl_genders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name")] tbl_genders tbl_genders)
        {
            if (ModelState.IsValid)
            {
                db.tbl_genders.Add(tbl_genders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_genders);
        }

        // GET: tbl_genders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_genders tbl_genders = db.tbl_genders.Find(id);
            if (tbl_genders == null)
            {
                return HttpNotFound();
            }
            return View(tbl_genders);
        }

        // POST: tbl_genders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name")] tbl_genders tbl_genders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_genders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_genders);
        }

        // GET: tbl_genders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_genders tbl_genders = db.tbl_genders.Find(id);
            if (tbl_genders == null)
            {
                return HttpNotFound();
            }
            return View(tbl_genders);
        }

        // POST: tbl_genders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_genders tbl_genders = db.tbl_genders.Find(id);
            db.tbl_genders.Remove(tbl_genders);
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
