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
    public class tbl_accountsController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: tbl_accounts
        public ActionResult Index()
        {
            var tbl_accounts = db.tbl_accounts.Include(t => t.sec_users).Include(t => t.tbl_cities).Include(t => t.tbl_countries).Include(t => t.tbl_genders);
            return View(tbl_accounts.ToList());
        }

        // GET: tbl_accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_accounts tbl_accounts = db.tbl_accounts.Find(id);
            if (tbl_accounts == null)
            {
                return HttpNotFound();
            }
            return View(tbl_accounts);
        }

        // GET: tbl_accounts/Create
        public ActionResult Create()
        {
            ViewBag.id = new SelectList(db.sec_users, "id", "pwd");
            ViewBag.city_id = new SelectList(db.tbl_cities, "id", "name");
            ViewBag.country_id = new SelectList(db.tbl_countries, "id", "name");
            ViewBag.gender_id = new SelectList(db.tbl_genders, "id", "name");
            return View();
        }

        // POST: tbl_accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,first_name,last_name,date_of_birth,mobile,email,register_time,is_deleted,gender_id,id_no,driver_license_no,city_id,country_id")] tbl_accounts tbl_accounts)
        {
            if (ModelState.IsValid)
            {
                db.tbl_accounts.Add(tbl_accounts);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id = new SelectList(db.sec_users, "id", "pwd", tbl_accounts.id);
            ViewBag.city_id = new SelectList(db.tbl_cities, "id", "name", tbl_accounts.city_id);
            ViewBag.country_id = new SelectList(db.tbl_countries, "id", "name", tbl_accounts.country_id);
            ViewBag.gender_id = new SelectList(db.tbl_genders, "id", "name", tbl_accounts.gender_id);
            return View(tbl_accounts);
        }

        // GET: tbl_accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_accounts tbl_accounts = db.tbl_accounts.Find(id);
            if (tbl_accounts == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = new SelectList(db.sec_users, "id", "pwd", tbl_accounts.id);
            ViewBag.city_id = new SelectList(db.tbl_cities, "id", "name", tbl_accounts.city_id);
            ViewBag.country_id = new SelectList(db.tbl_countries, "id", "name", tbl_accounts.country_id);
            ViewBag.gender_id = new SelectList(db.tbl_genders, "id", "name", tbl_accounts.gender_id);
            return View(tbl_accounts);
        }

        // POST: tbl_accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,first_name,last_name,date_of_birth,mobile,email,register_time,is_deleted,gender_id,id_no,driver_license_no,city_id,country_id")] tbl_accounts tbl_accounts)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_accounts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id = new SelectList(db.sec_users, "id", "pwd", tbl_accounts.id);
            ViewBag.city_id = new SelectList(db.tbl_cities, "id", "name", tbl_accounts.city_id);
            ViewBag.country_id = new SelectList(db.tbl_countries, "id", "name", tbl_accounts.country_id);
            ViewBag.gender_id = new SelectList(db.tbl_genders, "id", "name", tbl_accounts.gender_id);
            return View(tbl_accounts);
        }

        // GET: tbl_accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_accounts tbl_accounts = db.tbl_accounts.Find(id);
            if (tbl_accounts == null)
            {
                return HttpNotFound();
            }
            return View(tbl_accounts);
        }

        // POST: tbl_accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_accounts tbl_accounts = db.tbl_accounts.Find(id);
            db.tbl_accounts.Remove(tbl_accounts);
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
