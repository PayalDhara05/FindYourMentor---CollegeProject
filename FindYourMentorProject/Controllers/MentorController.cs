using FindYourMentorProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;


namespace FindYourMentorProject.Controllers
{
    public class MentorController : Controller
    {
        // GET: Mentor
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ProfilePicture(HttpPostedFileBase file, string submitButton)
        {
            if (Session["UserID"] != null)
            {
                int userid = Convert.ToInt32(Session["UserID"]);

                if (submitButton == "Upload")
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        String filepath = Path.Combine(Server.MapPath("/Image/"), fileName);
                        file.SaveAs(filepath);
                        using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
                        {
                            var user = dc.RegisterMentors.Single(c => c.UserID == userid);
                            string oldFileName = user.ProfilePicture;
                            user.ProfilePicture = "/Image/" + fileName;
                            dc.Configuration.ValidateOnSaveEnabled = false;
                            dc.SaveChanges();

                            string fullPath = Request.MapPath("~" + oldFileName);
                            if (oldFileName != "/Image/defaultProfile1.jpg")
                            {
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                    Response.Write("Deleted");
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
                    {
                        var user = dc.RegisterMentors.Single(c => c.UserID == userid);
                        string oldFileName = user.ProfilePicture;
                        user.ProfilePicture = "/Image/defaultProfile1.jpg";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();

                        string fullPath = Request.MapPath("~" + oldFileName);
                        if (oldFileName != "/Image/defaultProfile1.jpg")
                        {
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                                Response.Write("Deleted");
                            }
                        }
                    }
                }
            }
            return RedirectToAction("PersonalInfo", "Mentor");
        }

