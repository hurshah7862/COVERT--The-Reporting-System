using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FYP.Models;

namespace FYP.Controllers
{
    public class HandleSessionController : Controller
    {

        public Users_Tbl ux
        {
            set
            {
                Session["id"] = value;
            }
            get
            {
                Users_Tbl user = new Users_Tbl();
                user=Session["id"] as Users_Tbl;
                return  user;
            }
        }

        public int activeuserlocid
        {
            get
            {
                int id = 0;
                Users_Tbl u = ux;
                if(u!=null)
                {
                    id = u.UserId;
                }
                return id;
            }
        }
        
    }
}