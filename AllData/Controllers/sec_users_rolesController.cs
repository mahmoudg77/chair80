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
    public class sec_users_rolesController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: sec_users_roles
        public ActionResult Index()
        {
            var sec_users_roles = db.sec_users_roles.Include(s => s.sec_roles).Include(s => s.sec_users);
            return View(sec_users_roles.ToList());
        }

        // GET: sec_users_roles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_users_roles sec_users_roles = db.sec_users_roles.Find(id);
            if (sec_users_roles == null)
            {
                return HttpNotFound();
            }
            return View(sec_users_roles);
        }

        // GET: sec_users_roles/Create
        public ActionResult Create()
        {
            ViewBag.role_id = new SelectList(db.sec_roles, "id", "name");
            ViewBag.user_id = new SelectList(db.sec_users, "id", "pwd");
            return View();
        }

        // POST: sec_users_roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,user_id,role_id")] sec_users_roles sec_users_roles)
        {
            if (ModelState.IsValid)
            {
                db.sec_users_roles.Add(sec_users_roles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.role_id = new SelectList(db.sec_roles, "id", "name", sec_users_roles.role_id);
            ViewBag.user_id = new SelectList(db.sec_users, "id", "id", sec_users_roles.user_id);
            return View(sec_users_roles);
        }

        // GET: sec_users_roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_users_roles sec_users_roles = db.sec_users_roles.Find(id);
            if (sec_users_roles == null)
            {
                return HttpNotFound();
            }
            ViewBag.role_id = new SelectList(db.sec_roles, "id", "name", sec_users_roles.role_id);
            ViewBag.user_id = new SelectList(db.sec_users, "id", "id", sec_users_roles.user_id);
            return View(sec_users_roles);
        }

        // POST: sec_users_roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,user_id,role_id")] sec_users_roles sec_users_roles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sec_users_roles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.role_id = new SelectList(db.sec_roles, "id", "name", sec_users_roles.role_id);
            ViewBag.user_id = new SelectList(db.sec_users, "id", "id", sec_users_roles.user_id);
            return View(sec_users_roles);
        }

        // GET: sec_users_roles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_users_roles sec_users_roles = db.sec_users_roles.Find(id);
            if (sec_users_roles == null)
            {
                return HttpNotFound();
            }
            return View(sec_users_roles);
        }

        // POST: sec_users_roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sec_users_roles sec_users_roles = db.sec_users_roles.Find(id);
            db.sec_users_roles.Remove(sec_users_roles);
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
