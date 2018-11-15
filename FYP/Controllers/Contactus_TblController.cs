using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FYP.Models;
using System.Net.Mail;


namespace FYP.Controllers
{
    public class Contactus_TblController : HandleSessionController
    {
        Users_Tbl u = new Users_Tbl();
        private FYPBDEntities db = new FYPBDEntities();

        // GET: Contactus_Tbl
        public ActionResult Index()
        {
            ViewData["ED"] = "";
            if(Session["id"]!=null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if(u.RollId==1)
                { 
                return View(db.Contactus_Tbl.OrderByDescending(x=>x.ContactId).ToList());
                }
                return RedirectToAction("Index","Home");
            }
            return RedirectToAction("Index","Home");
            
        }

     

        // GET: Contactus_Tbl/Create
        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        
        public ActionResult Create(Contactus_Tbl contactus_Tbl)
        {
            
            
            if (ModelState.IsValid)
            {
                MailMessage mm = new MailMessage("systemcovert@gmail.com", "systemcovert@gmail.com");
                mm.Subject = "Covert-- ReplyTo:" + contactus_Tbl.Email;
                mm.Body = contactus_Tbl.Name +":\n"+contactus_Tbl.Message;
                mm.IsBodyHtml = false;



                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;


                NetworkCredential nc = new NetworkCredential("systemcovert@gmail.com", "adminofcovert");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = nc;
                smtp.Send(mm);

                db.Contactus_Tbl.Add(contactus_Tbl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(contactus_Tbl);
        }
        public ActionResult newsletter(string emailsss)
        {
            Contactus_Tbl contactus_Tbl = new Contactus_Tbl();
            if (emailsss != null)
            {
                contactus_Tbl.Email = emailsss;
                contactus_Tbl.Name = "Request";
                contactus_Tbl.Message = "Newsletter";
                db.Contactus_Tbl.Add(contactus_Tbl);
                db.SaveChanges();
            }
            return RedirectToAction("index","home");
        }
        

        // GET: Contactus_Tbl/Edit/5
       

        // GET: Contactus_Tbl/Delete/5
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
            Contactus_Tbl contactus_Tbl = db.Contactus_Tbl.Find(id);
            if (contactus_Tbl == null)
            {
                return HttpNotFound();
            }
            return View(contactus_Tbl);
        }
               return RedirectToAction("Index","Home");
    }
            return RedirectToAction("Index","Home");

}

        // POST: Contactus_Tbl/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contactus_Tbl contactus_Tbl = db.Contactus_Tbl.Find(id);
            db.Contactus_Tbl.Remove(contactus_Tbl);
            db.SaveChanges();
            ViewData["ED"] = "Successfully Deleted";
            return View("Index",db.Contactus_Tbl.OrderByDescending(s=>s.ContactId).ToList());
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
