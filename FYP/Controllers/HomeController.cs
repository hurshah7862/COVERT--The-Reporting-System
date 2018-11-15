using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FYP.Models;
using System.Data;
using System.Data.Entity;
using System.Net;
namespace FYP.Controllers
{
    public class HomeController : HandleSessionController
    {
        FYPBDEntities db = new FYPBDEntities();
        Users_Tbl u = new Users_Tbl();

        // GET: Home
        public ActionResult aboutus()
        {
            return View();
        }
        public ActionResult Index()
        {
            if(Session["id"]!=null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if(u.RollId==1)             //for admin
                {
                    return View();
                }
            else if(u.RollId==2)            //for users
                {
                    return View();
                }
            

            }
            return View();                  //for guests
        }
        public ActionResult pro(/*int ?id*/)
        {
            int ?id = Convert.ToInt32(Session["id"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            return RedirectToAction("Details", "Users_Tbl", new { id = id });
        }
    }
}