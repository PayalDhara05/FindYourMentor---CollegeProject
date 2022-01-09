using FindYourMentorProject.Models;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace FindYourMentorProject.Controllers
{
    [HandleError]
    public class UserController : Controller
    {
        [HttpGet]                              // Default method
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegistrationStudent()
        {
            return View();
        }

        public ActionResult Registration()            //Common Registration Page
        {
            return View();
        }

        public ActionResult RegistrationMentor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]     // we add in View i.e. @Html.AntiForgeryToken() generates __RequestVerificationToken on load time and [ValidateAntiForgeryToken] available on Controller method. Match this token on post time. If token is the same, then it means this is a valid request.
        public ActionResult RegistrationStudent([Bind(Exclude = "IsEmailVerified,ActivationCode,Role")] RegisterStudent studentuser, HttpPostedFileBase file)   // this bind attribute will exclude an "IsEmailVerified,ActivationCode,Role" property from being posted to server.
        {
            bool Status = false;
            string message = "";
            
            // Model Validation 
            if (ModelState.IsValid)
            {
                studentuser.Role = "Mentee";

                #region Email is already Exist 
                var isExist = IsEmailExist(studentuser.EmailID,studentuser.Role);
                if (isExist)
                {
                    ViewBag.Message1 = "Email already exist";
                    return View(studentuser);
                }
                #endregion

                #region SaveProfilePicture
                if (file != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    String filepath = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    file.SaveAs(filepath);
                    studentuser.ProfilePicture = "/Image/" + fileName;
                }
                #endregion

                //The #region directive partitions your code. With #region, we can organize code into blocks that can be expanded or collapsed visually.
                #region Generate Activation Code     
                studentuser.ActivationCode = Guid.NewGuid();    //GUID stands for Global Unique Identifier. A GUID is a 128-bit integer (16 bytes) that you can use across all computers and networks wherever a unique identifier is required.              
                #endregion

                #region  Password Hashing 
                studentuser.Password = Crypto.Hash(studentuser.Password);
                studentuser.ConfirmPassword = Crypto.Hash(studentuser.ConfirmPassword);
                #endregion

                studentuser.IsEmailVerified = false;

                try
                {
                    #region Save to Database
                    using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
                    {
                        dc.RegisterStudents.Add(studentuser);
                        dc.SaveChanges();
                        ModelState.Clear();

                        //Send Email to User
                        SendVerificationLinkEmail(studentuser.EmailID, studentuser.ActivationCode.ToString(), studentuser.Role);
                        message = "Registration successfully done. Account activation link " +
                            " has been sent to your email id:" + studentuser.EmailID;
                        Status = true;
                        
                    }   
                    #endregion
                }
                catch(Exception e)
                {
                    Response.Write(e);
                }
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }

        public JsonResult ValidateMenteeEmailID(string EmailID)     // Remote validation of EmailID
        {
            using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            {
                var v = dc.RegisterStudents.Where(a => a.EmailID == EmailID).FirstOrDefault();
                return v != null ?
                    Json("EmailID already exists", JsonRequestBehavior.AllowGet) : Json(true, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ValidateMentorEmailID(string EmailID)      // Remote validation of EmailID
        {
            using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            {
                var v = dc.RegisterMentors.Where(a => a.EmailID == EmailID).FirstOrDefault();
                return v != null ?
                    Json("EmailID already exists", JsonRequestBehavior.AllowGet) : Json(true, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrationMentor([Bind(Exclude = "IsEmailVerified,ActivationCode,Role")] RegisterMentor mentoruser, HttpPostedFileBase file)
        {
            bool Status = false;
            string message = "";
            
            if (ModelState.IsValid)
            {
                mentoruser.Role = "Mentor";

                #region Email is already Exist 
                var isExist = IsEmailExist(mentoruser.EmailID,mentoruser.Role);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    message = "Email already exist";
                    return View(mentoruser);
                }
                #endregion

                if (file != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    String filepath = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    file.SaveAs(filepath);
                    mentoruser.ProfilePicture = "/Image/" + fileName;
                }

                #region Generate Activation Code 
                mentoruser.ActivationCode = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
                mentoruser.Password = Crypto.Hash(mentoruser.Password);
                mentoruser.ConfirmPassword = Crypto.Hash(mentoruser.ConfirmPassword); //
                #endregion

                mentoruser.IsEmailVerified = false;

                #region Save to Database
                using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
                {
                    dc.RegisterMentors.Add(mentoruser);
                    dc.SaveChanges();
                    ModelState.Clear();

                    //Send Email to User
                    SendVerificationLinkEmail(mentoruser.EmailID, mentoruser.ActivationCode.ToString(),mentoruser.Role);
                    message = "Registration successfully done. Account activation link " +
                        " has been sent to your email id:" + mentoruser.EmailID;
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }

        [NonAction]
        public bool IsEmailExist(string emailID,string Role)
        {
            using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            {
                if(Role=="Mentee")
                {
                    var v = dc.RegisterStudents.Where(a => a.EmailID == emailID).FirstOrDefault();
                    return v != null;
                }
                else
                {
                    var v = dc.RegisterMentors.Where(a => a.EmailID == emailID).FirstOrDefault();
                    return v != null;
                }
            }
        }

        [NonAction]  //public method in a controller but do not want to treat it as an action method. An action method is a public method in a controller that can be invoked using a URL
        public void SendVerificationLinkEmail(string emailID, string activationCode, string Role, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/User/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("payaldhara05@gmail.com", "Find Your Mentor");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "priyapayu"; 

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Find Your Mentor account as "+ Role + " is " +
                    "successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href=" + link + ">Verify your account</a>";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }


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
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
                                                                // Confirm password does not match issue on save changes

                var v = dc.RegisterStudents.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                var m = dc.RegisterMentors.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v == null)
                {
                    if (m != null && m.Role == "Mentor")
                    {
                        if (m != null)
                        {
                            m.IsEmailVerified = true;
                            var bytes = new Byte[16];
                            m.ActivationCode = new Guid(bytes);     // Deactivating code. Since verification can be done only one time
                            dc.SaveChanges();
                            Status = true;
                        }
                        else
                        {
                            ViewBag.Message = "Invalid Request";
                        }
                    }
                    else
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
                else
                {
                    if (v != null && v.Role == "Mentee")
                    {
                        if (v != null)
                        {
                            v.IsEmailVerified = true;
                            var bytes = new Byte[16];
                            v.ActivationCode = new Guid(bytes);
                            dc.SaveChanges();
                            Status = true;
                        }
                        else
                        {
                            ViewBag.Message = "Invalid Request";
                        }
                    }
                    else
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
                ViewBag.Status = Status;
                return View();
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            string message = "";
            using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            {
                string Role = Convert.ToString(login.UserRole);

                if (Role == "Student")
                {
                    var v = dc.RegisterStudents.Where(a => a.EmailID == login.EmailID).FirstOrDefault();
                    if (v == null)
                    {
                        ViewBag.message1 = "Account with given EmailID does not exist";
                    }
                    else
                    {
                        if (!v.IsEmailVerified)
                        {
                            message = "Please verify your email first";
                            ViewBag.Message = message;
                            return View();
                        }
                        if (string.Compare(Crypto.Hash(login.Password), v.Password) == 0)
                        {
                            int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                            var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);
                            string emailID = ticket.Name;
                            Session["EmailID"] = ticket.Name;
                            var w = dc.RegisterStudents.Where(a => a.EmailID == emailID).FirstOrDefault();
                            Session["UserID"] = w.UserID.ToString();

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Student");
                            }
                        }
                        else
                        {
                            message = "Invalid credentials provided";
                        }
                    }
                }
                else
                {
                    var m = dc.RegisterMentors.Where(a => a.EmailID == login.EmailID).FirstOrDefault();
                    if (m == null)
                    {
                        ViewBag.message1 = "Account with given EmailID does not exist";
                    }
                    else
                    {
                        if (!m.IsEmailVerified)
                        {
                            message = "Please verify your email first";
                            return View();
                        }
                        if (string.Compare(Crypto.Hash(login.Password), m.Password) == 0)
                        {
                            int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                            var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);
                            string emailID = ticket.Name;
                            Session["EmailID"] = ticket.Name;
                            var n = dc.RegisterMentors.Where(a => a.EmailID == emailID).FirstOrDefault();
                            Session["UserID"] = n.UserID.ToString();

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Mentor");
                            }
                        }
                        else
                        {
                            message = "Invalid credentials provided";
                        }
                    }
                }

                ViewBag.Message = message;
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string EmailID,string Role)
        {
            string message = "";
            bool status = false;

            using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            {
                if(Role == "Mentee")
                {
                    var account = dc.RegisterStudents.Where(a => a.EmailID == EmailID).FirstOrDefault();
                    if (account != null)
                    {
                        string resetCode = Guid.NewGuid().ToString();
                        SendVerificationLinkEmail(account.EmailID, resetCode,Role, "ResetPassword");
                        account.ResetPasswordCode = resetCode;
                        //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                        //in our model class 
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "Reset password link has been sent to your email id.";
                    }
                    else
                    {
                        message = "Account not found";
                    }
                }
                else
                {
                    var account = dc.RegisterMentors.Where(a => a.EmailID == EmailID).FirstOrDefault();
                    if (account != null)
                    {
                        //Send email for reset password
                        string resetCode = Guid.NewGuid().ToString();
                        SendVerificationLinkEmail(account.EmailID, resetCode,Role, "ResetPassword");
                        account.ResetPasswordCode = resetCode;
                        //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                        //in our model class
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "Reset password link has been sent to your email id.";
                    }
                    else
                    {
                        message = "Account not found";
                    }
                }
                
            }
            ViewBag.Message = message;
            ModelState.Clear();
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            else
            {
                using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
                {
                    var mentee = dc.RegisterStudents.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                    var mentor = dc.RegisterMentors.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                    if (mentee == null)
                    {
                        if (mentor != null)
                        {
                            ResetPasswordModel model = new ResetPasswordModel();
                            model.ResetCode = id;
                            return View(model);
                        }
                        else
                        {
                            return RedirectToAction("NotFound", "Error");
                        }
                    }
                    else
                    {
                        if (mentee != null)
                        {
                            ResetPasswordModel model = new ResetPasswordModel();
                            model.ResetCode = id;
                            return View(model);
                        }
                        else
                        {
                            return RedirectToAction("NotFound", "Error");
                        }
                    }
                }
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            try
            {
                if (ModelState.IsValid)
                {
                    using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
                    {
                        var mentee = dc.RegisterStudents.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                        var mentor = dc.RegisterMentors.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                        if (mentee == null)
                        {
                            if (mentor != null)
                            {
                                mentor.Password = Crypto.Hash(model.NewPassword);
                                mentor.ResetPasswordCode = "";
                                dc.Configuration.ValidateOnSaveEnabled = false;
                                dc.SaveChanges();
                                message = "New password updated successfully";
                                ViewBag.Message = message;
                                return View();
                            }
                            else
                            {
                                message = "Something invalid";
                            }
                        }
                        else
                        {
                            if (mentee != null)
                            {
                                mentee.Password = Crypto.Hash(model.NewPassword);
                                mentee.ResetPasswordCode = "";
                                dc.Configuration.ValidateOnSaveEnabled = false;
                                dc.SaveChanges();
                                message = "New password updated successfully";
                            }
                            else
                            {
                                message = "Something invalid";
                            }
                        }

                    }
                }
            }
            catch(Exception e)
            {
                Response.Write(e);
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult Contact(ContactU con)
        {
            if (ModelState.IsValid)
            {
                using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
                {
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.ContactUs.Add(con);
                    dc.SaveChanges();
                    ModelState.Clear();
                    return Json(new { success = true, message = "Contact message sent !!!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = true, message = "Error sent !!!" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}

