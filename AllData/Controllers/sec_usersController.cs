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
    public class sec_usersController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: sec_users
        public ActionResult Index()
        {
            var sec_users = db.sec_users.Include(s => s.tbl_accounts);
            return View(sec_users.ToList());
        }

        // GET: sec_users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_users sec_users = db.sec_users.Find(id);
            if (sec_users == null)
            {
                return HttpNotFound();
            }
            return View(sec_users);
        }

        // GET: sec_users/Create
        public ActionResult Create()
        {
            ViewBag.id = new SelectList(db.tbl_accounts, "id", "first_name");
            return View();
        }

        // POST: sec_users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,pwd,reset_pwd_token,facebook_token,twitter_token,google_token,instagram_token,confirm_mail_token,mail_verified,phone_verified,firebase_uid")] sec_users sec_users)
        {
            if (ModelState.IsValid)
            {
                db.sec_users.Add(sec_users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id = new SelectList(db.tbl_accounts, "id", "first_name", sec_users.id);
            return View(sec_users);
        }

        // GET: sec_users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_users sec_users = db.sec_users.Find(id);
            if (sec_users == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = new SelectList(db.tbl_accounts, "id", "first_name", sec_users.id);
            return View(sec_users);
        }

        // POST: sec_users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,pwd,reset_pwd_token,facebook_token,twitter_token,google_token,instagram_token,confirm_mail_token,mail_verified,phone_verified,firebase_uid")] sec_users sec_users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sec_users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id = new SelectList(db.tbl_accounts, "id", "first_name", sec_users.id);
            return View(sec_users);
        }

        // GET: sec_users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sec_users sec_users = db.sec_users.Find(id);
            if (sec_users == null)
            {
                return HttpNotFound();
            }
            return View(sec_users);
        }

        // POST: sec_users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sec_users sec_users = db.sec_users.Find(id);
            db.sec_users.Remove(sec_users);
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
