using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FYP.Models;
using PagedList;
using PagedList.Mvc;
using System.Net.Mail;

namespace FYP.Controllers
{
    public class Users_TblController : HandleSessionController
    {
        Users_Tbl u = new Users_Tbl();
        
        private FYPBDEntities db = new FYPBDEntities();

        public ActionResult pendingusers(/*int ? page*/)
        {
            if(Session["id"]!=null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if(u.RollId==1)
                {
                    ViewData["Error"] = "";
                    return View(db.Users_Tbl.Where(x => x.Status == "pending" && (x.AreaId==u.AreaId || u.DesignationId==1)).OrderByDescending(x => x.UserId).ToList()/*.ToPagedList(page ?? 1,10)*/);
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult logout()
        {
            if (Session["id"] != null)
            {
                Session["id"] = null;
            }
            return RedirectToAction("Index","Home");
        }
        
        [HttpGet]
        public ActionResult login()
        {
            if(Session["id"]==null)
            { 
            ViewData["login"] = "";
            return View();
            }
            return RedirectToAction("index", "home");
        }
        [HttpPost]
        public ActionResult login( Users_Tbl us)
        {
            u = db.Users_Tbl.FirstOrDefault(x=>x.Username==us.Username && x.Password==us.Password);
            if(u!=null)
            {
                Session["id"] = u.UserId;
                return RedirectToAction("Index", "Home");
            }
            ViewData["login"] = "Please enter correct info";
            return View("login");
        }

        // GET: Users_Tbl
        public ActionResult Index(string Search , int? page)
            
        {
            
            if(Session["id"]!=null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if(u.RollId==1)
                {
                    ViewData["Error"] = "";
                    return View(db.Users_Tbl.Where(x =>(( x.Name.Contains(Search) && x.Status=="Approved") || (Search == null && x.Status=="Approved"))).ToList().ToPagedList(page ?? 1,10));
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");

        }

        // GET: Users_Tbl/Details/5
        public ActionResult Details(int? id)
        {
            if(Session["id"]!=null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
               if(u.RollId==1 || u.UserId==id)
                {

                
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users_Tbl users_Tbl = db.Users_Tbl.Find(id);
            if (users_Tbl == null)
            {
                return HttpNotFound();
            }
            return View(users_Tbl);
        }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Users_Tbl/Create
        public ActionResult Create()
        {
            ViewBag.rollid = new SelectList(db.Roles_Tbl, "RollId", "RollName");
            ViewBag.desid = new SelectList(db.Designation_Tbl, "DesignationId", "DesignationName");
            ViewBag.areaid = new SelectList(db.Area_Tbl, "AreaId", "AreaName");
            if (Session["id"] != null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if (u.RollId == 2)
                    return Redirect("Index");
                else if (u.RollId == 1)
                {
                    return View();
                }
            }
                return View();
            
        }

        // POST: Users_Tbl/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult Create(string rollid, string Status, string name, string username, string cnic, string email, string number, string password, string confirmpassword, string desid, string areaid)
        {
            u = db.Users_Tbl.FirstOrDefault(x => x.Username == username || x.Email == email);
            if(u!=null)
            {
                ViewBag.desid = new SelectList(db.Designation_Tbl, "DesignationId", "DesignationName");
                ViewBag.areaid = new SelectList(db.Area_Tbl, "AreaId", "AreaName");
                ViewBag.rollid = new SelectList(db.Roles_Tbl, "RollId", "RollName");
                ViewData["Error"] = "Please select some other email or username";
                return View();
            }
            if (password != confirmpassword)
            {
                ViewBag.desid = new SelectList(db.Designation_Tbl, "DesignationId", "DesignationName");
                ViewBag.areaid = new SelectList(db.Area_Tbl, "AreaId", "AreaName");
                ViewBag.rollid = new SelectList(db.Roles_Tbl, "RollId", "RollName");
                ViewData["Error"] = "Password and confirm password are not matching";
                return View();
            }
            if(username.Count()<5 || username.Count()>12)
            {
                ViewBag.desid = new SelectList(db.Designation_Tbl, "DesignationId", "DesignationName");
                ViewBag.areaid = new SelectList(db.Area_Tbl, "AreaId", "AreaName");
                ViewBag.rollid = new SelectList(db.Roles_Tbl, "RollId", "RollName");
                ViewData["Error"] = "Username should be minimum 5 characters and maximum 12 characters";
                return View();

            }
            if(password.Count()<8 || password.Count()>12)
            {
                ViewBag.desid = new SelectList(db.Designation_Tbl, "DesignationId", "DesignationName");
                ViewBag.areaid = new SelectList(db.Area_Tbl, "AreaId", "AreaName");
                ViewBag.rollid = new SelectList(db.Roles_Tbl, "RollId", "RollName");
                ViewData["Error"] = "Password should be minimum 8 characters and maximum 12 characters";
                return View();

            }
            u = db.Users_Tbl.FirstOrDefault(x => x.CNIC == cnic);
            if(u!=null)
            {
                ViewBag.desid = new SelectList(db.Designation_Tbl, "DesignationId", "DesignationName");
                ViewBag.areaid = new SelectList(db.Area_Tbl, "AreaId", "AreaName");
                ViewBag.rollid = new SelectList(db.Roles_Tbl, "RollId", "RollName");
                ViewData["Error"] = "CNIC already registered";
                return View();

            }
            if(number.Contains('_'))
            {
                ViewBag.desid = new SelectList(db.Designation_Tbl, "DesignationId", "DesignationName");
                ViewBag.areaid = new SelectList(db.Area_Tbl, "AreaId", "AreaName");
                ViewBag.rollid = new SelectList(db.Roles_Tbl, "RollId", "RollName");
                ViewData["Error"] = "Number Incomplete";
                return View();

            }
            if(cnic.Contains('_'))
            {
                ViewBag.desid = new SelectList(db.Designation_Tbl, "DesignationId", "DesignationName");
                ViewBag.areaid = new SelectList(db.Area_Tbl, "AreaId", "AreaName");
                ViewBag.rollid = new SelectList(db.Roles_Tbl, "RollId", "RollName");
                ViewData["Error"] = "CNIC Incomplete";
                return View();

            }
            if(areaid=="")
            {
                ViewBag.desid = new SelectList(db.Designation_Tbl, "DesignationId", "DesignationName");
                ViewBag.areaid = new SelectList(db.Area_Tbl, "AreaId", "AreaName");
                ViewBag.rollid = new SelectList(db.Roles_Tbl, "RollId", "RollName");
                ViewData["Error"] = "Please Select an area";
                return View();

            }

           
            else
            {
                Users_Tbl users_Tbl = new Users_Tbl();


            
                if(Session["id"]==null)
                {
                    users_Tbl.RollId = 2;
                    users_Tbl.Status = "Pending";
                }
            else
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if(u.RollId==1)
                {
                        if(rollid!="")
                        { 
                    users_Tbl.RollId = Convert.ToInt32(rollid);
                        }
                        else
                        {
                            users_Tbl.RollId = 2;
                        }
                        if(Status!="")
                        {
                        users_Tbl.Status = Status;
                        }
                        else
                        {
                            users_Tbl.Status = "Pending";
                        }
                        if(desid!="" && users_Tbl.RollId==1)
                        {
                            users_Tbl.DesignationId =Convert.ToInt32( desid);
                        }
                        else
                        {
                            users_Tbl.DesignationId = 2;
                        }
                    }
            }
                users_Tbl.SignUpDate = DateTime.Now.ToString();
                if(users_Tbl.Status=="Pending")
                {
                    users_Tbl.ApprovalDate = null;
                }
                else
                {

                    users_Tbl.ApprovalDate = DateTime.Now.ToString();
                }
            users_Tbl.AreaId = Convert.ToInt32(areaid);
            users_Tbl.Username = username;
            users_Tbl.Name = name;
            users_Tbl.CNIC = cnic;
            users_Tbl.Email = email;
            users_Tbl.Number = number;
                users_Tbl.Password = password;
       
                db.Users_Tbl.Add(users_Tbl);
                db.SaveChanges();
                MailMessage mm = new MailMessage("systemcovert@gmail.com", users_Tbl.Email);
                mm.Subject = "Covert Registration";
                mm.Body = "Dear " + users_Tbl.Name + ",\nThank You for registering on covert system. Your account has been created. You will shortly be approved. \n" + "Regards: COVERT ADMIN";
                mm.IsBodyHtml = false;



                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;


                NetworkCredential nc = new NetworkCredential("systemcovert@gmail.com", "adminofcovert");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = nc;
                smtp.Send(mm);
                if(u!=null)
                {
                    if(u.RollId==1)
                    {
                        ViewData["Error"] = "Successfully added";
                        return View("Index", db.Users_Tbl.Where(x=>x.Status=="Approved").ToList().ToPagedList( 1, 10));
                    }
                }

                return RedirectToAction("Index", "Home");

            }
        }

        // GET: Users_Tbl/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users_Tbl users_Tbl = db.Users_Tbl.Find(id);
            if (users_Tbl == null)
            {
                return HttpNotFound();
            }
            return View(users_Tbl);
        }

        // POST: Users_Tbl/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,RollId,Name,Status,Username,Email,Number,CNIC,Password,SignUpDate,ApprovalDate")] Users_Tbl users_Tbl)
        {
            if(users_Tbl.Password==null|| users_Tbl.Number==null|| users_Tbl.Name==null)
            {
                ViewData["ED"] = "Please fill all the fields";
                return View(users_Tbl);
            }
            if(users_Tbl.Password.Count()>12 || users_Tbl.Password.Count()<8)
            {
                ViewData["ED"] = "Password length should be minimum 8 characters and maximum 12 characters";
                return View(users_Tbl);

            }
            if(users_Tbl.Number.Count()>11 || users_Tbl.Number.Count()<11)
            {
                ViewData["ED"] = "Phone number should be of 11 characters and the format should be 03xxxxxxxxx";
                return View(users_Tbl);

            }
            if (ModelState.IsValid)
            {
                db.Entry(users_Tbl).State = EntityState.Modified;
                db.SaveChanges();
                ViewData["Error"] = "Successfully edited";
                return View("Index", db.Users_Tbl.Where(x=>x.Status=="Approved").ToList().ToPagedList( 1, 10));
            }
            return View(users_Tbl);
        }

        // GET: Users_Tbl/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users_Tbl users_Tbl = db.Users_Tbl.Find(id);
            if (users_Tbl == null)
            {
                return HttpNotFound();
            }
            return View(users_Tbl);
        }

        // POST: Users_Tbl/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int xx = 0;
            Users_Tbl users_Tbl = db.Users_Tbl.Find(id);

            try { 
            List<Posts_Tbl> pl = db.Posts_Tbl.Where(x => x.UserId == id).ToList();
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

                try
                {
                    List<Comment_Tbl> cl = db.Comment_Tbl.Where(x => x.UserId == id).ToList();
                    foreach (var comment in cl)
                    {
                        db.Comment_Tbl.Remove(comment);
                    }
                }
                catch(Exception)
                {
                    db.Users_Tbl.Remove(users_Tbl);
                    xx = 1;
                }
            }
            catch(Exception)
            {
                try
                {
                    List<Comment_Tbl> cl = db.Comment_Tbl.Where(x => x.UserId == id).ToList();
                    foreach (var comment in cl)
                    {
                        db.Comment_Tbl.Remove(comment);
                    }
                }
                catch (Exception)
                {
                    db.Users_Tbl.Remove(users_Tbl);
                    xx = 1;
                }

            }

           if(xx==0)
            {
                db.Users_Tbl.Remove(users_Tbl);
            }
            db.SaveChanges();
            ViewData["Error"] = "Successfully deleted";
            return View("Index", db.Users_Tbl.Where(x => x.Status=="Approved").ToList().ToPagedList(1, 10));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult approve (int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
             u = db.Users_Tbl.Find(id);
            if (u == null)
            {
                return HttpNotFound();
            }
            u.ApprovalDate = DateTime.Now.ToString();
            u.Status = "Approved";
            u.UserId = Convert.ToInt32(id);
            db.Entry(u).State= EntityState.Modified;
            db.SaveChanges();




            MailMessage mm = new MailMessage("systemcovert@gmail.com", u.Email);
            mm.Subject = "Covert Registration";
            mm.Body = "Dear "+ u.Name + ",\nThank You for registering on covert system. Your account has been approaved. Enjoy posting.\n"+"Regards: COVERT ADMIN";
            mm.IsBodyHtml = false;



            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;


            NetworkCredential nc = new NetworkCredential("systemcovert@gmail.com", "adminofcovert");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mm);






            return RedirectToAction("chalan");
        }
        public ActionResult chalan(/*int ? page*/)
        {
            if (Session["id"] != null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if (u.RollId == 1)
                {
                    ViewData["Error"] = "Successfully Approved";
                    return View(db.Users_Tbl.Where(x => x.Status == "pending" && (x.AreaId == u.AreaId || u.DesignationId == 1)).OrderByDescending(x => x.UserId).ToList()/*.ToPagedList(page ?? 1,10)*/);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
