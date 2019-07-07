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
    public class sec_mobile_verifyController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: sec_mobile_verify
        public ActionResult Index()
        {
            return View(db.sec_mobile_verify.ToList());
        }

        // GET: sec_mobile_verify/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_mobile_verify sec_mobile_verify = db.sec_mobile_verify.Find(id);
            if (sec_mobile_verify == null)
            {
                return HttpNotFound();
            }
            return View(sec_mobile_verify);
        }

        // GET: sec_mobile_verify/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: sec_mobile_verify/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,user_id,mobile,code,is_used,created_at,verification_id")] sec_mobile_verify sec_mobile_verify)
        {
            if (ModelState.IsValid)
            {
                db.sec_mobile_verify.Add(sec_mobile_verify);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sec_mobile_verify);
        }

        // GET: sec_mobile_verify/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_mobile_verify sec_mobile_verify = db.sec_mobile_verify.Find(id);
            if (sec_mobile_verify == null)
            {
                return HttpNotFound();
            }
            return View(sec_mobile_verify);
        }

        // POST: sec_mobile_verify/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,user_id,mobile,code,is_used,created_at,verification_id")] sec_mobile_verify sec_mobile_verify)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sec_mobile_verify).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sec_mobile_verify);
        }

        // GET: sec_mobile_verify/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_mobile_verify sec_mobile_verify = db.sec_mobile_verify.Find(id);
            if (sec_mobile_verify == null)
            {
                return HttpNotFound();
            }
            return View(sec_mobile_verify);
        }

        // POST: sec_mobile_verify/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sec_mobile_verify sec_mobile_verify = db.sec_mobile_verify.Find(id);
            db.sec_mobile_verify.Remove(sec_mobile_verify);
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
