using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FYP.Models;

namespace FYP.Controllers
{
    public class Category_TblController : HandleSessionController
    {
        Users_Tbl u = new Users_Tbl();
        private FYPBDEntities db = new FYPBDEntities();

        // GET: Category_Tbl
        public ActionResult Index()
        {
            ViewData["Error"] = "";
            ViewData["ED"] = "";

            if (Session["id"]!=null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if (u.RollId==1)
                { 
                    return View(db.Category_Tbl.ToList());
                }
                return RedirectToAction("index", "home");

            }
            return RedirectToAction("index","home");
        }

       

        [HttpPost]
       
        public ActionResult Create(string name)
        {
            if(name=="")
            {
                ViewData["Error"] = "Please Enter some thing";
                return RedirectToAction("Index");
            }
            Category_Tbl category_Tbl = new Category_Tbl();
            category_Tbl.Name = name;
            Category_Tbl c = db.Category_Tbl.FirstOrDefault(x => x.Name == category_Tbl.Name);
            if(c!=null)
            {
                ViewData["Error"] = "Already exists";
                return View("Index", db.Category_Tbl.ToList());
            }
            

            if (ModelState.IsValid)
            {
                db.Category_Tbl.Add(category_Tbl);
                db.SaveChanges();
                ViewData["Error"] = "Successfully Added";
                return View("Index", db.Category_Tbl.ToList());
            }

            return RedirectToAction("Index");

        }


        public ActionResult Edit(int? id)
        {
            if (Session["id"] != null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if(u.RollId==1)
                { 

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category_Tbl category_Tbl = db.Category_Tbl.Find(id);
                if (category_Tbl == null)
                {
                    return HttpNotFound();
                }
                    Session["cat"] = category_Tbl.Name;
                return View(category_Tbl);
                }
                return RedirectToAction("index", "home");

        }
                    return RedirectToAction("index", "home");

    }

    
    [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryId,Name")] Category_Tbl category_Tbl)
        {
            
            if(category_Tbl.Name==null)
            {
                return View(category_Tbl);

            }
            if(Session["cat"].ToString()!=category_Tbl.Name)
            { 
            Category_Tbl c = db.Category_Tbl.FirstOrDefault(x => x.Name == category_Tbl.Name);
            if(c!=null)
            {
                ViewData["Error"] = "Category already exists";
                return View(category_Tbl);
            }
            }

            if (ModelState.IsValid)
            {
                db.Entry(category_Tbl).State = EntityState.Modified;
                db.SaveChanges();
                ViewData["ED"] = "Successfully edited";
                return View("Index", db.Category_Tbl.ToList());
            
            }
            return View(category_Tbl);

        }

        // GET: Category_Tbl/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["id"] != null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if (u.RollId == 1)
                {



                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Category_Tbl category_Tbl = db.Category_Tbl.Find(id);
                    if (category_Tbl == null)
                    {
                        return HttpNotFound();
                    }
                    return View(category_Tbl);

                }
                return RedirectToAction("index", "home");

            }
            return RedirectToAction("index", "home");
        }

        // POST: Category_Tbl/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                List<Posts_Tbl> pl = db.Posts_Tbl.Where(x => x.CategoryId == id).ToList();
                foreach (var item in pl)
                {
                    try
                    {
                        List<Comment_Tbl> cl = db.Comment_Tbl.Where(x => x.PostId == item.PostId).ToList();
                        foreach (var comment in cl)
                        {
                            db.Comment_Tbl.Remove(comment);
                        }
                        db.Posts_Tbl.Remove(item);
                    }
                    catch (Exception)
                    {
                        db.Posts_Tbl.Remove(item);
                    }
                }
                    Category_Tbl category_Tbl = db.Category_Tbl.Find(id);
                    db.Category_Tbl.Remove(category_Tbl);
                    db.SaveChanges();
                    ViewData["ED"] = "Successfully deleted";
                    return View("Index", db.Category_Tbl.ToList());
            }
            
            catch (Exception)
            {
                Category_Tbl category_Tbl = db.Category_Tbl.Find(id);
                db.Category_Tbl.Remove(category_Tbl);
                db.SaveChanges();
                ViewData["ED"] = "Successfully deleted";
                return View("Index", db.Category_Tbl.ToList());
            }
            

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
