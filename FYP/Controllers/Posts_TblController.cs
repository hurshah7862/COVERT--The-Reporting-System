using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FYP.Models;
using System.Security.Cryptography;
using PagedList;
using PagedList.Mvc;

namespace FYP.Controllers
{
    public class Posts_TblController : HandleSessionController
    {
        private FYPBDEntities db = new FYPBDEntities();
        //MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        Users_Tbl u = new Users_Tbl();
        Posts_Tbl p = new Posts_Tbl();
        Category_Tbl c = new Category_Tbl();
        
        public ActionResult div(int? id, int pid)
        {
            if(id!=null && p!=null)
            {
            Visual_Tbl v = db.Visual_Tbl.Find(id);
                if(v!=null)
                {
                    p = db.Posts_Tbl.Find(pid);
                    db.Visual_Tbl.Remove(v);
                    db.SaveChanges();
                    ViewData["Error"] = "Successfully removed";
                    return View("Edit", p);
                }

                return RedirectToAction("Index");
            }
            return RedirectToAction("index");
        }
        public ActionResult pendingposts(int? page)
        {
            ViewData["Error"] = "";
            if (Session["id"] != null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if(u.RollId==1)
                {
                    return View(db.Posts_Tbl.OrderByDescending(x=>x.PostId).Where(x=>x.Status=="pending" &&(x.AreaId==u.AreaId || u.DesignationId==1)).ToList().ToPagedList(page ?? 1, 10));
                }
                return Redirect("index");
            }
            return Redirect("index");

        }
        // GET: Posts_Tbl
        [HttpGet]
        public ActionResult Index(string searchBy, string search, int? page)
        {
            //ViewBag.search = new SelectList(db.Category_Tbl, "categoryid", "Name");


            if (searchBy == "Category")
            {
                c = db.Category_Tbl.FirstOrDefault(x => x.Name == search);
                if (c != null)
                {

                    


                    return View(db.Posts_Tbl.OrderByDescending(x => x.PostId).Where(x => (x.Status != "Pending" && x.CategoryId == c.CategoryId) || (x.Status != "pending" && search == null)).ToList().ToPagedList(page  ?? 1, 10));

                }
                return View(db.Posts_Tbl.Where(x=>x.CategoryId==-1).ToList().ToPagedList(page ?? 1,10));
            }

            return View(db.Posts_Tbl.OrderByDescending(x => x.PostId).Where(x =>( x.PostName.Contains(search) && x.Status!="Pending") ||( search == null && x.Status!="Pending")).ToList().ToPagedList(page ?? 1, 10));
        }
                    
               

        // GET: Posts_Tbl/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Posts_Tbl posts_Tbl = db.Posts_Tbl.Find(id);
            if (posts_Tbl == null)
            {
                return HttpNotFound();
            }
            return View(posts_Tbl);
        }

        public ActionResult reportpost(int? id)
        {
            if(Session["id"]!=null && id!=null)
            { 
                Contactus_Tbl cc = new Contactus_Tbl();
                cc.PostId = id;
                cc.UserId = Convert.ToInt32(Session["id"]);
                u = db.Users_Tbl.Find(cc.UserId);
                cc.Email = u.Email;
                p = db.Posts_Tbl.Find(id);
                cc.Name = u.Name+"\t reported \t"+p.PostName ;
                cc.Message = "Post Reported:"+ cc.PostId.ToString();
                db.Contactus_Tbl.Add(cc);
                db.SaveChanges();
            }
            return View("Index", db.Posts_Tbl.Where(x => x.Status == "Approved").OrderByDescending(x => x.PostId).ToList().ToPagedList(1,10));
        }
        // GET: Posts_Tbl/Create
        public ActionResult Create()
        {
           
            if (Session["id"]!=null)
            {
                u = db.Users_Tbl.Find(Convert.ToInt32(Session["id"]));
                if(u.Status=="Approved")
                { 
                ViewBag.categoryid = new SelectList(db.Category_Tbl, "categoryid", "Name");
                    ViewBag.areaid = new SelectList(db.Area_Tbl, "Areaid", "AreaName");
                    return View();
                }
                return RedirectToAction("Index ", "Home");
            }
            return RedirectToAction("Index ", "Home");
        }

