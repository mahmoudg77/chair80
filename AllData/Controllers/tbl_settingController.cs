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
    public class tbl_settingController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: tbl_setting
        public ActionResult Index()
        {
            return View(db.tbl_setting.ToList());
        }

        // GET: tbl_setting/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_setting tbl_setting = db.tbl_setting.Find(id);
            if (tbl_setting == null)
            {
                return HttpNotFound();
            }
            return View(tbl_setting);
        }

        // GET: tbl_setting/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tbl_setting/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,setting_key,setting_name,setting_value,setting_type,datasource_url,datasource_json,setting_group,display,sequance")] tbl_setting tbl_setting)
        {
            if (ModelState.IsValid)
            {
                db.tbl_setting.Add(tbl_setting);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_setting);
        }

        // GET: tbl_setting/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_setting tbl_setting = db.tbl_setting.Find(id);
            if (tbl_setting == null)
            {
                return HttpNotFound();
            }
            return View(tbl_setting);
        }

        // POST: tbl_setting/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,setting_key,setting_name,setting_value,setting_type,datasource_url,datasource_json,setting_group,display,sequance")] tbl_setting tbl_setting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_setting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_setting);
        }

        // GET: tbl_setting/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_setting tbl_setting = db.tbl_setting.Find(id);
            if (tbl_setting == null)
            {
                return HttpNotFound();
            }
            return View(tbl_setting);
        }

        // POST: tbl_setting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_setting tbl_setting = db.tbl_setting.Find(id);
            db.tbl_setting.Remove(tbl_setting);
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
