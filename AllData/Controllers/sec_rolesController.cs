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
    public class sec_rolesController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: sec_roles
        public ActionResult Index()
        {
            return View(db.sec_roles.ToList());
        }

        // GET: sec_roles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_roles sec_roles = db.sec_roles.Find(id);
            if (sec_roles == null)
            {
                return HttpNotFound();
            }
            return View(sec_roles);
        }

        // GET: sec_roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: sec_roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,role_key,description")] sec_roles sec_roles)
        {
            if (ModelState.IsValid)
            {
                db.sec_roles.Add(sec_roles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sec_roles);
        }

        // GET: sec_roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_roles sec_roles = db.sec_roles.Find(id);
            if (sec_roles == null)
            {
                return HttpNotFound();
            }
            return View(sec_roles);
        }

        // POST: sec_roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,role_key,description")] sec_roles sec_roles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sec_roles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sec_roles);
        }

        // GET: sec_roles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_roles sec_roles = db.sec_roles.Find(id);
            if (sec_roles == null)
            {
                return HttpNotFound();
            }
            return View(sec_roles);
        }

        // POST: sec_roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sec_roles sec_roles = db.sec_roles.Find(id);
            db.sec_roles.Remove(sec_roles);
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
