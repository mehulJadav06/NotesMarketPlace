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
using System.Web.Services.Description;


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
        public bool IsEmailExist(string email) {
            var v = dbObj.Users.Where(x => x.EmailID == email).FirstOrDefault();
            return v != null;
        }
        [HttpPost]
        public ActionResult AddUser(User model)
        {
            bool status = false;
            if (ModelState.IsValid) {

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
                    ModelState.AddModelError("EmailExist","Email already exist");
                    return View("SignUp");
                }
                user.RoleID = 2;
                user.IsEmailVerified = false;
                user.IsActive = true;

                dbObj.Users.Add(user);
                dbObj.SaveChanges();

                status = true;
                //var activationCode = Guid.NewGuid();
                SendLinkForEmailVerification(user.EmailID,user.FirstName,user.LastName);
                return View("SignUp");
            }
            return View("SignUp");

        }

        //Email verification
        [NonAction]
        public void SendLinkForEmailVerification(string email,string firstName,string lastName) {
            var verifyURL = "User/EmailVerification/" + email;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery,verifyURL);

            var fromEmail = new MailAddress("jadavmehul0615@gmail.com","Mehul Admin");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "15061996";

            string subject = "Your Account has been Succesfully Created!";
            
            string body = "<br><br>" +
                "Hello, "+ firstName+" "+lastName+"<br><br>Thank you for signing up with us." +
                "Please click on below link to verify your email address and to do login.<br><br>"+
                "<a href="+link+">Verify Email Page</a>"+
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

            var v = dbObj.Users.Where(x =>x.EmailID == id).FirstOrDefault();

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
                if (string.Compare(model.Password,v.Password)== 0)
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

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        string newPassword = "";
                        SendLinkForEmailVerification(v.EmailID,v.FirstName,v.LastName);
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

        // Logout
        [Authorize]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [NonAction]
        public void SendLinkForNewPassword(string email,string newPassword)
        {
            var v = dbObj.Users.Where(m => m.EmailID == email).FirstOrDefault();
            var fromEmail = new MailAddress("jadavmehul0615@gmail.com", "Mehul Admin");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "15061996";

            string subject = "Your Password has been Succesfully Updated!";
            
            string body = "<br><br>" +
                "Hello, " + v.FirstName + " " + v.LastName + "<br><br>We have generated new password for you<br><br>" +
                "Password : "+ newPassword +
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
            while(i<4)
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
            List<NoteType> noteTypes= dbObj.NoteTypes.ToList();
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
            string upload_note_name = null, path = null, preview_path=null;
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
                var sellerNote = new SellerNote
                {
                    Title = model.Title,
                    Category = model.CategoryID,
                    NoteType = model.NoteTypeID,
                    Status = 4,
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
                    SellerID = 4
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
            return RedirectToAction("SignUp");
        }

        //Contact us

        [NonAction]
        public void SendEmailForContactUs(string email, string fullname, string subject,string question)
        {
            var fromEmail = new MailAddress("jadavmehul0615@gmail.com", "Mehul Admin");
            var toEmail = new MailAddress("jadavmehul0610@gmail.com");
            var fromEmailPassword = "15061996";

            //string subject = subject;

            string body = "<br><br>" +
                "Hello, " + "<br><br>" + question +
                "<br><br>Regards,<br>" + fullname+"<br>"+email;

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

    }
}