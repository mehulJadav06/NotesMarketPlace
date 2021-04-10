using Notes_MarketPlace.Context;
using Notes_MarketPlace.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Notes_MarketPlace.Controllers
{
    public class AdminController : Controller   // Roll id  admin 1 , super admin 3
    {
        // GET: Admin
        readonly Entities3 dbObj = new Entities3();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult ManageAdmin()
        {
            int userID = (int)Session["UserID"];
            //int userID = 4;
            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID != 3)
            {
                return View("Login", "User");
            }
            List<User> users = dbObj.Users.Where(m => m.RoleID == 2).ToList();
            List<UserProfile> usersProfile = dbObj.UserProfiles.ToList();

            var admins = from u in users
                         join p in usersProfile on u.ID equals p.UserID into T1
                         from p in T1
                         select new Manager
                         {
                             user = u,
                             userProfile = p
                         };

            return View(admins);
        }
        [HttpGet]
        [Authorize]
        public ActionResult AddAdmin()
        {
            int userID = (int)Session["UserID"];
            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID != 3)
            {
                return View("Login", "User");
            }
            var countries = dbObj.Countries.ToList();
            AddAdmin admin = new AddAdmin
            {
                Countries = countries
            };
            return View(admin);
        }
        [Authorize]
        public ActionResult AddAdmin(string Id = "4")//for Edit Admin
        {
            int id = Convert.ToInt32(Id);
            User adminDetail = dbObj.Users.Where(m => m.ID == id).FirstOrDefault();
            UserProfile userProfile1 = dbObj.UserProfiles.Where(m => m.UserID == id).FirstOrDefault();

            int userID = (int)Session["UserID"];
            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID != 3)
            {
                return View("Login", "User");
            }
            var countries = dbObj.Countries.ToList();
            AddAdmin admin = new AddAdmin
            {
                FirstName = adminDetail.FirstName,
                LastName = adminDetail.LastName,
                EmailID = adminDetail.EmailID,
                CountryCode = userProfile1.CountryCode,
                MobileNo = userProfile1.MobileNumber,
                Countries = countries,
            };
            return View(admin);
        }
        [NonAction]
        public bool IsEmailExist(string email)
        {
            var v = dbObj.Users.Where(x => x.EmailID == email).FirstOrDefault();
            return v != null;
        }
        [Authorize]
        public ActionResult AddAdminPost(AddAdmin admin)
        {
            if (ModelState.IsValid)
            {
                User NewAdmin = new User
                {
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    EmailID = admin.EmailID,
                    RoleID = 1,
                    IsEmailVerified = false,
                    IsActive = true,
                    Password = "1234fornow"

                };
                if (IsEmailExist(admin.EmailID))
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return RedirectToAction("AddAdmin");
                }

                dbObj.Users.Add(NewAdmin);
                dbObj.SaveChanges();
                UserProfile userProfile = new UserProfile
                {
                    UserID = NewAdmin.ID,
                    MobileNumber = admin.MobileNo,
                    CountryCode = admin.CountryCode,
                    AddressLine1 = "TatvaSoft",
                    AdreessLine2 = "Iscon",
                    City = "Ahmedabad",
                    State = "Gujarat",
                    ZipCode = "034973",
                    Country = "India"
                };
                dbObj.UserProfiles.Add(userProfile);
                dbObj.SaveChanges();


                //Mailing Admin his/her password

                var v = dbObj.Users.Where(m => m.EmailID == admin.EmailID).FirstOrDefault();
                var fromEmail = new MailAddress("jadavmehul0615@gmail.com", "Mehul Admin");
                var toEmail = new MailAddress(admin.EmailID);
                var fromEmailPassword = "***";

                string subject = "You are Admin from this day!";

                string body = "<br><br>" +
                    "Hello, " + v.FirstName + " " + v.LastName + "<br><br>We have generated password for you,you can change it whenever you want.<br><br>" +
                    "Password : " + NewAdmin.Password +
                    "<br><br>Regards,<br>Notes Marketplace";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                };
                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                    smtp.Send(message);

                return RedirectToAction("ManageAdmin");
            }
            return RedirectToAction("AddAdmin");

        }
        [HttpGet]
        [Authorize]
        public ActionResult MyProfile()
        {
            //int userID = 23;
            int userID = (int)Session["UserID"];
            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID == 2)
            {
                return View("Login", "User");
            }
            List<Country> country = dbObj.Countries.ToList();
            UserProfile userProfile = dbObj.UserProfiles.Where(m => m.UserID == userID).FirstOrDefault();
            MyProfile myProfile = new MyProfile
            {
                FirstName=user.FirstName,
                LastName=user.LastName,
                EmailID=user.EmailID,
                EmailID2=userProfile.SecondaryEmailAddress,
                CountryCode=userProfile.CountryCode,
                MobileNo=userProfile.MobileNumber,
                ProfilePicture=userProfile.ProfilePicture,
                Countries = country
            };
            return View(myProfile);
        }
        public ActionResult MyProfilePost(MyProfile model, HttpPostedFileBase ProfilePicture)
        {
            //int userID = 4;
            int userID = (int)Session["UserID"];
            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID == 2)
            {
                return View("Login", "User");
            }
            UserProfile userProfile = dbObj.UserProfiles.Where(m => m.UserID == userID).FirstOrDefault();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.EmailID = model.EmailID;

            userProfile.SecondaryEmailAddress = model.EmailID2;
            userProfile.CountryCode = model.CountryCode;
            userProfile.MobileNumber = model.MobileNo;
            userProfile.ProfilePicture = model.ProfilePicture;

            string name = null;
            string path = null;
            if (ProfilePicture != null)
            {
                var profilePicture = Path.GetFileName(ProfilePicture.FileName);
                path = Path.Combine(Server.MapPath("~/UploadFiles"), profilePicture);
                ProfilePicture.SaveAs(path);
                name = Path.GetFileName(profilePicture);
            }
            dbObj.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        [Authorize]
        public ActionResult Dashboard()
        {
            int userID = (int)Session["UserID"];
            //int userID = 4;
            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
             if (user.RoleID == 2)
             {
                 return View("Login", "User");
             }
            //Statstics

            List<SellerNote> notes = dbObj.SellerNotes.Where(m => m.Status == 4).ToList();
            TempData["InReview"] = notes.Count;

            var dt = DateTime.Now.AddDays(-7);
            TempData["NoOfNotesDownloadedin7days"] = dbObj.Downloads.Where(m => m.AttachementDownloadedDate > dt).Count();
            TempData["NoOfNewRegistration"] = dbObj.Users.Where(m => m.RoleID == 1 && m.CreatedDate > dt).Count();

            //-- set from - will always be first day of current month
            DateTime dtFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            //-- set to - current date (with 00.00.00 time)
            DateTime dtTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var minDate = DateTime.Now.AddMonths(-1);
            List<SellerNote> sellerobj = dbObj.SellerNotes.Where(x => x.Status == 9 && (x.PublishedDate >= dtFrom && x.PublishedDate <= dtTo)).ToList();

            System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
            string sMonth = DateTime.Now.ToString("MM");
            int monthnumber = Convert.ToInt32(sMonth);
            ViewBag.MonthNumber = monthnumber;
            for (int i = 0; i < 6; i++)
            {
                //TempData["monthname"]= CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthnumber);
                string monthname = mfi.GetMonthName(monthnumber);
                TempData[Convert.ToString(monthnumber)] = monthname;

                monthnumber = monthnumber - 1;
                if (monthnumber == 0)
                {
                    monthnumber = 12;
                }
            }

            //Table
            List<SellerNote> note = dbObj.SellerNotes.Where(m => m.Status == 5).ToList();
            List<NoteCategory> categories = dbObj.NoteCategories.ToList();
            List<User> seller = dbObj.Users.ToList();

            var published = from s in note
                            join u in seller on s.SellerID equals u.ID into T1//publisher = seller (or admin confused)
                            from u in T1
                            join c in categories on s.Category equals c.ID into T2
                            from c in T2

                            select new PublishedNotes
                            {
                                note = s,
                                category = c,
                                seller = u
                            };
            foreach (SellerNote s in note)
            {
                List<Download> d = dbObj.Downloads.Where(m => m.NoteID == s.ID && m.IsSellerHasAllowedDownload == true && m.IsAttachmentDownloaded == true).ToList();
                string ids = Convert.ToString(s.ID);
                TempData[ids] = 0;

                    TempData[ids] = d.Count;
                    SellerNotesAttachement fileDetail = dbObj.SellerNotesAttachements.Where(m => m.NoteID == s.ID).FirstOrDefault();
                    if (fileDetail == null)
                    {
                        break;
                    }
                    var path = fileDetail.FilePath;
                    System.IO.FileInfo file = new System.IO.FileInfo(path);
                    string sizer = " B";
                    long fileSize = file.Length;
                    if (fileSize >= 1024 * 1024)
                    {
                        fileSize = fileSize / (1024 * 1024);
                        sizer = " MB";
                    }
                    else if (fileSize >= 1024)
                    {
                        fileSize /= 1024;
                        sizer = " KB";
                    }
                    TempData[s.ID.ToString() + "mb"] = fileSize.ToString() + sizer;
                

            }
            return View(published);
        }
        [HttpGet]
        [Authorize]
        public ActionResult Members()
        {
            int userID = (int)Session["UserID"];
            //int userID = 4;
            User admin = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (admin.RoleID == 2)
             {
                 return View("Login", "User");
             }
            List<User> users = dbObj.Users.ToList();

            Members members = new Members
            {
                users = users

            };

            foreach (User u in users)
            {
                List<Download> d = dbObj.Downloads.Where(m => m.Seller == u.ID && m.IsSellerHasAllowedDownload == true && m.IsAttachmentDownloaded == true).ToList();
                string id = Convert.ToString(u.ID) + " downloads";
                TempData[id] = d.Count;

                List<SellerNote> s = dbObj.SellerNotes.Where(m => m.SellerID == u.ID && m.Status == 5).ToList();
                id = Convert.ToString(u.ID) + " published";
                TempData[id] = s.Count;

                List<SellerNote> su = dbObj.SellerNotes.Where(m => m.SellerID == u.ID && m.Status == 4 && m.Status == 3).ToList();
                id = Convert.ToString(u.ID) + " inreview";
                TempData[id] = su.Count;

                List<Download> sold = dbObj.Downloads.Where(m => m.Seller == u.ID && m.IsSellerHasAllowedDownload == true && m.IsPaid == true).ToList();
                id = Convert.ToString(u.ID) + " sold";
                decimal money = 0;
                foreach (var i in sold)
                {
                    money += Convert.ToDecimal(i.PurchasedPrice);
                }
                TempData[id] = money;
                money = 0;

                List<Download> bought = dbObj.Downloads.Where(m => m.Downloader == u.ID && m.IsSellerHasAllowedDownload == true && m.IsPaid == true).ToList();
                id = Convert.ToString(u.ID) + " bought";
                foreach (var i in bought)
                {
                    money += Convert.ToDecimal(i.PurchasedPrice);
                }
                TempData[id] = money;
            }

            return View(members);
        }
        [Authorize]
        public ActionResult DeActiveUser(int id)
        {
            int userID = (int)Session["UserID"];
            
            User admin = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
             if (admin.RoleID == 2)
             {
                 return View("Login", "User");
             }

            User user = dbObj.Users.Where(m => m.ID == id).FirstOrDefault();
            user.IsActive = false;
            dbObj.SaveChanges();
            return RedirectToAction("Members");
        }
        [HttpGet]
        [Authorize]
        public ActionResult MemberDetails(int id=4)
        {
            int userID = (int)Session["UserID"];
            //int userID = 4;
            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID ==2)
             {
                 return View("Login", "User");
             }
            List<SellerNote> note = dbObj.SellerNotes.Where(m => m.SellerID == id).ToList();
            List<NoteCategory> categories = dbObj.NoteCategories.ToList();
            
            var detail = from s in note
                            join c in categories on s.Category equals c.ID into T1
                            from c in T1

                            select new MemberDetails
                            {
                                note = s,
                                category = c
                            };

            foreach (SellerNote s in note)
            {
                List<Download> d = dbObj.Downloads.Where(m => m.NoteID == s.ID && m.IsSellerHasAllowedDownload == true && m.IsAttachmentDownloaded == true).ToList();
                string ids = Convert.ToString(s.ID);
                TempData[ids] = d.Count;
                decimal money = 0;
                foreach(var i in d)
                {
                    if (i.IsPaid)
                    {
                        money += Convert.ToDecimal(i.PurchasedPrice);
                    }
                }
                TempData[ids + " earned"] = money;
            }
            Member member = new Member
            {
                user = dbObj.Users.Where(m => m.ID == id).FirstOrDefault(),
                userProfile = dbObj.UserProfiles.Where(m => m.UserID == id).FirstOrDefault(),
                member = detail
            };
            return View(member);
        }
        [HttpGet]
        [Authorize]
        public ActionResult NotesUnderReview()
        {
            int userID = (int)Session["UserID"];
            
            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID == 2)
            {
                return View("Login", "User");
            }
            List<SellerNote> note = dbObj.SellerNotes.Where(m=>m.Status==3 || m.Status==4).ToList();
            List<NoteCategory> categories = dbObj.NoteCategories.ToList();
            List<User> seller = dbObj.Users.ToList();

            List<SellerNote> sellerNotes = dbObj.SellerNotes.ToList();
            var notesUnderReview = from s in note
                                   join u in seller on s.SellerID equals u.ID into T1
                                   from u in T1
                                   join c in categories on s.Category equals c.ID into T2
                                   from c in T2
                                   select new NotesUnderReview
                                   {
                                       note = s,
                                       category = c,
                                       user=u
                                   };

            ViewBag.sellers = from u in seller
                              join s in sellerNotes on u.ID equals s.SellerID into t1
                              from s in t1
                              select new NotesUnderReview
                              {
                                  sellers = u,
                              };

            return View(notesUnderReview);
        }
        [Authorize]
        public ActionResult Approved(string Id)
        {
            int id = Convert.ToInt32(Id);
            var note = dbObj.SellerNotes.Where(m => m.ID == id).FirstOrDefault();
            note.Status = 5;
            note.PublishedDate = DateTime.Now;
            dbObj.SaveChanges();
            return RedirectToAction("NotesUnderReview");
        }
        [Authorize]
        public ActionResult Rejected(string Id)
        {
            int id = Convert.ToInt32(Id);
            var note = dbObj.SellerNotes.Where(m => m.ID == id).FirstOrDefault();
            note.Status = 6;
            dbObj.SaveChanges();
            return RedirectToAction("NotesUnderReview");
        }
        [Authorize]
        public ActionResult inReviewed(string Id)
        {
            int id = Convert.ToInt32(Id);
            var note = dbObj.SellerNotes.Where(m => m.ID == id).FirstOrDefault();
            note.Status = 4;
            dbObj.SaveChanges();
            return RedirectToAction("NotesUnderReview");
        }
        [HttpGet]
        [Authorize]
        public ActionResult PublishNotes()
        {
            int userID = (int)Session["UserID"];
            //int userID = 4;
            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
             if (user.RoleID ==2)
             {
                 return View("Login", "User");
             }
            List<SellerNote> note = dbObj.SellerNotes.Where(m => m.Status == 5).ToList();
            List<NoteCategory> categories = dbObj.NoteCategories.ToList();
            List<User> seller = dbObj.Users.ToList();

            var published = from s in note
                                     join u in seller on s.SellerID equals u.ID into T1
                                     from u in T1
                                     join a in seller on s.ActionedBy equals a.ID into T2
                                     from a in T2
                                     join c in categories on s.Category equals c.ID into T3
                                     from c in T3
                                     
                                     select new PublishedNotes
                                     {
                                         note = s,
                                         category = c,
                                         seller = u,
                                         approvedBy=a
                                     };
            foreach(SellerNote s in note)
            {
                List<Download> d = dbObj.Downloads.Where(m=>m.NoteID == s.ID && m.IsSellerHasAllowedDownload==true && m.IsAttachmentDownloaded==true).ToList();
                string ids = Convert.ToString(s.ID);
                TempData[ids] = d.Count;
            }

            ViewBag.sellers = from u in seller 
                              join s in note on u.ID equals s.SellerID into t1
                              from s in t1
                              select new NotesUnderReview
                              {
                                  sellers = u,
                              };

            return View(published);
        }
        [Authorize]
        public ActionResult UnPublish(int Id)
        {
            int userID = (int)Session["UserID"];
            string remarks = "add reamrks here";//add some remarks in html and pass here
            
            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID ==2)
             {
                 return View("Login", "User");
             }
            SellerNote note = dbObj.SellerNotes.Where(m => m.ID == Id).FirstOrDefault();
            dbObj.Configuration.ValidateOnSaveEnabled = false;
            note.Status = 8;
            dbObj.SaveChanges();


            //Mailto the seller that your note is removed
            var seller = dbObj.Users.Where(m => m.ID == note.SellerID).FirstOrDefault();
            var fromEmail = new MailAddress("jadavmehul0615@gmail.com", "Mehul Admin");
            var toEmail = new MailAddress(seller.EmailID);
            var fromEmailPassword = "15061996";

            string subject = "Sorry! We need to remove your notes from our portal.";

            string body = "<br><br>" +
                "Hello, " + seller.FirstName + " " + seller.LastName + "<br><br>we want to inform you that, your note " +note.Title +
                " > has been removed from the portal.<br> Please find our remarks as below -" + remarks +
                "<br><br>Regards,<br>Notes Marketplace";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            smtp.Send(message);
            return RedirectToAction("PublishNotes");
        }
        [HttpGet]
        [Authorize]
        public ActionResult RejectedNotes()
        {
             int userID = (int)Session["UserID"];
             User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
             if (user.RoleID ==2)
             {
                 return View("Login", "User");
             }

            List<SellerNote> note = dbObj.SellerNotes.Where(m => m.Status == 6).ToList();
            List<NoteCategory> categories = dbObj.NoteCategories.ToList();
            List<User> sellerAdmin = dbObj.Users.ToList();
            

            var Rejected = from s in note
                            join u in sellerAdmin on s.SellerID equals u.ID into T1
                            from u in T1
                            join a in sellerAdmin on s.ActionedBy equals a.ID into T2
                            from a in T2
                            join c in categories on s.Category equals c.ID into T3
                            from c in T3

                            select new RejectedNotesAdmin
                            {
                                note = s,
                                category = c,
                                seller = u,
                                RejectedBy = a
                            };

            ViewBag.sellers = from u in sellerAdmin
                              join s in note on u.ID equals s.SellerID into t1
                              from s in t1
                              select new NotesUnderReview
                              {
                                  sellers = u
                              };
            return View(Rejected);
        }
        [HttpGet]
        [Authorize]
        public ActionResult DownloadNotes()
        {
            int userID = (int)Session["UserID"];

            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
             if (user.RoleID ==2)
             {
                 return View("Login", "User");
             }

            List<Download> downloads = dbObj.Downloads.ToList();
            List<User> sellerBuyer = dbObj.Users.ToList();

            var download = from d in downloads
                           join s in sellerBuyer on d.Seller equals s.ID into T1
                           from s in T1
                           join b in sellerBuyer on d.Downloader equals b.ID into T2
                           from b in T2

                           select new Downloads
                           {
                               download=d,
                               seller=s,
                               Buyer=b
                           };

            return View(download);
        }
        [HttpGet]
        [Authorize]
        public ActionResult SpamReports()
        {
            int userID = (int)Session["UserID"];

            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
             if (user.RoleID ==2)
             {
                 return View("Login", "User");
             }

            List<SellerNote> note = dbObj.SellerNotes.ToList();
            List<NoteCategory> categories = dbObj.NoteCategories.ToList();
            List<User> reporter = dbObj.Users.ToList();
            List<SellerNotesReportedIssue> reports = dbObj.SellerNotesReportedIssues.ToList();

            var spams = from r in reports
                           join s in note on r.NoteID equals s.ID into T1
                           from s in T1
                           join u in reporter on s.SellerID equals u.ID into T2
                           from u in T2
                           join c in categories on s.Category equals c.ID into T3
                           from c in T3

                           select new SpamReports
                           {
                               note = s,
                               category = c,
                               ReportedBy= u,
                               reports=r
                           };

            return View(spams);

        }
        [Authorize]
        public ActionResult DeleteReport(string Id)
        {
            int id = Convert.ToInt32(Id);
            
            SellerNotesReportedIssue report = dbObj.SellerNotesReportedIssues.Where(m => m.ID == id).FirstOrDefault();
            dbObj.SellerNotesReportedIssues.Remove(report);
            dbObj.SaveChanges();
            return RedirectToAction("SpamReports");
        }
        [HttpGet]
        [Authorize]
        public ActionResult ManageCategory()
        {
            int userID = (int)Session["UserID"];

            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID ==2)
             {
                 return View("Login", "User");
             }
            List<NoteCategory> categories = dbObj.NoteCategories.ToList();
            List<User> admin = dbObj.Users.ToList();

            var manageCategories = from c in categories
                           join a in admin on c.CreatedBy equals a.ID into T1
                           from a in T1

                           select new ManageCategory
                           {
                               admin=a,
                               category=c
                           };

            return View(manageCategories);
        }
        public ActionResult DeleteCategory(string Id)
        {
            int id = Convert.ToInt32(Id);
            NoteCategory category = dbObj.NoteCategories.Where(m => m.ID == id).FirstOrDefault();
            if (category.IsActive)
            {
                category.IsActive = false;
            }
            else
            {
                category.IsActive = true;
            }
            dbObj.SaveChanges();
            return RedirectToAction("ManageCategory");
        }
        [HttpGet]
        [Authorize]
        public ActionResult ManageType()
        {
            int userID = (int)Session["UserID"];

            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID ==2)
             {
                 return View("Login", "User");
             }
            List<NoteType> types = dbObj.NoteTypes.ToList();
            List<User> admin = dbObj.Users.ToList();

            var manageTypes = from t in types
                                   join a in admin on t.CreatedBy equals a.ID into T1
                                   from a in T1

                                   select new ManageType
                                   {
                                       admin = a,
                                       type = t
                                   };

            return View(manageTypes);
        }

        [Authorize]
        public ActionResult DeleteTypes(string Id)
        {
            int id = Convert.ToInt32(Id);
            NoteType type = dbObj.NoteTypes.Where(m => m.ID == id).FirstOrDefault();
            if (type.IsActive)
            {
                type.IsActive = false;
            }
            else
            {
                type.IsActive = true;
            }
            dbObj.SaveChanges();
            return RedirectToAction("ManageType");
        }
        [HttpGet]
        [Authorize]
        public ActionResult ManageCountry()
        {
            int userID = (int)Session["UserID"];

            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
             if (user.RoleID ==2)
             {
                 return View("Login", "User");
             }
            List<Country> countries = dbObj.Countries.ToList();
            List<User> admin = dbObj.Users.ToList();

            var manageCoutries = from c in countries
                                   join a in admin on c.CreatedBy equals a.ID into T1
                                   from a in T1

                                   select new ManageCountry
                                   {
                                       admin = a,
                                       country = c
                                   };

            return View(manageCoutries);
        }
        [Authorize]
        public ActionResult DeleteCountry(string Id)
        {
            int id = Convert.ToInt32(Id);
            Country country = dbObj.Countries.Where(m => m.ID == id).FirstOrDefault();
            if (country.IsActive)
            {
                country.IsActive = false;
            }
            else
            {
                country.IsActive = true;
            }
            dbObj.SaveChanges();
            return RedirectToAction("ManageCountry");
        }
        [HttpGet]
        [Authorize]
        public ActionResult AddType()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult AddTypePost(AddType type)
        {
            int userID = (int)Session["UserID"];

            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID ==2)
             {
                 return View("Login", "User");
             }
            NoteType noteType = new NoteType
            {
                Description=type.Description,
                Name=type.Name,
                CreatedBy=user.ID,
                CreatedDate=DateTime.Now,
                IsActive=true
            };
            dbObj.NoteTypes.Add(noteType);
            dbObj.SaveChanges();
            return View("AddType");
        }
        [HttpGet]
        [Authorize]
        public ActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult AddCategoryPost(AddCategory category)
        {
            int userID = (int)Session["UserID"];

            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID == 2)
            {
                return View("Login", "User");
            }
            NoteCategory category1 = new NoteCategory
            {
                Description = category.Description,
                Name = category.Name,
                CreatedBy = user.ID,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            dbObj.NoteCategories.Add(category1);
            dbObj.SaveChanges();
            return View("AddCategory");
        }
        [HttpGet]
        [Authorize]
        public ActionResult AddCountry()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult AddCountryPost(AddCountry country)
        {
            int userID = (int)Session["UserID"];

            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID == 2)
            {
                return View("Login", "User");
            }
            Country country1 = new Country
            {
                CountryCode = country.CountryCode,
                Name = country.Name,
                CreatedBy = user.ID,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            dbObj.Countries.Add(country1);
            dbObj.SaveChanges();
            return View("AddCountry");
        }
        [HttpGet]
        [Authorize]
        public ActionResult NotesDetails(string Id)
        {
            int userID = (int)Session["UserID"];

            User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            if (user.RoleID == 2)
            {
                return View("Login", "User");
            }

            int id = Convert.ToInt32(Id);
            NotesDetail note = new NotesDetail();

            SellerNote sellerNote = dbObj.SellerNotes.Where(m => m.ID == id && m.IsActive == true && m.Status == 5).FirstOrDefault();
            if (sellerNote == null)
            {
                return View("SearchNotes");
            }
            note.ID = sellerNote.ID;
            note.sellerID = sellerNote.SellerID;
            note.sellerNote = sellerNote;
            var category = dbObj.NoteCategories.Where(m => m.ID == sellerNote.Category).FirstOrDefault();
            note.category = category.Name;
            var country = dbObj.Countries.Where(m => m.ID == sellerNote.Country).FirstOrDefault();
            note.country = country.Name;

            if (Session["UserID"] != null)
            {
                userID = (int)Session["UserID"];
                user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
                note.userName = user.FirstName;
                User user1 = dbObj.Users.Where(m => m.ID == sellerNote.SellerID).FirstOrDefault();
                note.sellerName = user1.FirstName + " " + user1.LastName;
            }

            var reviews = dbObj.SellerNotesReviews.Where(m => m.NoteID == sellerNote.ID && m.IsActive==true).ToList();
            List<User> users = dbObj.Users.ToList();
            List<Context.UserProfile> usersProfile = dbObj.UserProfiles.ToList();

            IEnumerable<reviews> customerReview = from r in reviews
                                                  join u in users on r.ReviewedByID equals u.ID into T1
                                                  from u in T1
                                                  join p in usersProfile on u.ID equals p.UserID into T2
                                                  from p in T2
                                                  select new reviews
                                                  {
                                                      review = r,
                                                      userProfiles = p,
                                                      users = u
                                                  };

            note.review = customerReview;

            int count = 0;
            if (customerReview.Count() != 0)
            {
                decimal rating = 0;
                note.count = count;
                foreach (var review1 in customerReview)
                {
                    rating = rating + review1.review.Ratings;
                    count += 1;
                }
                decimal rate = Math.Round(rating * 2 / count);
                note.rate = rate;
            }

            note.count = count;
            var Inappropriate = dbObj.SellerNotesReportedIssues.Where(m => m.NoteID == sellerNote.ID).ToList();
            note.userReport = Inappropriate.Count;

            TempData["note"] = note;
            return View(note);
        }
        [Authorize]
        public ActionResult NoteReviewDelete(int Id)
        {
            SellerNotesReview review = dbObj.SellerNotesReviews.Where(m => m.ID == Id).FirstOrDefault();
            review.IsActive = false;
            dbObj.Configuration.ValidateOnSaveEnabled = false;
            dbObj.SaveChanges();
            dbObj.Configuration.ValidateOnSaveEnabled = true;
            SellerNote note = dbObj.SellerNotes.Where(m => m.ID == review.NoteID).FirstOrDefault();
            return RedirectToAction("NotesDetails", "Admin", new { Id = note.ID.ToString() });
        }
    }
}