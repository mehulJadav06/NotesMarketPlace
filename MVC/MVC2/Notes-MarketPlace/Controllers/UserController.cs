using Microsoft.AspNet.Identity;
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
using System.Web.Security;


namespace Notes_MarketPlace.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        readonly Entities3 dbObj = new Entities3();

        public int Port { get; private set; }
        public bool EnableSsl { get; private set; }

        public ActionResult Index()
        {
            return View();
        }

        //Sign up 
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }
        /*public JsonResult IsEmailExists(String email)
        {
            return Json(dbObj.Users.Any(m => m.EmailID == email), JsonRequestBehavior.AllowGet);
        }
        */
        [NonAction]
        public bool IsEmailExist(string email)
        {
            var v = dbObj.Users.Where(x => x.EmailID == email).FirstOrDefault();
            return v != null;
        }
        [HttpPost]
        public ActionResult AddUser(User model)
        {
            bool status = false;
            if (ModelState.IsValid)
            {

                User user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailID = model.EmailID,
                    Password = model.Password,
                    ConformPassword = model.ConformPassword
                };

                if (IsEmailExist(user.EmailID))
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View("SignUp");
                }
                user.RoleID = 2;
                user.IsEmailVerified = false;
                user.IsActive = true;

                dbObj.Users.Add(user);
                dbObj.SaveChanges();

                status = true;
                //var activationCode = Guid.NewGuid();
                SendLinkForEmailVerification(user.EmailID, user.FirstName, user.LastName);
                return View("SignUp");
            }
            return View("SignUp");

        }

        //Email verification
        [NonAction]
        public void SendLinkForEmailVerification(string email, string firstName, string lastName)
        {
            var verifyURL = "User/EmailVerification/" + email;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyURL);

            var fromEmail = new MailAddress("jadavmehul0615@gmail.com", "Mehul Admin");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "***";

            string subject = "Your Account has been Succesfully Created!";

            string body = "<br><br>" +
                "Hello, " + firstName + " " + lastName + "<br><br>Thank you for signing up with us." +
                "Please click on below link to verify your email address and to do login.<br><br>" +
                "<a href=" + link + ">Verify Email Page</a>" +
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
        }
        [HttpGet]
        public ActionResult EmailVerification(string id)
        {
            bool status = false;

            dbObj.Configuration.ValidateOnSaveEnabled = false;// confirm password doesn't match issue.

            var v = dbObj.Users.Where(x => x.EmailID == id).FirstOrDefault();

            if (v != null)
            {
                v.IsEmailVerified = true;
                dbObj.SaveChanges();
                status = true;
                return View("Login");
            }
            else
            {
                ViewBag.Message = "Invalid Email Verification Request";
            }
            ViewBag.Status = true;
            return View("SignUp");
        }

        //Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin model, string ReturnUrl)
        {
            string message = "";
            var v = dbObj.Users.Where(m => m.EmailID == model.EmailID).FirstOrDefault();
            if (v != null)
            {
                if (string.Compare(model.Password, v.Password) == 0)
                {
                    if (v.IsEmailVerified)
                    {
                        int timeout = model.RememberMe ? 525600 : 20; // 525600 is year in minutes
                        var ticket = new FormsAuthenticationTicket(model.EmailID, model.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
                        {
                            Expires = DateTime.Now.AddMinutes(timeout),
                            HttpOnly = true
                        };
                        Response.Cookies.Add(cookie);
                        Session["UserID"] = v.ID;
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Home", "User");
                        }
                    }
                    else
                    {
                        string newPassword = "";
                        SendLinkForEmailVerification(v.EmailID, v.FirstName, v.LastName);
                        return View("Login");
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        ModelState.AddModelError("PasswordIncorrect", "The Password that you have entered is incorrect");
                    }
                    return View("Login");
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    ModelState.AddModelError("EmailExist", "Email Does not exist");
                }
                return View("Login");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ChangePassword model)
        {
            int userid = (int)Session["UserID"];
            User user = dbObj.Users.Where(m=>m.ID==userid).FirstOrDefault();
            user.Password = model.NewPassword;
            dbObj.Configuration.ValidateOnSaveEnabled = false;
            dbObj.SaveChanges();
            dbObj.Configuration.ValidateOnSaveEnabled = true;
            return View("Login");
        }
        public ActionResult Home()
        {
            return View();
        }

        // Logout
        [Authorize]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "User");
        }

        [NonAction]
        public void SendLinkForNewPassword(string email, string newPassword)
        {
            var v = dbObj.Users.Where(m => m.EmailID == email).FirstOrDefault();
            var fromEmail = new MailAddress("jadavmehul0615@gmail.com", "Mehul Admin");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "***";

            string subject = "Your Password has been Succesfully Updated!";

            string body = "<br><br>" +
                "Hello, " + v.FirstName + " " + v.LastName + "<br><br>We have generated new password for you<br><br>" +
                "Password : " + newPassword +
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
        }

        [NonAction]
        public string PasswordCreater()
        {
            string Password = string.Empty;
            int i = 0;
            Random random = new Random();
            while (i < 4)
            {
                Password += Convert.ToChar(random.Next(97, 123));
                i += 1;
            }
            Password += Convert.ToChar(random.Next(65, 91)) + ((char)random.Next(33, 48)).ToString() + ((char)random.Next(48, 58)).ToString() + ((char)random.Next(48, 58)).ToString();
            return Password;
        }

        [HttpGet]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(UserLogin model)
        {
            var v = dbObj.Users.Where(m => m.EmailID == model.EmailID).FirstOrDefault();

            if (v != null)
            {
                if (v.IsEmailVerified)
                {
                    string newPassword = PasswordCreater();
                    v.Password = newPassword;
                    dbObj.Configuration.ValidateOnSaveEnabled = false;
                    dbObj.SaveChanges();
                    SendLinkForNewPassword(v.EmailID, newPassword);
                    return View("Login");
                }
                else
                {
                    ModelState.AddModelError("EmailExist", "Verify your email by clicking link given in your gmail");
                    SendLinkForEmailVerification(v.EmailID, v.FirstName, v.LastName);
                    return View("ForgetPassword");
                }
            }
            else
            {
                ModelState.AddModelError("EmailExist", "Email Does not exist");
                return View("ForgetPassword");
            }

            return View("ForgetPassword");
        }
        //Add Notes
        [HttpGet]
        public ActionResult AddNotes()
        {
            List<NoteType> noteTypes = dbObj.NoteTypes.ToList();
            List<NoteCategory> noteCategories = dbObj.NoteCategories.ToList();
            List<Country> country = dbObj.Countries.ToList();

            AddNotes model = new AddNotes
            {
                types = noteTypes,
                categories = noteCategories,
                countries = country
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNotes(AddNotes model, HttpPostedFileBase DisplayPicture, HttpPostedFileBase NotesPreview, HttpPostedFileBase UploadNotes)
        {
            var userID = User.Identity.GetUserId();
            string name = null;
            string preview_img_name = null, notes_pdf_path = null;
            string upload_note_name = null, path = null, preview_path = null;
            //Store DisplayPicture in database
            if (DisplayPicture != null)
            {
                var displayPicture = Path.GetFileName(DisplayPicture.FileName);
                path = Path.Combine(Server.MapPath("~/UploadFiles"), displayPicture);
                DisplayPicture.SaveAs(path);
                name = Path.GetFileName(displayPicture);
            }
            //Store preview picture in database
            if (NotesPreview != null)
            {
                var notesPreview = Path.GetFileName(NotesPreview.FileName);
                preview_path = Path.Combine(Server.MapPath("~/UploadFiles"), notesPreview);
                NotesPreview.SaveAs(preview_path);
                preview_img_name = Path.GetFileName(notesPreview);
            }

            //Store Notes(pdf_form) in database
            if (UploadNotes != null)
            {
                var uploadNotes = Path.GetFileName(UploadNotes.FileName);
                notes_pdf_path = Path.Combine(Server.MapPath("~/UploadFiles"), uploadNotes);
                UploadNotes.SaveAs(notes_pdf_path);
                upload_note_name = Path.GetFileName(uploadNotes);
            }

            if (ModelState.IsValid)
            {
                if (model.IsPaid)
                {
                    model.SellingPrice = 0;
                }
                else
                {
                    if (model.NotesPreview == null)
                    {
                        ModelState.AddModelError("notepreview", "Paid Notes Required Note Preview");
                    }
                }
                var sellerNote = new SellerNote
                {
                    Title = model.Title,
                    Category = model.CategoryID,
                    NoteType = model.NoteTypeID,
                    Status = 2,
                    DisplayPicture = name,
                    NumberOfPages = model.NumberOfPages,
                    Description = model.Description,
                    UniversityName = model.UniversityName,
                    Country = model.CountryID,
                    Course = model.Course,
                    CourseCode = model.CourseCode,
                    Professor = model.Professor,
                    IsPaid = model.IsPaid,
                    SellingPrice = model.SellingPrice,
                    CreatedDate = DateTime.Now,
                    NotesPreview = preview_img_name,
                    SellerID = 4 //it should be userID 
                };
                var sellerNotesAttachment = new SellerNotesAttachement
                {
                    FileName = upload_note_name,
                    FilePath = notes_pdf_path,
                    CreatedDate = DateTime.Now
                };

                dbObj.SellerNotes.Add(sellerNote);
                dbObj.SellerNotesAttachements.Add(sellerNotesAttachment);
                dbObj.SaveChanges();
            }
            else
            {

                return RedirectToAction("AddNotes");
            }

            return RedirectToAction("AddNotes");
        }
        //Contact us

        [NonAction]
        public void SendEmailForContactUs(string email, string fullname, string subject, string question)
        {
            var fromEmail = new MailAddress("jadavmehul0615@gmail.com", "Mehul Admin");
            var toEmail = new MailAddress("jadavmehul0610@gmail.com");
            var fromEmailPassword = "***";

            //string subject = subject;

            string body = "<br><br>" +
                "Hello, " + "<br><br>" + question +
                "<br><br>Regards,<br>" + fullname + "<br>" + email;

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
        }
        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ContactUs(ContactUs model)
        {
            SendEmailForContactUs(model.EmailID, model.FullName, model.Subject, model.Question);
            return View("Login");
        }

        //FAQ
        [HttpGet]
        public ActionResult FAQ()
        {
            return View();
        }

        //Search Notes
        [HttpGet]
        public ActionResult SearchNotes()
        {
            var sellerNote = dbObj.SellerNotes.ToList();
            return View(sellerNote);
        }

        public JsonResult SearchNotesByText(string searchText)
        {
            dbObj.Configuration.ProxyCreationEnabled = false;
            List<SellerNote> sellerNotes = dbObj.SellerNotes.Where(m => m.Title.Contains(searchText)).ToList();
            dbObj.Configuration.ProxyCreationEnabled = true;
            return Json(sellerNotes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult NotesDetails(int id = 11)
        {
            NotesDetail note = new NotesDetail();

            
            SellerNote sellerNote = dbObj.SellerNotes.Where(m => m.ID == id).FirstOrDefault();// id should be passed in search notes
            note.ID = sellerNote.ID;
            note.sellerID = sellerNote.SellerID;
            note.sellerNote = sellerNote;
            var category = dbObj.NoteCategories.Where(m => m.ID == sellerNote.Category).FirstOrDefault();
            note.category = category.Name;
            var country = dbObj.Countries.Where(m => m.ID == sellerNote.Country).FirstOrDefault();
            note.country = country.Name;

            if (Session["UserID"] != null)
            {
                int userID = (int)Session["UserID"];
                User user = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
                note.userName = user.FirstName;
                user= dbObj.Users.Where(m => m.ID == sellerNote.ID).FirstOrDefault();
                note.sellerName = user.FirstName + " " + user.LastName;
            }

            var reviews = dbObj.SellerNotesReviews.Where(m => m.NoteID == sellerNote.ID).ToList();
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
            if(customerReview.Count() !=0)
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

        [HttpPost]
        [Authorize]
        public ActionResult FreeNote(int id)
        {
            int userID = (int)Session["UserID"];

            SellerNote sellerNote = dbObj.SellerNotes.Where(m => m.ID == id).FirstOrDefault();
            var category = dbObj.NoteCategories.Where(m => m.ID == sellerNote.Category).FirstOrDefault();
            var file = dbObj.SellerNotesAttachements.Where(m => m.NoteID == id).FirstOrDefault();
            Download download = new Download
            {
                NoteID = id,
                Downloader = userID,
                Seller = sellerNote.SellerID,
                IsSellerHasAllowedDownload = true,
                AttachmentPath = file.FilePath,
                IsAttachmentDownloaded = true,
                AttachementDownloadedDate = DateTime.Now,
                IsPaid = false,
                PurchasedPrice = null,
                NoteTitle = sellerNote.Title,
                NoteCategory = category.Name,
                CreatedBy = userID,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ModifiedBy = userID
            };
            dbObj.Downloads.Add(download);
            dbObj.SaveChanges();

            SellerNotesAttachement notesAttachement = dbObj.SellerNotesAttachements.Where(x => x.NoteID == id).FirstOrDefault();
            string path = Server.MapPath("~/UploadFiles/") + notesAttachement.FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", notesAttachement.FileName);
        }
        [Authorize]
        [HttpPost]
        public ActionResult NotesDetail(string Id)
        {
            int id = int.Parse(Id);
            SellerNote sellerNote = dbObj.SellerNotes.Where(m => m.ID == id).FirstOrDefault();
            Download d = dbObj.Downloads.Where(m => m.Downloader == id).Where(m => m.NoteID == sellerNote.ID).FirstOrDefault();
            if (d != null)
            {
                return Json(new EmptyResult());
            }
            var category = dbObj.NoteCategories.Where(m => m.ID == sellerNote.Category).FirstOrDefault();
            int userID = (int)Session["UserID"];
            Download download = new Download
            {
                NoteID = id,
                Downloader = userID,
                Seller = sellerNote.SellerID,
                IsSellerHasAllowedDownload = false,
                AttachmentPath = null,
                IsAttachmentDownloaded = false,
                AttachementDownloadedDate = DateTime.Now,
                IsPaid = true,
                PurchasedPrice = sellerNote.SellingPrice,
                NoteTitle = sellerNote.Title,
                NoteCategory = category.Name,
                CreatedBy = userID,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ModifiedBy = userID
            };
            dbObj.Downloads.Add(download);
            dbObj.SaveChanges();

            //mail to seller
            
            var sellerDetail = dbObj.Users.Where(m => m.ID == sellerNote.SellerID).FirstOrDefault();
            var user1 = dbObj.Users.Where(m => m.ID == userID).FirstOrDefault();
            var email = sellerDetail.EmailID;
            var firstName = sellerDetail.FirstName;
            var userName = user1.FirstName;
            
            var fromEmail = new MailAddress("jadavmehul0615@gmail.com", "Mehul Admin");
            var toEmail = new MailAddress("jadavmehul0610@gmail.com");//sellerNote emailid  email
            var fromEmailPassword = "****";//write password here.
            var subject = userName+" wants to purchase your notes ";
            string body = "<br><br>" +
                "Hello, "+firstName +
                "<br><br>We would like to inform you that, "+userName+" wants to purchase your notes. Please see Buyer Requests tab and allow download access to Buyer if you have received the payment from him."+
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

            return Json(new EmptyResult());
        }

        [HttpGet]
        [Authorize]
        public ActionResult UserProfile()
        {
            var id = (int)Session["UserID"];
            User user1 = dbObj.Users.Where(m => m.ID == id).FirstOrDefault();
            List<ReferenceData> genderList = dbObj.ReferenceDatas.Where(m => m.RefCategory == "Gender").ToList();
            List<Country> country = dbObj.Countries.ToList();
            Models.UserDetail userDetail = new Models.UserDetail
            {
                UserID = user1.ID,
                RoleID = user1.RoleID,
                FirstName = user1.FirstName,
                LastName = user1.LastName,
                GenderList = genderList,
                Countries = country,
                isAdmin = true,//admin
                EmailID = user1.EmailID
            };
            if (user1.RoleID == 2)
            {
                userDetail.isAdmin = false;//Member
            }
            return View(userDetail);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UserProfile(UserDetail model, HttpPostedFileBase ProfilePicture)
        {
            //Context.UserProfile userProfile = dbObj.UserProfiles.Where(m => m.UserID == model.UserID).FirstOrDefault();
            var id = (int)Session["USerID"];
            User user1 = dbObj.Users.Where(m => m.ID == id).FirstOrDefault();
            bool isAdmin = true;
            if (user1.RoleID == 2)
            {
                isAdmin = false;
            }
            string name = null;
            string path = null;
            if (ProfilePicture != null)
            {
                var profilePicture = Path.GetFileName(ProfilePicture.FileName);
                path = Path.Combine(Server.MapPath("~/UploadFiles"), profilePicture);
                ProfilePicture.SaveAs(path);
                name = Path.GetFileName(profilePicture);
            }
            Context.UserProfile userProfile = new Context.UserProfile
            {
                UserID = id,
                DateOfBirth = model.DOB,
                Gender = model.Gender,
                CountryCode = model.CountryCode,
                MobileNumber = model.MobileNo,
                ProfilePicture = name,
                AddressLine1 = model.AddressLine1,
                AdreessLine2 = model.AddreessLine2,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                Country = model.Country,
                University = model.University,
                Collage = model.College
            };
            if (isAdmin)
            {
                userProfile.SecondaryEmailAddress = model.EmailID;
            }
            dbObj.UserProfiles.Add(userProfile);
            dbObj.SaveChanges();
            return RedirectToAction("UserProfile");
        }
        [HttpGet]
        [Authorize]
        public ActionResult BuyerRequest()
        {
            int sellerID = (int)Session["UserID"];
            List<Download> downloads = dbObj.Downloads.Where(m => m.Seller == sellerID).Where(m => m.IsSellerHasAllowedDownload == false).Where(m => m.IsPaid == true).ToList();
            List<User> users = dbObj.Users.ToList();
            List<Context.UserProfile> usersProfile = dbObj.UserProfiles.ToList();

            var RequestTable = from d in downloads
                               join u in users on d.Downloader equals u.ID into T1
                               from u in T1
                               join p in usersProfile on u.ID equals p.UserID into T2
                               from p in T2
                               select new BuyerRequest
                               {
                                   download = d,
                                   userProfile = p,
                                   user = u
                               };

            return View(RequestTable);
        }
        [HttpGet]
        public ActionResult BuyerRequestAllow(int id)// you should make it post for security purpose.
        {
            Download download = dbObj.Downloads.Where(m => m.ID == id).FirstOrDefault();
            download.IsSellerHasAllowedDownload = true;
            var file = dbObj.SellerNotesAttachements.Where(m => m.NoteID == download.NoteID).FirstOrDefault();
            download.AttachmentPath = file.FilePath;
            dbObj.Configuration.ValidateOnSaveEnabled = false;
            dbObj.SaveChanges();

            //Sending mail to buyer that he or she can download the note.
            
            var seller = dbObj.Users.Where(m => m.ID == download.Seller).FirstOrDefault();
            var Downloader = dbObj.Users.Where(m => m.ID == download.Downloader).FirstOrDefault();
            var email = Downloader.EmailID;
            var firstName = seller.FirstName;
            var userName = Downloader.FirstName;

            var fromEmail = new MailAddress("jadavmehul0615@gmail.com", "Mehul Admin");
            var toEmail = new MailAddress("jadavmehul0610@gmail.com");//buyer emailid  email
            var fromEmailPassword = "***";
            var subject = firstName + " Allows you to download a note ";
            string body = "<br><br>" +
                "Hello, " + userName +
                "<br><br>We would like to inform you that, " + firstName + "Allows you to download a note.Please login and see My Download tabs to download particular note." +
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

            
            return RedirectToAction("BuyerRequest", "User");
        }

        [HttpGet]
        [Authorize]
        public ActionResult MyDownloads()
        {
            int userID = (int)Session["UserID"];

            List<Download> downloads = dbObj.Downloads.Where(m => m.Downloader == userID).Where(m => m.IsSellerHasAllowedDownload == true).Where(m => m.AttachmentPath != null).ToList();
            List<User> users = dbObj.Users.ToList();

            var myDownload = from d in downloads
                             join u in users on d.Downloader equals u.ID into T1
                             from u in T1
                             select new MyDownloads
                             {
                                 downloads = d,
                                 users = u
                             };

            return View(myDownload);
        }
        [HttpGet]
        [Authorize]
        public ActionResult Downloading(int id)
        {
            /* checking valid entry */
            int userId=(int)Session["UserID"];
            SellerNote sellerNote = dbObj.SellerNotes.Where(m=>m.ID==id).FirstOrDefault();
            Download download = dbObj.Downloads.Where(m => m.Downloader == userId ).Where(m=>m.NoteID==id).Where(m => m.IsAttachmentDownloaded == true).FirstOrDefault();
            if (download == null)
            {
                download = dbObj.Downloads.Where(m => m.Seller == sellerNote.SellerID).FirstOrDefault();
                if (download == null)
                {
                    return View();
                }
            }
            /* Downloading */
            SellerNotesAttachement notesAttachement = dbObj.SellerNotesAttachements.Where(x => x.NoteID == id).FirstOrDefault();
            string path = Server.MapPath("~/UploadFiles/") + notesAttachement.FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", notesAttachement.FileName);
        }
        [HttpGet]
        public ActionResult Reporting(int id)
        {
            Download download = dbObj.Downloads.Where(m => m.ID == id).FirstOrDefault();
            SellerNotesReportedIssue report = new SellerNotesReportedIssue
            {
                NoteID = download.NoteID,
                ReportedByID = (int)Session["UserID"],
                AgainstDownloadID = id,
                
            };
            return View();
        }
    
        [HttpGet]
        [Authorize]
        public ActionResult MySoldNotes()
        {
            int userID = (int)Session["UserID"];
            
            List<Download> downloads = dbObj.Downloads.Where(m => m.Seller == userID).Where(m => m.IsSellerHasAllowedDownload == true).Where(m => m.AttachmentPath != null).ToList();
            List<User> users = dbObj.Users.ToList();

            var myDownload = from d in downloads
                             join u in users on d.Downloader equals u.ID into T1
                             from u in T1
                             select new MyDownloads
                             {
                                 downloads = d,
                                 users = u
                             };
            return View(myDownload);
        }
        
        [HttpGet]
        [Authorize]
        public ActionResult MyRejectedNotes()
        {
            int userID = (int)Session["UserID"];
            List<SellerNote> sellerNotes = dbObj.SellerNotes.Where(m => m.Status==6).ToList();   //change status 
            List<NoteCategory> category = dbObj.NoteCategories.ToList();

            var rejectedNotes = from s in sellerNotes
                             join c in category on s.Category equals c.ID into T1
                             from c in T1
                             select new RejectedNotes
                             {
                                 sellerNote = s,
                                 category = c
                             };
            return View(rejectedNotes);
        }
        [HttpGet]
        [Authorize]
        public ActionResult Dashboard()
        {
            int userID = (int)Session["UserID"];
            
            List<Download> download = dbObj.Downloads.Where(m => m.Seller == userID).Where(m => m.IsSellerHasAllowedDownload == true).Where(m=>m.IsPaid==true).ToList();
            decimal money = 0;
            List<Download> myDownloads = dbObj.Downloads.Where(m => m.IsAttachmentDownloaded == true).Where(m=>m.Downloader==userID).ToList();
            List<SellerNote> rejectedNotes = dbObj.SellerNotes.Where(m => m.Status == 6).ToList();
            List<Download> BuyerRequest = dbObj.Downloads.Where(m => m.Seller == userID).Where(m => m.IsSellerHasAllowedDownload == false).Where(m => m.IsPaid == true).ToList();
            foreach (Download d in download)
            {
                money = (decimal)(money + d.PurchasedPrice);   
            }

            //Table 1
            List<SellerNote> sellerNotes = dbObj.SellerNotes.Where(m => m.SellerID == userID).ToList();   //change status 
            List<NoteCategory> category = dbObj.NoteCategories.ToList();
            List<ReferenceData> referenceDatas = dbObj.ReferenceDatas.ToList();
            var progressNote = from s in sellerNotes
                                join c in category on s.Category equals c.ID into T1
                                from c in T1
                                join r in referenceDatas on s.Status equals r.ID into T2
                                from r in T2
                                select new ProgressNotes
                                {
                                    sellerNote = s,
                                    category = c,
                                    referenceData=r
                                };

            //Table 2
            referenceDatas = dbObj.ReferenceDatas.Where(m=>m.ID == 6).ToList();    //change status for published note if youchange it in db
            var publishedNote = from s in sellerNotes
                               join c in category on s.Category equals c.ID into T1
                               from c in T1
                               join r in referenceDatas on s.Status equals r.ID into T2
                               from r in T2
                               select new ProgressNotes
                               {
                                   sellerNote = s,
                                   category = c,
                                   referenceData = r
                               };
            Dashboard dashboard = new Dashboard
            {
                numberOfNotes=download.Count,
                myRejectedNotes=rejectedNotes.Count,
                earnMoney=money,
                myDownloads=myDownloads.Count,
                buyerRequest=BuyerRequest.Count,
                progressNotes=progressNote,
                PublishedNotes=publishedNote
            };
            return View(dashboard);
        }
    }
}