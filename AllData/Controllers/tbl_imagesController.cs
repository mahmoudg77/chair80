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
    public class tbl_imagesController : Controller
    {
        private chari80_dbEntities db = new chari80_dbEntities();

        // GET: tbl_images
        public ActionResult Index()
        {
            return View(db.tbl_images.ToList());
        }

        // GET: tbl_images/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_images tbl_images = db.tbl_images.Find(id);
            if (tbl_images == null)
            {
                return HttpNotFound();
            }
            return View(tbl_images);
        }

        // GET: tbl_images/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tbl_images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,model_name,model_id,model_tag,thumb,meduim,large,original,created_at,created_by")] tbl_images tbl_images)
        {
            if (ModelState.IsValid)
            {
                db.tbl_images.Add(tbl_images);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_images);
        }

        // GET: tbl_images/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_images tbl_images = db.tbl_images.Find(id);
            if (tbl_images == null)
            {
                return HttpNotFound();
            }
            return View(tbl_images);
        }

        // POST: tbl_images/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,model_name,model_id,model_tag,thumb,meduim,large,original,created_at,created_by")] tbl_images tbl_images)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_images).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_images);
        }

        // GET: tbl_images/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_images tbl_images = db.tbl_images.Find(id);
            if (tbl_images == null)
            {
                return HttpNotFound();
            }
            return View(tbl_images);
        }

        // POST: tbl_images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_images tbl_images = db.tbl_images.Find(id);
            db.tbl_images.Remove(tbl_images);
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
