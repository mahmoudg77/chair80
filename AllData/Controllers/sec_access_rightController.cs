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
    public class sec_access_rightController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: sec_access_right
        public ActionResult Index()
        {
            var sec_access_right = db.sec_access_right.Include(s => s.sec_roles);
            return View(sec_access_right.ToList());
        }

        // GET: sec_access_right/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_access_right sec_access_right = db.sec_access_right.Find(id);
            if (sec_access_right == null)
            {
                return HttpNotFound();
            }
            return View(sec_access_right);
        }

        // GET: sec_access_right/Create
        public ActionResult Create()
        {
            ViewBag.role_id = new SelectList(db.sec_roles, "id", "name");
            return View();
        }

        // POST: sec_access_right/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,role_id,model_name,method_name,force_filter")] sec_access_right sec_access_right)
        {
            if (ModelState.IsValid)
            {
                db.sec_access_right.Add(sec_access_right);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.role_id = new SelectList(db.sec_roles, "id", "name", sec_access_right.role_id);
            return View(sec_access_right);
        }

        // GET: sec_access_right/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_access_right sec_access_right = db.sec_access_right.Find(id);
            if (sec_access_right == null)
            {
                return HttpNotFound();
            }
            ViewBag.role_id = new SelectList(db.sec_roles, "id", "name", sec_access_right.role_id);
            return View(sec_access_right);
        }

        // POST: sec_access_right/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,role_id,model_name,method_name,force_filter")] sec_access_right sec_access_right)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sec_access_right).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.role_id = new SelectList(db.sec_roles, "id", "name", sec_access_right.role_id);
            return View(sec_access_right);
        }

        // GET: sec_access_right/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_access_right sec_access_right = db.sec_access_right.Find(id);
            if (sec_access_right == null)
            {
                return HttpNotFound();
            }
            return View(sec_access_right);
        }

        // POST: sec_access_right/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sec_access_right sec_access_right = db.sec_access_right.Find(id);
            db.sec_access_right.Remove(sec_access_right);
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
