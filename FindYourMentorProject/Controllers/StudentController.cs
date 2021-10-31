using FindYourMentorProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace FindYourMentorProject.Controllers
{
    public class StudentController : Controller
    {
        [Authorize]
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Avatar()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            {
                var user = dc.RegisterStudents.Find(userid);
                ViewData["Image"] = user.ProfilePicture;
            }
            return View();
        }

        [HttpGet]
        [Authorize]
        public new ActionResult Profile()
        {
            return View();
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
                        var user = dc.RegisterStudents.Single(c => c.UserID == userid);
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

        [HttpPost]
        [Authorize]
        public ActionResult ProfilePicture(HttpPostedFileBase file)
        {
            if (Session["UserID"] != null)
            {
                int userid = Convert.ToInt32(Session["UserID"]);

                if (file!= null && file.ContentLength>0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    String filepath = Path.Combine(Server.MapPath("/Image/"), fileName);
                    file.SaveAs(filepath);
                    using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
                    {
                        var user = dc.RegisterStudents.Single(c => c.UserID == userid);
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
            return RedirectToAction("Avatar", "Student");
        }

        [HttpGet]
        [Authorize]
        public ActionResult PersonalInfo()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            {
                var user = dc.RegisterStudents.Find(userid);
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
                    var existinguser = db.RegisterStudents.Find(userid);
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
        public ActionResult Notes()
        {
            return View();
        }


        //public ActionResult NotesData()
        //{
        //    int userid = Convert.ToInt32(Session["UserID"]);
        //    using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
        //    {
        //        var existinguser = db.AddNotesMentees.ToList<AddNotesMentee>();
        //        return Json(new { data = existinguser }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public JsonResult NotesData()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<AddNotesMentee> List = db.AddNotesMentees.Where(a => a.UserID == userid).ToList();

            //List<AddNotesMentee> List = new List<AddNotesMentee>();
            //int userid = Convert.ToInt32(Session["UserID"]);


            //// Here "MyDatabaseEntities " is dbContext, which is created at time of model creation.

            //using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            //{
            //    List = dc.AddNotesMentees.Where(a => a.UserID == userid).ToList();
            //}

            return Json(List, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public ActionResult AddorEditNotes(int id=0)
        {
            if(id==0)
            {
                return View(new AddNotesMentee());
            }
            else
            {
                using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
                {
                    return View(db.AddNotesMentees.Where(x => x.NoteID == id).FirstOrDefault<AddNotesMentee>());
                }
            }
        }

        [HttpPost]
        public ActionResult AddorEditNotes(AddNotesMentee mentee)
       {
            if (ModelState.IsValid)
            { 
                int userid = Convert.ToInt32(Session["UserID"]);
                
                using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
                {
                    if (mentee.NoteID == 0)
                    {
                        mentee.UserID = userid;
                        mentee.CreationDate = System.DateTime.Now;
                        db.AddNotesMentees.Add(mentee);
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        return Json(new { success = true, message = "Saved Successfully !!!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        mentee.UserID = userid;
                        mentee.CreationDate = System.DateTime.Now;
                        db.Entry(mentee).State = EntityState.Modified;
                        //var existinguser = db.AddNotesMentees.Find(userid);
                        //existinguser.Title = mentee.Title;
                        //existinguser.Description = mentee.Description;
                        //db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    
                }
                
            }
            else
            {
                return Json(new { success = false, message = "Error !!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteNotes(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                AddNotesMentee note = db.AddNotesMentees.Where(x => x.NoteID == id).FirstOrDefault<AddNotesMentee>();
                db.AddNotesMentees.Remove(note);
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewNotes(int id=0)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                return View(db.AddNotesMentees.Where(x => x.NoteID == id).FirstOrDefault<AddNotesMentee>());
            }
        }


        public ActionResult ViewAdvertisement()
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                return View(db.CourseAdvertisements.ToList());
            }
        }

        public ActionResult ViewFullAdvertisement(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                return View(db.CourseAdvertisements.Where(x => x.AdvertisementID == id).FirstOrDefault<CourseAdvertisement>());
            }
        }

        public ActionResult AddToSavedList(int id)
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                SavedList savel = db.SavedLists.Where(x => x.AdvertisementID == id && x.MenteeID == userid).FirstOrDefault();
                if (savel!=null)
                {
                    return Json(new { success = true, message = "Already added to saved list !!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    CourseAdvertisement cAdv = db.CourseAdvertisements.Where(x => x.AdvertisementID == id).FirstOrDefault<CourseAdvertisement>();

                    SavedList save = new SavedList();

                    save.ClassName = cAdv.ClassName;
                    save.CourseName = cAdv.CourseName;
                    save.MentorName = cAdv.MentorName;
                    save.Description = cAdv.Description;
                    save.AdvertisementID = id;
                    save.MenteeID = userid;
                    save.MentorID = cAdv.MentorID;
                    save.BatchesFull = cAdv.BatchesFull;
                    save.BatchesAvailable = cAdv.BatchesAvailable;
                    save.TotalStudents = cAdv.TotalStudents;
                    save.Fees = cAdv.Fees;
                    save.YearsOfExperience = cAdv.YearsOfExperience;
                    //save.DemoLec1 = cAdv.DemoLec1;
                    //save.DemoLec2 = cAdv.DemoLec2;
                    save.Field = cAdv.Field;
                    save.Location = cAdv.Location;
                    save.Address = cAdv.Address;
                    save.State = cAdv.State;
                    save.GitHub = cAdv.GitHubAccount;
                    save.City = cAdv.City;
                    save.SpokenLanguage = cAdv.SpokenLanguage;
                    db.SavedLists.Add(save);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Added to saved list successfully !!!" }, JsonRequestBehavior.AllowGet);
                }   
            }
        }

        public ActionResult ViewSavedList()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            List<SavedList> saveList = new List<SavedList>();
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {

                saveList = db.SavedLists.Where(a=>a.MenteeID == userid).OrderByDescending(r => r.SavedPostID).ToList();
                return View(saveList);
                //HttpResponseMessage response;
                //response = Request.CreateResponse(HttpStatusCode.OK, saveList);
                //return response;
            }
        }

        public ActionResult RemoveFromSavedList(int id)
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                SavedList savelist = db.SavedLists.Where(a => a.AdvertisementID == id && a.MenteeID == userid).FirstOrDefault();
                db.SavedLists.Remove(savelist);
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted from Saved list successfully !!!" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ApplyCourse(int id)
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var user = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                Application appln = db.Applications.Where(x => x.AdvertisementID == id && x.MenteeID == userid).FirstOrDefault();
                if (appln != null)
                {
                    ViewBag.status = "Applied";
                    ViewBag.Mentee = user.Username;
                    ViewBag.EmailID = user.EmailID;
                    ViewBag.Contact = user.ContactNo;
                    ViewBag.Age = user.Age;
                    ViewBag.Address = user.Address;
                    ViewBag.State = user.State;
                    ViewBag.GitHub = user.GitHubID;
                    ViewBag.Linkedin = user.LinkedInID;
                    ViewBag.MenteeMessage = appln.MenteeMessage;
                    ViewBag.MenteeExpectations = appln.MenteeExpectations;
                    ViewBag.MenteeWorkingStatus = appln.WorkingStatus;
                    ViewBag.MenteeBackground = appln.MenteeBackground;
                    ViewBag.Time = appln.AppliedTime;
                    return View();
                }
                else
                {
                    
                    ViewBag.Id = userid;
                    ViewBag.Mentee = user.Username;
                    ViewBag.EmailID = user.EmailID;
                    ViewBag.Contact = user.ContactNo;
                    ViewBag.Age = user.Age;
                    ViewBag.Address = user.Address;
                    ViewBag.State = user.State;
                    ViewBag.GitHub = user.GitHubID;
                    ViewBag.Linkedin = user.LinkedInID;
                    ViewBag.Message = id;
                    return View();
                }
            }
        }

        public ActionResult SubmitApppliedCourse(Application app)
        {
            if (ModelState.IsValid)
            {
                int userid = Convert.ToInt32(Session["UserID"]);
                using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
                {
                    var user = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                    Application app_form = new Application();
                    app_form.AdvertisementID = app.AdvertisementID;
                    int advid = app.AdvertisementID;
                    var advdetails = db.CourseAdvertisements.Find(advid);
                    app_form.MentorID = advdetails.MentorID;
                    app_form.MenteeID = userid;
                    app_form.MenteeName = user.Username;
                    app_form.MenteeEmailID = user.EmailID;
                    app_form.MenteeAge = user.Age;
                    app_form.MenteeContactNo = user.ContactNo;
                    app_form.MenteeAddress = user.Address;
                    app_form.MenteeState = user.State;
                    app_form.MenteeAddress = user.Address;
                    app_form.MenteeGithub = user.GitHubID;
                    app_form.MenteeLinkedin = user.LinkedInID;
                    app_form.MenteeMessage = app.MenteeMessage;
                    app_form.MenteeBackground = app.MenteeBackground;
                    app_form.MenteeExpectations = app.MenteeExpectations;
                    app_form.WorkingStatus = app.WorkingStatus;
                    app_form.ApplicationStatus = "Pending";
                    app_form.MentorEmailID = advdetails.EmailID;
                    app_form.AppliedTime = System.DateTime.Now;
                    app_form.MentorRemoveStatus = "Unremoved";
                    db.Applications.Add(app_form);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    applicationnotificationmentor(user.Username, user.State, advdetails.EmailID, advdetails.CourseName, advdetails.MentorName, advdetails.CreationDate);
                    return Json(new { success = true, message = "Applied !!!" }, JsonRequestBehavior.AllowGet);
                }   
            }
            else
            {
                return Json(new { success = "not", message = "Applied !!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewApppliedCourse()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var appliedcourse = db.Applications.Where(a => a.MenteeID == userid).ToList();
                return View(appliedcourse);
            }
        }

        public ActionResult ViewAppliedApplication(int id = 0)
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


        public ActionResult ApplyAppointment(int id)
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var user = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                Appointment appln = db.Appointments.Where(x => x.AdvertisementID == id && x.MenteeID == userid).FirstOrDefault();
                if (appln != null)
                {
                    ViewBag.status = "Applied";
                    ViewBag.Mentee = user.Username;
                    ViewBag.EmailID = user.EmailID;
                    ViewBag.Contact = user.ContactNo;
                    ViewBag.Age = user.Age;
                    ViewBag.Address = user.Address;
                    ViewBag.State = user.State;
                    ViewBag.GitHub = user.GitHubID;
                    ViewBag.Linkedin = user.LinkedInID;
                    ViewBag.AppointmentDate = appln.AppointmentDate;
                    ViewBag.AppointmentTime = appln.AppointmentTime;
                    ViewBag.MenteeWorkingStatus = appln.MenteeWorkingStatus;
                    ViewBag.Time = appln.AppointmentApplied;
                    return View();
                }
                else
                {

                    ViewBag.Id = userid;
                    ViewBag.Mentee = user.Username;
                    ViewBag.EmailID = user.EmailID;
                    ViewBag.Contact = user.ContactNo;
                    ViewBag.Age = user.Age;
                    ViewBag.Address = user.Address;
                    ViewBag.State = user.State;
                    ViewBag.GitHub = user.GitHubID;
                    ViewBag.Linkedin = user.LinkedInID;
                    ViewBag.Message = id;
                    return View();
                }
            }
        }


        public ActionResult SubmitAppointment(Appointment appoint)
        {
            if (ModelState.IsValid)
            {
                int userid = Convert.ToInt32(Session["UserID"]);
                using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
                {
                    var user = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                    Appointment app_form = new Appointment();
                    app_form.AdvertisementID = appoint.AdvertisementID;
                    int advid = appoint.AdvertisementID;
                    var advdetails = db.CourseAdvertisements.Find(advid);
                    app_form.MentorID = advdetails.MentorID;
                    app_form.MenteeID = userid;
                    app_form.MenteeName = user.Username;
                    app_form.MenteeEmailID = user.EmailID;
                    app_form.MenteeAge = user.Age;
                    app_form.MenteeContactNo = user.ContactNo;
                    app_form.MenteeAddress = user.Address;
                    app_form.MenteeState = user.State;
                    app_form.MenteeAddress = user.Address;
                    app_form.MenteeGithubID = user.GitHubID;
                    app_form.MenteeLinkedinID = user.LinkedInID;
                    app_form.AppointmentDate = appoint.AppointmentDate;
                    app_form.AppointmentStatus = "Pending";
                    app_form.MentorEmailID = advdetails.EmailID;
                    app_form.AppointmentApplied= System.DateTime.Now;
                    app_form.AppointmentRemoveStatus = "Unremoved";
                    app_form.StartTime = appoint.StartTime;
                    app_form.EndTime = appoint.EndTime;
                    app_form.MenteeWorkingStatus = appoint.MenteeWorkingStatus;
                    string startTime, endTime;
                    if(appoint.StartTime < 12)
                    {
                        startTime = appoint.StartTime + "am";
                    }
                    else
                    {
                        startTime = appoint.StartTime + "pm";
                    }

                    if(appoint.EndTime < 12)
                    {
                        endTime = appoint.EndTime + "am";
                    }
                    else
                    {
                        endTime = appoint.EndTime + "pm";
                    }

                    app_form.AppointmentTime = startTime + "-" + endTime;
                    db.Appointments.Add(app_form);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    //appointmentnotificationmentor(user.Username, user.State, advdetails.EmailID, advdetails.CourseName, advdetails.MentorName, advdetails.CreationDate);
                    return Json(new { success = true, message = "Appointment request sent !!!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = "not", message = "Error while Applied !!!" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ViewApprovedApplication()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            List<Application> applnList = new List<Application>();
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {

                applnList = db.Applications.Where(a => a.MenteeID == userid && a.ApplicationStatus == "Approve" ).ToList();
                return View(applnList);
            }
        }


        public ActionResult ViewPayFees(int id)
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var user = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                Fee feeappln = db.Fees.Where(x => x.AdvertisementID == id && x.MenteeID == userid).FirstOrDefault();
                var cdv = db.CourseAdvertisements.Where(x => x.AdvertisementID == id).FirstOrDefault();
                if (feeappln != null)
                {
                    ViewBag.status = "Paid";
                    ViewBag.Mentee = user.Username;
                    ViewBag.EmailID = user.EmailID;
                    ViewBag.Contact = user.ContactNo;
                    ViewBag.Age = user.Age;
                    ViewBag.Address = user.Address;
                    ViewBag.State = user.State;
                    ViewBag.GitHub = user.GitHubID;
                    ViewBag.Linkedin = user.LinkedInID;
                    ViewBag.Mode = feeappln.PaymentMode;
                    ViewBag.PaymentStatus = feeappln.PaymentStatus;
                    ViewBag.PaymentTime = feeappln.PaymentTime;
                    ViewBag.Fees = cdv.Fees;
                    return View();
                }
                else
                { 

                    ViewBag.Id = userid;
                    ViewBag.Fees = cdv.Fees;
                    ViewBag.Mentee = user.Username;
                    ViewBag.EmailID = user.EmailID;
                    ViewBag.Contact = user.ContactNo;
                    ViewBag.Age = user.Age;
                    ViewBag.Address = user.Address;
                    ViewBag.State = user.State;
                    ViewBag.GitHub = user.GitHubID;
                    ViewBag.Linkedin = user.LinkedInID;
                    ViewBag.Message = id;
                    return View();
                }
            }
        }

        [NonAction]
        public void applicationnotificationmentor(string menteeName, string State, string mentorEmail, string CourseName, string mentorName, DateTime creationDate)
        {
            string subject = "";
            string body = "";

            var fromEmail = new MailAddress("payaldhara05@gmail.com", "Find Your Mentor");
            var toEmail = new MailAddress(mentorEmail);
            var fromEmailPassword = "priyapayu";

            subject = "Mentee Application Request";
            body = "Name : "+menteeName+"<br>"+"Location : "+State+"<br>"+"Above mentee has requested to apply for the Course Advertisement that you have posted on "+creationDate+" for courses "+CourseName+" under Mentor "+mentorName+".<br> Please visit our portal for more infomation.<br><br><br><br>Thanks & Regards<br>Find Your Mentor";

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