        [HttpGet]
        [Authorize]
        public new ActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult PersonalInfo()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            {
                var user = dc.RegisterMentors.Find(userid);
                return View(user);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdatePersonalInfo(RegisterMentor obj)
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var existinguser = db.RegisterMentors.Find(userid);
                existinguser.FirstName = obj.FirstName;
                existinguser.LastName = obj.LastName;
                existinguser.City = obj.City;
                existinguser.State = obj.State;
                existinguser.Pincode = obj.Pincode;
                existinguser.ContactNo = obj.ContactNo;
                existinguser.GithubID = obj.GithubID;
                existinguser.LinkedinID = obj.LinkedinID;
                existinguser.Description = obj.Description;
                existinguser.Age = obj.Age;
                existinguser.Address = obj.Address;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                return RedirectToAction("PersonalInfo", "Mentor");
            }
            //return RedirectToAction("PersonalInfo", "Student");
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordModel pass)
        {
            string message = "";
            if (ModelState.IsValid)
            {

                if (Session["UserID"] != null)
                {
                    int userid = Convert.ToInt32(Session["UserID"]);
                    using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
                    {
                        var user = dc.RegisterMentors.Single(c => c.UserID == userid);
                        if (string.Compare(Crypto.Hash(pass.OldPassword), user.Password) == 0)
                        {
                            user.Password = Crypto.Hash(pass.NewPassword);
                            dc.Configuration.ValidateOnSaveEnabled = false;
                            dc.SaveChanges();
                            message = "Password changed successfully";
                        }
                        else
                        {
                            message = "Invalid Password";
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            ViewBag.message = message;
            return View();
        }

        [HttpGet]
        public ActionResult Advertisement()
        {
            return View();
        }

        public JsonResult AdvertisementData()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<CourseAdvertisement> List = db.CourseAdvertisements.Where(a => a.MentorID == userid && a.RemovalStatus == "No").ToList();
            return Json(List, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult AddorEditAdvertisements(int id = 0)
        {
            CourseAdvertisement cAdv = new CourseAdvertisement();
            if (id != 0)
            {
                using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
                {
                    cAdv = db.CourseAdvertisements.Where(x => x.AdvertisementID == id).FirstOrDefault<CourseAdvertisement>();
                }
            }
            return View(cAdv);
        }

        [HttpPost]
        public ActionResult AddorEditAdvertisements(CourseAdvertisement adv, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, HttpPostedFileBase file4)
        {
          int userid = Convert.ToInt32(Session["UserID"]);
            CourseAdvertisement cAdv = new CourseAdvertisement();
            if (ModelState.IsValid)
                {
                    if (file1 != null)
                    {
                        string fileName = Path.GetFileName(file1.FileName);
                        String filepath = Path.Combine(Server.MapPath("~/VideoFile/"), fileName);
                        if (file1.ContentLength < 104857600)
                        {
                            file1.SaveAs(filepath);
                        }
                        adv.DemoLec1 = "/VideoFile/" + fileName;
                    }

                    if (file2 != null)
                    {
                        string fileName = Path.GetFileName(file2.FileName);
                        String filepath = Path.Combine(Server.MapPath("~/VideoFile/"), fileName);
                        if (file2.ContentLength < 104857600)
                        {
                            file2.SaveAs(filepath);
                        }
                        adv.DemoLec2 = "/VideoFile/" + fileName;
                    }

                    if (file3 != null)
                    {
                        string fileName = Path.GetFileName(file3.FileName);
                        String filepath = Path.Combine(Server.MapPath("~/VideoFile/"), fileName);
                        if (file3.ContentLength < 104857600)
                        {
                            file3.SaveAs(filepath);
                        }
                        adv.DemoLec3 = "/VideoFile/" + fileName;
                    }

                    if (file4 != null)
                    {
                        string fileName = Path.GetFileName(file4.FileName);
                        String filepath = Path.Combine(Server.MapPath("~/VideoFile/"), fileName);
                        if (file4.ContentLength < 104857600)
                        {
                            file4.SaveAs(filepath);
                        }
                        adv.DemoLec4 = "/VideoFile/" + fileName;
                    }
                    using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
                    {
                        if (adv.AdvertisementID == 0)
                        {
                            db.CourseAdvertisements.Add(adv);
                            adv.MentorID = userid;
                            adv.CreationDate = System.DateTime.Now;
                            adv.RemovalStatus = "No";
                            db.SaveChanges();
                            //return Json(new { success = true, message = "Added Successfully" }, JsonRequestBehavior.AllowGet);
                            return RedirectToAction("Advertisement", "Mentor");
                        }
                        else
                        {
                            adv.MentorID = userid;
                            adv.CreationDate = System.DateTime.Now;
                            adv.RemovalStatus = "No";
                            db.Entry(adv).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Advertisement", "Mentor"); 
                        }
                    }
                }
                else
                {
                            return View(cAdv);
                }       
            }


        [NonAction]
        public string saveToPhysicalLocation(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string fileName = Path.GetFileName(file.FileName);
                String filepath = Path.Combine(Server.MapPath("~/VideoFile/"), fileName);
                if (file.ContentLength < 104857600)
                {
                    file.SaveAs(filepath);
                    return filepath;
                }   
            }
            return String.Empty;
        }

        [HttpPost]
        public ActionResult DeleteAdvertisement(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                CourseAdvertisement advert = db.CourseAdvertisements.Where(x => x.AdvertisementID == id).FirstOrDefault<CourseAdvertisement>();
                advert.RemovalStatus = "Yes";
                db.SaveChanges();
                CheckAdvertisement(id);
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }


        [NonAction]
        public void CheckAdvertisement(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                SavedList saveAdv = db.SavedLists.Where(a => a.AdvertisementID == id).FirstOrDefault();
                if(saveAdv == null)
                {
                    CourseAdvertisement cadv = db.CourseAdvertisements.Where(a => a.AdvertisementID == id).FirstOrDefault();
                    db.CourseAdvertisements.Remove(cadv);
                    db.SaveChanges();
                }
            }
        }
        public ActionResult ViewAdvertisement(int id = 0)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                return View(db.CourseAdvertisements.Where(x => x.AdvertisementID == id).FirstOrDefault<CourseAdvertisement>());
            }
        }

        public ActionResult viewMenteeApplication()
        {
            return View();
        }
        public ActionResult viewMenteeApplicationData()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<Application> List = db.Applications.Where(a => a.MentorID == userid && a.MentorRemoveStatus == "Unremoved").ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewFullMenteeApplication(int id = 0)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                return View(db.Applications.Where(a => a.ApplicationID == id).FirstOrDefault<Application>());
            }
        }

        public ActionResult ViewCourseApplication(int id = 0)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                return View(db.CourseAdvertisements.Where(a => a.AdvertisementID == id).FirstOrDefault<CourseAdvertisement>());
            }
        }

        public ActionResult ApproveStatus(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var app = db.Applications.Where(a => a.ApplicationID == id).FirstOrDefault();
                var advid = db.CourseAdvertisements.Where(a => a.MentorID == app.MentorID).FirstOrDefault();
                if(app.statusCounter < 3)
                {
                    app.ApplicationStatus = "Approve";
                    app.statusCounter++;
                    app.StatusUpdateTime = System.DateTime.Now;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    Thread.Sleep(1500);
                    //ApprovalNotificationMentee(app.MenteeName, app.MenteeEmailID, advid.MentorName, advid.CourseName, advid.Address, advid.State);
                    return Json(new { success = true, message = "Approve status updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Thread.Sleep(2000);
                    return Json(new { success = true, message = "You cannot update status more than 3 times !!!" }, JsonRequestBehavior.AllowGet);
                }
                
            }
        }

        public ActionResult RejectStatus(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var app = db.Applications.Where(a => a.ApplicationID == id).FirstOrDefault();
                var advid = db.CourseAdvertisements.Where(a => a.MentorID == app.MentorID).FirstOrDefault();
                if (app.statusCounter < 3)
                {
                    app.ApplicationStatus = "Reject";
                    app.statusCounter++;
                    app.StatusUpdateTime = System.DateTime.Now;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    Thread.Sleep(1500);
                    //RejectNotificationMentee(app.MenteeName, app.MenteeEmailID, advid.MentorName, advid.CourseName, advid.Address, advid.State);
                    return Json(new { success = true, message = "Reject status updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Thread.Sleep(2000);
                    return Json(new { success = true, message = "You cannot update status more than 3 times !!!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult UpdateRemoveStatus(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var app = db.Applications.Where(a => a.ApplicationID == id).FirstOrDefault();
                var advid = db.CourseAdvertisements.Where(a => a.MentorID == app.MentorID).FirstOrDefault();
                app.MentorRemoveStatus = "Removed";
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                return Json(new { success = true, message = "Removed" }, JsonRequestBehavior.AllowGet);
            }
        }

        [NonAction]
        public void ApprovalNotificationMentee(string menteeName, string menteeEmailID, string MentorName, string CourseName, string Address, string State)
        {
            string subject = "";
            string body = "";

            var fromEmail = new MailAddress("payaldhara05@gmail.com", "Find Your Mentor");
            var toEmail = new MailAddress(menteeEmailID);
            var fromEmailPassword = "priyapayu";

            subject = "Mentor Response to your Application Request";
            body = " Dear " + menteeName + ",<br><br>" + "Congratulations !! Your Apllication request for the courses " + CourseName +" under mentors" + MentorName + ".<br> Kindly do rest of the procdures. If want pay online fees, you can do our website 'Find Your Mentor'. <br>For more information, please do visit our website.<br>All the best for your future !!!<br><br><br><br>Thanks & Regards<br>Find Your Mentor";

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
        public void RejectNotificationMentee(string menteeName, string menteeEmailID, string MentorName, string CourseName, string Address, string State)
        {
            string subject = "";
            string body = "";

            var fromEmail = new MailAddress("payaldhara05@gmail.com", "Find Your Mentor");
            var toEmail = new MailAddress(menteeEmailID);
            var fromEmailPassword = "priyapayu";

            subject = "Mentor Response to your Application Request";
            body = " Dear " + menteeName + ",<br><br>" + "Sorry !! Your Apllication request for the courses " + CourseName + " under mentors" + MentorName + ".<br> Kindly do rest of the procdures. If want pay online fees, you can do our website 'Find Your Mentor'. <br>For more information, please do visit our website.<br>All the best for your future !!!<br><br><br><br>Thanks & Regards<br>Find Your Mentor";

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


        public ActionResult viewMenteeAppointment()
        {
            return View();
        }
        public ActionResult viewMenteeAppointmentData()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<Appointment> List = db.Appointments.Where(a => a.MentorID == userid && a.AppointmentRemoveStatus == "Unremoved").ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewFullMenteeAppointment(int id = 0)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                return View(db.Appointments.Where(a => a.AppointmentID == id).FirstOrDefault<Appointment>());
            }
        }

        public ActionResult ConfirmStatus(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var app = db.Appointments.Where(a => a.AppointmentID == id).FirstOrDefault();
                var advid = db.CourseAdvertisements.Where(a => a.MentorID == app.MentorID).FirstOrDefault();
                if (app.StatusCounter < 3)
                {
                    app.AppointmentStatus = "Confirmed";
                    app.StatusCounter++;
                    app.AppointmentUpdateStatusTime = System.DateTime.Now;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    Thread.Sleep(1500);
                    //ConfirmNotificationMentee(app.MenteeName, app.MenteeEmailID, advid.MentorName, advid.CourseName, advid.Address, advid.State);
                    return Json(new { success = true, message = "Confirm status updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Thread.Sleep(2000);
                    return Json(new { success = true, message = "You cannot update status more than 3 times !!!" }, JsonRequestBehavior.AllowGet);
                }

            }
        }

        public ActionResult CancelStatus(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var app = db.Appointments.Where(a => a.AppointmentID == id).FirstOrDefault();
                var advid = db.CourseAdvertisements.Where(a => a.MentorID == app.MentorID).FirstOrDefault();
                if (app.StatusCounter < 3)
                {
                    app.AppointmentStatus = "Cancelled";
                    app.StatusCounter++;
                    app.AppointmentUpdateStatusTime = System.DateTime.Now;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    Thread.Sleep(1500);
                    //CancelNotificationMentee(app.MenteeName, app.MenteeEmailID, advid.MentorName, advid.CourseName, advid.Address, advid.State);
                    return Json(new { success = true, message = "Cancel status updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Thread.Sleep(2000);
                    return Json(new { success = true, message = "You cannot update status more than 3 times !!!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult UpdateRemoveAppointmentStatus(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var app = db.Appointments.Where(a => a.AppointmentID == id).FirstOrDefault();
                var advid = db.CourseAdvertisements.Where(a => a.MentorID == app.MentorID).FirstOrDefault();
                app.AppointmentRemoveStatus = "Removed";
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                return Json(new { success = true, message = "Removed" }, JsonRequestBehavior.AllowGet);
            }
        }

        [NonAction]
        public void ConfirmNotificationMentee(string menteeName, string menteeEmailID, string MentorName, string CourseName, string Address, string State)
        {
            string subject = "";
            string body = "";

            var fromEmail = new MailAddress("payaldhara05@gmail.com", "Find Your Mentor");
            var toEmail = new MailAddress(menteeEmailID);
            var fromEmailPassword = "priyapayu";

            subject = "Mentor Response to your Application Request";
            body = " Dear " + menteeName + ",<br><br>" + "Your Appointment request for the courses " + CourseName + " under mentors" + MentorName + " have been confirmed.<br> Kindly do rest of the procdures. If want pay online fees, you can do our website 'Find Your Mentor'. <br>For more information, please do visit our website.<br>All the best for your future !!!<br><br><br><br>Thanks & Regards<br>Find Your Mentor";

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
        public void CancelNotificationMentee(string menteeName, string menteeEmailID, string MentorName, string CourseName, string Address, string State)
        {
            string subject = "";
            string body = "";

            var fromEmail = new MailAddress("payaldhara05@gmail.com", "Find Your Mentor");
            var toEmail = new MailAddress(menteeEmailID);
            var fromEmailPassword = "priyapayu";

            subject = "Mentor Response to your Application Request";
            body = " Dear " + menteeName + ",<br><br>" + "Your Appointment request for the courses " + CourseName + " under mentors" + MentorName + " have been cancelled due to some reasons.<br> Kindly do rest of the procdures. If want pay online fees, you can do our website 'Find Your Mentor'. <br>For more information, please do visit our website.<br>All the best for your future !!!<br><br><br><br>Thanks & Regards<br>Find Your Mentor";

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


        public ActionResult viewOnlineFeeApplication()
        {
            return View();
        }
        public ActionResult viewOnlineFeeApplicationData()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<Fee> List = db.Fees.Where(a => a.MentorID == userid && a.PaymentMode == "Online").ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewOfflineFeeApplication()
        {
            return View();
        }
        public ActionResult viewOfflineFeeApplicationData()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<Fee> List = db.Fees.Where(a => a.MentorID == userid && a.PaymentMode == "Offline").ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }

    }
}