        // POST: Posts_Tbl/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Create(string categoryid, string name, string Description, string areaid)
        {
            Posts_Tbl posts_Tbl = new Posts_Tbl();
            if(categoryid=="")
            {
                ViewData["Error"] = "PLAESE SELECT A CATAGORY";
                ViewBag.categoryid = new SelectList(db.Category_Tbl, "categoryid", "Name");
                ViewBag.areaid = new SelectList(db.Area_Tbl, "Areaid", "AreaName");

                return View();
            }
            if (areaid == "")
            {
                ViewData["Error"] = "PLAESE SELECT AN AREA";
                ViewBag.categoryid = new SelectList(db.Category_Tbl, "categoryid", "Name");
                ViewBag.areaid = new SelectList(db.Area_Tbl, "Areaid", "AreaName");

                return View();
            }
            posts_Tbl.PostName = name;
            posts_Tbl.PostDescription = Description;
            posts_Tbl.CategoryId =Convert.ToInt32( categoryid);
            posts_Tbl.AreaId = Convert.ToInt32(areaid);
            posts_Tbl.UserId =Convert.ToInt32( Session["id"]);
            posts_Tbl.PostDate = DateTime.Now.ToString();
           
                if (string.IsNullOrEmpty(Request.Files[0].FileName))
            {
                List<string> match = new List<string>() { "Abuse", "abuse", "Accessory","accessory", "Accomplice", "accomplice", "Accused", "accused", "Accuser", "accuser",
                    "Activists","activists","Adversary","adversary","Affect","affect","AFIS","Against","against","Agency","agency","Aggravated","aggravated","assault","Assault",
                    "Alarm","alarm","Alcohol","alcohol","Alert","alert","Alias","alias","Alibi","alibi","Alienate","alienate","Allegation","allegation","Ammunition","ammunition",
                    "APB","Appeal","appeal","Armed","armed","Arraignment","arraignment","Arrest","arrest","Arsenal","arsenal","Arson","arson","Art forgery","Art Forgery","art forgery",
                    "Assailant","assailant","Attack","attack","Authority","authority","Autopsy","autopsy","Background check","Background Check","background check","check","Check",
                    "Backup","backup","Bail","bail","Ballistics","ballistics","Battery","battery","Beat","beat","Behavior","behavior","Behind bars","behind bars","Behind Bars",
                    "Belligerence","belligerence","Big house","Big House","big house","Blackmail","blackmail","Bloodstain","bloodstain","Blood","blood","Bombing","Bombing",
                    "Brawl","brawl","Breach","breach","Break-in","break-in","Broke-in","broke-in","broke-into","Broke-into","broke","Broke","Break","break","Breaking and entering",
                    "breaking and entering","Breaking and Entering","Bribery","bribery","Brutality","Bullying","bullying","brutality","Burden of proof","Burden of Proof","burden of proof",
                    "Burglary","burglary","Bystander","bystander","Capture","capture","case","Case","Caution","caution","Chase","chase","Cheat","cheat","Civil","civil","Claim","claim",
                    "Coercion","coercion","Collusion","collusion","Combat","combat","Commission","commission","Commit","commit","Complaint","complaint","Complication","complication",
                    "Conduct","conduct","Confession","confession","Connection","connection","Conspiracy","conspiracy","Contact","contact","Contempt","contempt","Control","control",
                    "Controversial","controversial","Conviction","conviction","Cops","cops","Coroner","coroner","Corruption","corruption","Counsel","counsel","Counterfeit","counterfeit",
                    "Credit theft","Credit Theft","credit theft","Crime","crime","Criminal","criminal","justice","Justice","Criminology","criminology","Cuffs","cuffs","Custody","custody",
                    "Damage","damage","Danger","danger","Dangerous","dangerous","Dark side","Dark Side","dark side","Deadly","deadly","Deal","deal","Dealings","dealings","Death","death",
                    "Deed","deed","Defendant","defendant","Defense","defense","Deliberate","deliberate","Delinquency","delinquency","Democratic","democratic","Denial","denial","Department",
                    "department","Deputy","deputy","Detail","detail","Detain","detain","Detection","detection","Detective","detective","Deter","deter","Determination","determination","Deviant",
                    "deviant","Direct","direct","Discovery","discovery","Dismember","dismember","Disobedience","disobedience","Disorderly","disorderly","Dispatch","dispatch","Disregard","disregard",
                    "Disruption","disruption","attorney","Attorney","DNA","domestic","Domestic","Document","document","Dossier","dossier","Drill","drill","Drugs","drugs","Duty","duty",
                    "Educate","educate","Education","education","Effect","effect","Elusive","elusive","Embezzle","Emergency","emergency","embezzle","Enable","enable","Encumber","encumber",
                    "Enforce","enforce","Entail","entail","Equality","equality","Escape","escape","Ethical","ethical","Evasive","evasive","Eviction","Evidence","evidence","eviction","Evil","evil",
                    "Examination","examination","Execute","execute","Exonerate","exonerate","Expert","expert","Explosives","explosives","Expunge","expunge","Extort","extort","Extradition",
                    "extradition","Extreme","extreme","Failure","failure","Fairness","fairness","Fatality","fatality","Fault","fault","Family","family","FBI","Federal","federal","Felony","felony",
                    "Ferocity","ferocity","Fight","fight","Fine","fine","Fingerprint","fingerprint","Bomb","bomb","First-degree","first-degree","First-Degree","Flee","flee","Footprints","footprints",
                    "Forbidden","forbidden","Force","force","Forensics","forensics","Forgery","forgery","Formal charge","formal charge","Formal Charge","Frantic","frantic","Fraud","fraud","Freedom",
                    "freedom","Full-scale","Full-Scale","full-scale","Fundamental","fundamental","Furtive","furtive","Good guys","good guys","Good Guys","Gory","gory","Government","government","Grief",
                    "grief","Grievance","grievance","Guarantee","guarantee","Guard","guard","Guilt","guilt","Gun","gun","Hand-to-hand","hand-to-hand","Hand-to-Hand","Handcuffs","handcuffs","Handle",
                    "handle","Harassment","harassment","Harassing","harassing","Harm","harm","Headquarters","headquarters","Heinous","heinous","Helicopter","helicopter","Help","help","rifle","Rifle",
                    "High-profile","high-profile","High-Profile","Hijack","hijack","Hire","hire","Holding cell","holding cell","Holding Cell","Holster","holster","Homicide","homicide","Honesty",
                    "honesty","Honor","honor","Hostage","hostage","Hot-line","hot-line","Hot-Line","Humanity","humanity","Identification","identification","Illegal","illegal","Immoral","immoral",
                    "Immunity","immunity","Impeach","impeach","Impression","impression","Prison","prison","Improper","improper","Incarceration","incarceration","Competent","competent",
                    "Incriminating","incriminating","Indictment","indictment","Influence","influence","Informant","informant","Information","information","Initiative","initiative","Injury","injury",
                    "Injure","injure","Inmate","inmate","Innocence","innocence","innocent","Innocent","Inquest","inquest","Instruct","instruct","Integrity","integrity","Intelligence","intelligence",
                    "Interests","interests","Interference","interference","national","National","Central","central","Fedral","fedral","Local","local","Interpol","interpol","interpret","Interpret",
                    "Interrogate","interrogate","Interstate","interstate","Intervention","intervention","Interview","interview","Intrastate","intrastate","Intruder","intruder","Invasive","invasive",
                    "Investigate","investigate","Investigation","investigation","Irregular","irregular","Firring","firring","Fire","fire","Irresponsible","irresponsible","Issue","issue","Jail","jail",
                    "Judge","judge","Judgment","judgment","Judicial","judicial","Judiciary","judiciary","Jurisdiction","jurisdiction","Jury","jury","Justice","justice","Juvenile","juvenile","Kidnap",
                    "kidnap","Kill","kill","Kin","kin","Laboratory","laboratory","Larceny","larceny","Law","law","Leaks","leaks","Lease","lease","legal","Legal","Legislation","legislation",
                    "Legislation","legislation","Legislature","legislature","Lethal","lethal","Libel","libel","Liberty","liberty","License","license","Lie","lie","Limits","limits","Long hours",
                    "Long Hours","long hours","Lowlife","lowlife","Loyal","loyal","Lynch","lynch","Mace","mace","Maintain","maintain","Major","major","Malice","malice","Mal","mal","Manacled",
                    "manacled","Manslaughter","manslaughter","Marshal","marshal","Mayhem","mayhem","detector","Detector","Detective","detective","Minor","minor","Miscreant","miscreant",
                    "Misdemeanor","misdemeanor","Missing","missing","Mission","mission","Model","model","Money laundering","money laundering","Money Laundering","Moratorium","moratorium",
                    "Motorist","motorist","Murder","murder","Negligent","negligent","Negotiable","negotiable","neglible","Neglible","Negotiate","negotiate","Neighborhood","neighborhood",
                    "Network","network","Taliban","taliban","Terrorist","terrorist","Notation","notation","Notification","notification","Nuisance","nuisance","Oath","oath","Obey","obey",
                    "Obligation","obligation","Offender","offender","Offense","offense","Officer","officer","Official","official","Case","case","Opinion","opinion","Opportunity","opportunity",
                    "Order","order","Organize","organize","Ownership","ownership","Partner","partner","Partnership","partnership","Pathology","pathology","Patrol","patrol","Pattern","pattern",
                    "Pedestrian","pedestrian","Peeping Tom","peeping tom","Peeping tom","Penalize","penalize","Penalty","penalty","Perjury","perjury","Perpetrator","perpetrator","Petition","petition",
                    "theft","Theft","Phony","phony","Plainclothes officer","plainclothes officer","Plainclothes Officer","Plea","plea","Police","police","Policy","policy","Power","power",
                    "Precedent","precedent","Precinct","precinct","Preliminary findings","preliminary findings","Preliminary Findings","Prevention","prevention","Principle","principle","Prior","prior",
                    "Prison","prison","Private","private","Probable cause","probable cause","Probable Cause","Probation","probation","Procedure","procedure","Professional","professional",
                    "Profile","profile","Prohibit","prohibit","Proof","proof","Property","property","Prosecute","prosecute","Prosecutor","prosecutor","Prostitution","prostitution","Protection",
                    "protection","Protocol","protocol","Provision","provision","Public","public","Punish","punish","Quake","quake","Qualification","qualification","Quality","quality",
                    "Quantify","quantify","Quantity","quantity","Quarrel","quarrel","Quell","quell","Question","question","Quickly","quickly","Quirk","quirk","Quiver","quiver","Radar","radar",
                    "Raid","raid","Rank","rank","Rap","rap","Reason","reason","Reckless","reckless","Record","record","Recovery","recovery","Recruit","recruit","Redress","redress","Reduction",
                    "reduction","Refute","refute","Register","register","Regulations","regulations","Reinforce","reinforce","Reject","reject","Release","release","Repeal","repeal",
                    "Report","report","Reprobate","reprobate","Reputation","reputation","Require","require","Resist","resist","Responsibility","responsibility","Restitution",
                    "restitution","Restrain","restrain","Restriction","restriction","Revenge","revenge","Rights","rights","Riot","riot","Robbery","robbery","Rogue","rogue","Rough","rough",
                    "Rules","rules","Rulings","rulings","Sabotage","sabotage","Safeguard","safeguard","Sanction","sanction","Scene","scene","Sealed","sealed","Search","search","Rescue","rescue",
                    "Secret","secret","Team","team","Seize","seize","Seizure","seizure","Selection","selection","Sentence","sentence","Sergeant","sergeant","Serial","serial","killer","Killer",
                    "Seriousness","seriousness","Services","services","Sex","sex","Crime","crime","Shackles","shackles","Sheriff","sheriff","Shoot","shoot","Shyster","shyster","Sighting",
                    "sighting","Situation","situation","Skillful","skillful","Slander","slander","Slash","slash","Slay","slay","Smuggl","smuggl","Sorrow","sorrow","Speculation","speculation",
                    "Spying","spying","Squad","squad","Stabbing","stabbing","Stalking","stalking","Statute","statute","Stigma","stigma","Limit","limit","Stipulation","stipulation","Subdue","subdue",
                    "Subpoena","subpoena","Successful","successful","Summons","summons","Supervise","supervise","Suppress","suppress","Surveillance","surveillance","Survivor","survivor",
                    "Suspect","suspect","Suspicion","suspicion","Suspicious","suspicious","Sworn","sworn","System","system","Tactic","tactic","Task","task","Force","force","Terrorism","terrorism",
                    "Test","test","Theft","theft","Threatening","threatening","Three-strikes law","three-strikes law","Three-Strikes Law","Thwart","thwart","Tire-slashing","Tire-Slashing","tire-slashing",
                    "Torture","torture","Toxic","toxic","Trace","trace","Traffic","traffic","Tragedy","tragedy","Transfer","transfer","Trauma","trauma","Treat","treat","Trespass","trespass","Trial","trial",
                    "Troop","troop","Trust","trust","Unacceptable","unacceptable","Unauthorized","unauthorized","Unclaimed","unclaimed","Unconstitutional","unconstitutional","Undercover","undercover",
                    "Underpaid","underpaid","Understaffed","understaffed","Unexpected","unexpected","Unharmed","unharmed","Uniform","uniform","Unintentional","unintentional","Unit","unit","Unjust","unjust",
                    "Unknown","unknown","Unlawful","unlawful","Unsolved","unsolved","Uphold","uphold","Vagrancy","vagrancy","Vandalism","vandalism","Viable","viable","Vice","vice","Victim","victim","Victory",
                    "victory","Vigilance","vigilance","Vigilante","vigilante","Violate","violate","Violation","violation","Violence","violence","Volunteer","volunteer","Vow","vow","Voyeurism","voyeurism",
                    "Vulnerable","vulnerable","Wanted","wanted","Poster","poster","Ward","ward","Warn","warn","Warped","warped","Warrant","warrant","Watch","watch","Weapon","weapon","Will","will","Wiretap",
                    "wiretap","Wisdom","wisdom","Witness","witness","Worse","worse","Wrong","wrong","youth","Youth","Zeal","zeal"};
                if (match.Any(posts_Tbl.PostName.Contains) && match.Any(posts_Tbl.PostDescription.Contains))
                {
                    posts_Tbl.Status = "Approved";
                    posts_Tbl.ApprovalDate = DateTime.Now.ToString();
                    db.Posts_Tbl.Add(posts_Tbl);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    posts_Tbl.Status = "Pending";
                    posts_Tbl.ApprovalDate = null;
                    db.Posts_Tbl.Add(posts_Tbl);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
             
            }
            u = db.Users_Tbl.Find(posts_Tbl.UserId);
                if(u.RollId!=1)
            {
                posts_Tbl.Status = "Pending";
                posts_Tbl.ApprovalDate = null;
            }
                else
            {
                posts_Tbl.Status = "Approved";
                posts_Tbl.ApprovalDate = DateTime.Now.ToString();
            }
            
           
            
            db.Posts_Tbl.Add(posts_Tbl);
            db.SaveChanges();

            var rn = Guid.NewGuid().ToString();
            //string dateobj = DateTime.Now.ToString();
            int counter = 0, x=-1;
            foreach (string fileName in Request.Files)
            {
                x++;
                HttpPostedFileBase file = Request.Files[x];
                if (!string.IsNullOrEmpty(file.FileName))
                {
                    string url = "~/Images/"+rn +(counter++).ToString()+file.FileName.Substring(file.FileName.LastIndexOf('.'));
                    string path = Server.MapPath(url);
                    file.SaveAs(path);
                    Visual_Tbl item = new Visual_Tbl();
                    item.Url = url;
                    item.PostId = posts_Tbl.PostId;
                    item.UserId = posts_Tbl.UserId;
                    db.Visual_Tbl.Add(item);
                    db.SaveChanges();
                }


            }
            return RedirectToAction("Index");
        }



        // GET: Posts_Tbl/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewData["Error"] = "";
            Session["pdiv"] = id;
            if (Session["id"] != null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                p = db.Posts_Tbl.Find(id);
                if (u.RollId == 1 ||( p.UserId == u.UserId && p.Status=="Pending"))
                {

                
                
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Posts_Tbl posts_Tbl = db.Posts_Tbl.Find(id);
            if (posts_Tbl == null)
            {
                return HttpNotFound();
            }
            return View(posts_Tbl);
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // POST: Posts_Tbl/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId,UserId,CategoryId,PostName,Status,PostDate,PostDescription,ApprovalDate,PostImage,PostVideo")] Posts_Tbl p)
        {

            if (Session["pdiv"] != null)
            {
                Posts_Tbl posts_Tbl = db.Posts_Tbl.Find(Session["pdiv"]);

                posts_Tbl.CategoryId = p.CategoryId;
                posts_Tbl.PostName = p.PostName;
                posts_Tbl.PostDescription = p.PostDescription;

                if (ModelState.IsValid)
                {
                    db.Entry(posts_Tbl).State = EntityState.Modified;
                    db.SaveChanges();

                    var rn = Guid.NewGuid().ToString();
                    //string dateobj = DateTime.Now.ToString();
                    int counter = 0, x = -1;
                    foreach (string fileName in Request.Files)
                    {
                        x++;
                        HttpPostedFileBase file = Request.Files[x];
                        if (!string.IsNullOrEmpty(file.FileName))
                        {
                            string url = "~/Images/" + rn + (counter++).ToString() + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                            string path = Server.MapPath(url);
                            file.SaveAs(path);
                            Visual_Tbl item = new Visual_Tbl();
                            item.Url = url;
                            item.PostId = posts_Tbl.PostId;
                            item.UserId = posts_Tbl.UserId;
                            db.Visual_Tbl.Add(item);
                            db.SaveChanges();

                        }


                    }
                    return RedirectToAction("Index");
                }
            }
            return View(p);
        }
            
        

        // GET: Posts_Tbl/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["id"] != null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                p = db.Posts_Tbl.Find(id);
                if (u.RollId == 1 || (p.UserId == u.UserId && p.Status == "Pending"))
                {

                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Posts_Tbl posts_Tbl = db.Posts_Tbl.Find(id);
                    if (posts_Tbl == null)
                    {
                        return HttpNotFound();
                    }
                    return View(posts_Tbl);
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // POST: Posts_Tbl/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Posts_Tbl posts_Tbl = db.Posts_Tbl.Find(id);
            try
            {
                List<Comment_Tbl> cl = db.Comment_Tbl.Where(x => x.PostId == id).ToList();
                foreach(var item in cl)
                {
                    db.Comment_Tbl.Remove(item);
                }
                try
                {
                    List<Visual_Tbl> vlist = db.Visual_Tbl.Where(x => x.PostId == id).ToList();
                    foreach (var item in vlist)
                    {
                        db.Visual_Tbl.Remove(item);
                    }
                    db.Posts_Tbl.Remove(posts_Tbl);


                    db.SaveChanges();


                }
                catch(Exception)
                {
                    db.Posts_Tbl.Remove(posts_Tbl);


                    db.SaveChanges();

                }
                
       
            }
            catch(Exception)
            {
                try
                {
                    List<Visual_Tbl> vlists = db.Visual_Tbl.Where(x => x.PostId == id).ToList();
                    foreach (var item in vlists)
                    {
                        db.Visual_Tbl.Remove(item);
                    }
                    db.Posts_Tbl.Remove(posts_Tbl);


                    db.SaveChanges();


                }
                catch (Exception)
                {
                    db.Posts_Tbl.Remove(posts_Tbl);


                    db.SaveChanges();

                }
                
                
            }
            ViewData["Error"] = "Successfully Deleted";
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
        public ActionResult comments(int? id)
        {
            return RedirectToAction("index", "comment_Tbl", new { id = id });
        }


        public ActionResult Edits(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Posts_Tbl posts_Tbl = db.Posts_Tbl.Find(id);
            if (posts_Tbl == null)
            {
                return HttpNotFound();
            }
            posts_Tbl.ApprovalDate = DateTime.Now.ToString();
            posts_Tbl.Status = "Approved";
            db.Entry(posts_Tbl).State = EntityState.Modified;
            db.SaveChanges();
            
            return RedirectToAction("chalan");
        }
        public ActionResult chalan(int? page)
        {
            ViewData["Error"] = "Successfully approved";
            if (Session["id"] != null)
            {
                u = db.Users_Tbl.Find(Session["id"]);
                if (u.RollId == 1)
                {
                    return View(db.Posts_Tbl.OrderByDescending(x => x.PostId).Where(x => x.Status == "pending" && (x.AreaId == u.AreaId || u.DesignationId == 1)).ToList().ToPagedList(page ?? 1, 10));
                }
                return Redirect("index");
            }
            return Redirect("index");

        }

    }
}
