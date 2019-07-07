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
    public class tbl_setting_typesController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: tbl_setting_types
        public ActionResult Index()
        {
            return View(db.tbl_setting_types.ToList());
        }

        // GET: tbl_setting_types/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_setting_types tbl_setting_types = db.tbl_setting_types.Find(id);
            if (tbl_setting_types == null)
            {
                return HttpNotFound();
            }
            return View(tbl_setting_types);
        }

        // GET: tbl_setting_types/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tbl_setting_types/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name")] tbl_setting_types tbl_setting_types)
        {
            if (ModelState.IsValid)
            {
                db.tbl_setting_types.Add(tbl_setting_types);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_setting_types);
        }

        // GET: tbl_setting_types/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_setting_types tbl_setting_types = db.tbl_setting_types.Find(id);
            if (tbl_setting_types == null)
            {
                return HttpNotFound();
            }
            return View(tbl_setting_types);
        }

        // POST: tbl_setting_types/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name")] tbl_setting_types tbl_setting_types)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_setting_types).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_setting_types);
        }

        // GET: tbl_setting_types/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_setting_types tbl_setting_types = db.tbl_setting_types.Find(id);
            if (tbl_setting_types == null)
            {
                return HttpNotFound();
            }
            return View(tbl_setting_types);
        }

        // POST: tbl_setting_types/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_setting_types tbl_setting_types = db.tbl_setting_types.Find(id);
            db.tbl_setting_types.Remove(tbl_setting_types);
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
