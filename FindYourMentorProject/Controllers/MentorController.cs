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
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;


namespace FindYourMentorProject.Controllers
{
    public class MentorController : Controller
    {
        // GET: Mentor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Avatar()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            {
                var user = dc.RegisterMentors.Find(userid);
                ViewData["Image"] = user.ProfilePicture;
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ProfilePicture(HttpPostedFileBase file)
        {
            if (Session["UserID"] != null)
            {
                int userid = Convert.ToInt32(Session["UserID"]);

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
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                            Response.Write("Deleted");
                        }
                    }
                }
            }
            return RedirectToAction("Avatar", "Mentor");
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
        public ActionResult UpdatePersonalInfo(RegisterStudent obj)
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var existinguser = db.RegisterMentors.Find(userid);
                existinguser.Username = obj.Username.Trim();
                existinguser.State = obj.State.Trim();
                existinguser.Pincode = obj.Pincode;
                existinguser.Description = obj.Description.Trim();
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                return Json("Updated successfully!");
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
            List<CourseAdvertisement> List = db.CourseAdvertisements.Where(a => a.MentorID == userid).ToList();
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

        //[HttpPost]
        //public ActionResult AddorEditAdvertisements(CourseAdvertisement adv, HttpPostedFileBase file, HttpPostedFileBase file1)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        int userid = Convert.ToInt32(Session["UserID"]);

        //        using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
        //        {
        //            if (adv.AdvertisementID == 0)
        //            {
        //                adv.MentorID = userid;
        //                adv.CreationDate = System.DateTime.Now;

        //                if (file != null)
        //                {
        //                    string fileName = Path.GetFileName(file.FileName);
        //                    String filepath = Path.Combine(Server.MapPath("~/VideoFile/"), fileName);
        //                    if (file.ContentLength<104857600)
        //                    {
        //                        file.SaveAs(filepath);
        //                    }
        //                    adv.DemoLec1 = "/VideoFile/" + fileName;
        //                }

        //                if (file1 != null)
        //                {
        //                    string fileName = Path.GetFileName(file1.FileName);
        //                    String filepath = Path.Combine(Server.MapPath("~/VideoFile/"), fileName);
        //                    if (file1.ContentLength < 104857600)
        //                    {
        //                        file1.SaveAs(filepath);
        //                    }
        //                    adv.DemoLec2 = "/VideoFile/" + fileName;
        //                }

        //                db.CourseAdvertisements.Add(adv);
        //                db.Configuration.ValidateOnSaveEnabled = false;
        //                db.SaveChanges();
        //                return Json(new { success = true, message = "Saved Successfully !!!" }, JsonRequestBehavior.AllowGet);
        //                //return RedirectToAction("Advertisement", "Mentor");
        //            }
        //            else
        //            {
        //                adv.MentorID = userid;
        //                adv.CreationDate = System.DateTime.Now;

        //                if (file != null)
        //                {
        //                    string fileName = Path.GetFileName(file.FileName);
        //                    String filepath = Path.Combine(Server.MapPath("~/VideoFile/"), fileName);
        //                    if (file.ContentLength < 104857600)
        //                    {
        //                        file.SaveAs(filepath);
        //                    }
        //                    adv.DemoLec1 = "/VideoFile/" + fileName;
        //                }

        //                if (file1 != null)
        //                { 
        //                    string fileName = Path.GetFileName(file1.FileName);
        //                    String filepath = Path.Combine(Server.MapPath("~/VideoFile/"), fileName);
        //                    if (file1.ContentLength < 104857600)
        //                    {
        //                        file1.SaveAs(filepath);
        //                    }
        //                    adv.DemoLec2 = "/VideoFile/" + fileName;
        //                }

        //                db.Entry(adv).State = EntityState.Modified;
        //                db.SaveChanges();
        //                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
        //                //return RedirectToAction("Advertisement", "Mentor");
        //            }

        //        }

        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "Error !!!" }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        //[HttpPost]
        //public ActionResult AddorEditAdvertisements(CourseAdvertisement adv)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        int userid = Convert.ToInt32(Session["UserID"]);

        //        using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
        //        {
        //            if (adv.AdvertisementID == 0)
        //            {
        //                adv.MentorID = userid;
        //                adv.CreationDate = System.DateTime.Now;
        //                //var path1 = saveToPhysicalLocation(adv.videoFile1);      
        //                //var path2 = saveToPhysicalLocation(adv.videoFile2);

        //                //adv.DemoLec1 = path1;
        //                //adv.DemoLec2 = path2;

        //                db.CourseAdvertisements.Add(adv);
        //                db.Configuration.ValidateOnSaveEnabled = false;
        //                db.SaveChanges();
        //                return Json(new { success = true, message = "Saved Successfully !!!" }, JsonRequestBehavior.AllowGet);
        //                //return RedirectToAction("Advertisement", "Mentor");
        //            }
        //            else
        //            {
        //                adv.MentorID = userid;
        //                adv.CreationDate = System.DateTime.Now;

        //                //if (file != null)
        //                //{
        //                //    string fileName = Path.GetFileName(file.FileName);
        //                //    String filepath = Path.Combine(Server.MapPath("~/VideoFile/"), fileName);
        //                //    if (file.ContentLength < 104857600)
        //                //    {
        //                //        file.SaveAs(filepath);
        //                //    }
        //                //    adv.DemoLec1 = "/VideoFile/" + fileName;
        //                //}

        //                //if (file1 != null)
        //                //{
        //                //    string fileName = Path.GetFileName(file1.FileName);
        //                //    String filepath = Path.Combine(Server.MapPath("~/VideoFile/"), fileName);
        //                //    if (file1.ContentLength < 104857600)
        //                //    {
        //                //        file1.SaveAs(filepath);
        //                //    }
        //                //    adv.DemoLec2 = "/VideoFile/" + fileName;
        //                //}

        //                db.Entry(adv).State = EntityState.Modified;
        //                db.SaveChanges();
        //                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
        //                //return RedirectToAction("Advertisement", "Mentor");
        //            }

        //        }

        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "Error !!!" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public ActionResult AddorEditAdvertisements(CourseAdvertisement adv)
        {
          int userid = Convert.ToInt32(Session["UserID"]);
                if (ModelState.IsValid)
                {
                    if (adv.VideoUpload1 != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(adv.VideoUpload1.FileName);
                        string extension = Path.GetExtension(adv.VideoUpload1.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        adv.DemoLec1 = "~/VideoFile/" + fileName;
                        adv.VideoUpload1.SaveAs(Path.Combine(Server.MapPath("~/VideoFile/"), fileName));
                    }
                    if (adv.VideoUpload2 != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(adv.VideoUpload2.FileName);
                        string extension = Path.GetExtension(adv.VideoUpload2.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        adv.DemoLec2 = "~/VideoFile/" + fileName;
                        adv.VideoUpload2.SaveAs(Path.Combine(Server.MapPath("~/VideoFile/"), fileName));
                    }
                    using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
                    {
                        if (adv.AdvertisementID == 0)
                        {
                            db.CourseAdvertisements.Add(adv);
                            adv.MentorID = userid;
                            adv.CreationDate = System.DateTime.Now;
                            db.SaveChanges();
                            return Json(new { success = true, message = "Added Successfully" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            adv.MentorID = userid;
                            adv.CreationDate = System.DateTime.Now;
                            db.Entry(adv).State = EntityState.Modified;
                            db.SaveChanges();
                            return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Some error" }, JsonRequestBehavior.AllowGet);
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
                //Application appAdv = db.Applications.Where(x => x.AdvertisementID == id).FirstOrDefault<Application>();
                //if (appAdv!=null)
                //{
                //    db.Applications.Remove(appAdv);
                //}
                //Appointment apptmtAdv = db.Appointments.Where(x => x.AdvertisementID == id).FirstOrDefault<Appointment>();
                //if(apptmtAdv!=null)
                //{
                //    db.Appointments.Remove(apptmtAdv);
                //}
                //SavedList saveAdv = db.SavedLists.Where(x => x.AdvertisementID == id).FirstOrDefault<SavedList>();
                //if(saveAdv!=null)
                //{
                //    db.SavedLists.Remove(saveAdv);
                //}

                db.CourseAdvertisements.Remove(advert);
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
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
                    ApprovalNotificationMentee(app.MenteeName, app.MenteeEmailID, advid.MentorName, advid.CourseName, advid.Address, advid.State);
                    return Json(new { success = true, message = "Approve status updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
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
                    RejectNotificationMentee(app.MenteeName, app.MenteeEmailID, advid.MentorName, advid.CourseName, advid.Address, advid.State);
                    return Json(new { success = true, message = "Reject status updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, message = "You cannot update status more than 5 times !!!" }, JsonRequestBehavior.AllowGet);
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

    }
}