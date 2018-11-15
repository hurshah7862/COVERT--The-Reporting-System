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
    public class Comment_TblController : HandleSessionController
    {
        private FYPBDEntities db = new FYPBDEntities();
        Comment_Tbl c = new Comment_Tbl();
        Users_Tbl u = new Users_Tbl();
        Posts_Tbl p = new Posts_Tbl();

        // GET: Comment_Tbl
        [HttpGet]
        public ActionResult Index(int? id)
        {

            ViewData["Error"] = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            

            if (id != null)
            {
            
            Session["pid"] = id;
            p = db.Posts_Tbl.Find(id);
                if (p == null)
                {
                    return HttpNotFound();
                }
                try
            {
                List<Comment_Tbl> clist = db.Comment_Tbl.Where(x => x.PostId == p.PostId).ToList();
                if (clist != null)
                {
                        
                    return View(clist);
                }
            }
            catch 
                {
                    return View(db.Comment_Tbl.Where(x => x.CommentId == -1).ToList());

                }
            }
            return View(db.Comment_Tbl.Where(x => x.CommentId == -1).ToList());
        }

        // GET: Comment_Tbl/Details/5
      
        [HttpPost]
        public ActionResult Create( string Comment)
        {
            if(Comment=="")
            {
                ViewData["Error"] = "Please write something";
                return RedirectToAction("index", new { id=Session["pid"]});
            }

            if (Session["id"]!=null)
            { 
            u = db.Users_Tbl.Find(Session["id"]);
                p = db.Posts_Tbl.Find(Session["pid"]);
              if (u!=null && p!=null && u.Status=="Approved")  
            {
                    c.PostId = p.PostId;
                    c.UserId = u.UserId;
                    c.Date = Convert.ToString(DateTime.Now);
                    c.Name = Comment;

                    db.Comment_Tbl.Add(c);
                    db.SaveChanges();
                    ViewData["Error"] = "Successfully Added";
                    return View("Index",db.Comment_Tbl.Where(x=>x.PostId==p.PostId));
            }
                return RedirectToAction("index","Posts_Tbl");
            }
            return RedirectToAction("index", "Posts_Tbl");
        }
      

        // GET: Comment_Tbl/Edit/5
        public ActionResult Edit(int? id)
        {





            if(Session["id"]!=null)
            {
                c = db.Comment_Tbl.Find(id);
                u = db.Users_Tbl.Find(Session["id"]);
                if(c.UserId==u.UserId || u.RollId==1)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Comment_Tbl comment_Tbl = db.Comment_Tbl.Find(id);
                    if (comment_Tbl == null)
                    {
                        return HttpNotFound();
                    }
                    return View(comment_Tbl);
                }
                return RedirectToAction("index", new { id = c.PostId });
            }
            return RedirectToAction("index", new { id = c.PostId });



        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommentId,UserId,PostId,Name,Date")] Comment_Tbl comment_Tbl)
        {
            if(comment_Tbl.Name!=null)
            {
               
            if (ModelState.IsValid)
            {
                db.Entry(comment_Tbl).State = EntityState.Modified;
                db.SaveChanges();
                    ViewData["ED"] = "Successfully edited";
                    return View("Index",db.Comment_Tbl.Where(x=>x.PostId==comment_Tbl.PostId));
            }
            return View(comment_Tbl);
            }
            ViewData["ED"] = "Write something";

            return View(comment_Tbl);
        }

        // GET: Comment_Tbl/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["id"] != null)
            {
                c = db.Comment_Tbl.Find(id);
                u = db.Users_Tbl.Find(Session["id"]);
                if (c.UserId == u.UserId || u.RollId == 1)
                {
                    if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment_Tbl comment_Tbl = db.Comment_Tbl.Find(id);
            if (comment_Tbl == null)
            {
                return HttpNotFound();
            }
            return View(comment_Tbl);
        }
                return RedirectToAction("index", new { id = c.PostId });
            }
            return RedirectToAction("index", new { id = c.PostId });



        }


        // POST: Comment_Tbl/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment_Tbl comment_Tbl = db.Comment_Tbl.Find(id);
            db.Comment_Tbl.Remove(comment_Tbl);
            db.SaveChanges();
            ViewData["ED"] = "Successfully deleted";

            return View("Index",db.Comment_Tbl.Where(x=>x.PostId==comment_Tbl.PostId));
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
