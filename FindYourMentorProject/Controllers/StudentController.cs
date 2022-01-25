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
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebPages;

namespace FindYourMentorProject.Controllers
{

    [Authorize]            // don’t want to allow anonymous access to any of our action methods
    public class StudentController : Controller
    {
        HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
        public String userName;
        int userid;
        FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();

        public StudentController()
        {
            if(authCookie!=null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                userName = ticket.Name;
                userid = db.RegisterStudents.FirstOrDefault(x => x.EmailID == userName).UserID;
            } 
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public new ActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProfilePicture(HttpPostedFileBase file, string submitButton)                           //Done - (25-01-22)
        {
            if (userid != 0)
            {
                if (submitButton == "Upload")
                {
                    if (file != null && file.ContentLength > 0)
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
                            if (oldFileName != "/Image/defaultProfile1.jpg")
                            {
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
                    {
                        var user = dc.RegisterStudents.Single(c => c.UserID == userid);
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
                            }
                        }
                    }
                }
            }
            return RedirectToAction("PersonalInfo", "Student");
        }

        [HttpGet]
        public ActionResult PersonalInfo()             //Done - (25-01-22)
        {
            using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
            {
                var user = dc.RegisterStudents.Find(userid);
                return View(user);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePersonalInfo(RegisterStudent obj)          //Done - (25-01-22)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                obj.UserID = userid;
                db.Entry(obj).State = EntityState.Modified;
                db.Entry(obj).Property("Password").IsModified = false;             //Prevention of update null value error
                db.Entry(obj).Property("IsEmailVerified").IsModified = false;
                // 1st method - If you have an entity that you know already exists in the database but to which changes may have been made then you can tell the db to attach the entity and set its state to Modified.
                //var existinguser = db.RegisterStudents.Find(userid);  //2nd method - Traditional approach
                //existinguser.FirstName = obj.FirstName;
                //existinguser.LastName = obj.LastName;
                //existinguser.City = obj.City;
                //existinguser.State = obj.State;
                //existinguser.Pincode = obj.Pincode;
                //existinguser.ContactNo = obj.ContactNo;
                //existinguser.GitHubID = obj.GitHubID;
                //existinguser.LinkedInID = obj.LinkedInID;
                //existinguser.Description = obj.Description;
                //existinguser.Age = obj.Age;
                //existinguser.Address = obj.Address;
                db.Configuration.ValidateOnSaveEnabled = false;                 //To prevent error on validation of password and confirm password
                db.SaveChanges();
                return RedirectToAction("PersonalInfo", "Student");
            }
        }


        [HttpGet]
        public ActionResult ChangePass()         //Done - (25-01-22)
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(ChangePasswordModel pass)        //Done - (25-01-22)
        {
            string message = "";
            string message1 = "";
            if (ModelState.IsValid)
            {
                if (userid != 0)
                {
                    using (FindYourMentorProjectEntities dc = new FindYourMentorProjectEntities())
                    {
                        var user = dc.RegisterStudents.Single(c => c.UserID == userid);
                        if (string.Compare(Crypto.Hash(pass.OldPassword), user.Password) == 0)
                        {
                            if (string.Compare(pass.OldPassword, pass.NewPassword) == 0)
                            {
                                message1 = "Old password and new password cannot be same !";
                                ViewBag.Message1 = message1;
                                return View();
                            }
                            else
                            {
                                user.Password = Crypto.Hash(pass.NewPassword);
                                dc.Configuration.ValidateOnSaveEnabled = false;
                                dc.SaveChanges();
                                message = "Password changed successfully";
                            }
                        }
                        else
                        {
                            message1 = "Old Password is not correct";
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            ViewBag.Message = message;
            ViewBag.Message1 = message1;
            return View();
        }

        public ActionResult AddToSavedList(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                SavedList savel = db.SavedLists.Where(x => x.AdvertisementID == id && x.MenteeID == userid).FirstOrDefault();
                if (savel != null)
                {
                    return Json(new { success = true, message = "Already added to saved list !!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    CourseAdvertisement cAdv = db.CourseAdvertisements.Where(x => x.AdvertisementID == id).FirstOrDefault<CourseAdvertisement>();

                    SavedList save = new SavedList()
                    {
                        ClassName = cAdv.ClassName,
                        CourseName = cAdv.CourseName,
                        MentorName = cAdv.MentorName,
                        Description = cAdv.Description,
                        AdvertisementID = id,
                        MenteeID = userid,
                        MentorID = cAdv.MentorID,
                        BatchesFull = cAdv.BatchesFull,
                        BatchesAvailable = cAdv.BatchesAvailable,
                        TotalSeats = cAdv.TotalSeats,
                        SeatsOccupied = cAdv.SeatsOccupied,
                        Fees = cAdv.Fees,
                        YearsOfExperience = cAdv.YearsOfExperience,
                        Field = cAdv.Field,
                        SpokenLanguage1 = cAdv.SpokenLanguage1,
                        Address = cAdv.Address,
                        State = cAdv.State,
                        GitHub = cAdv.GitHubAccount,
                        City = cAdv.City,
                        SpokenLanguage2 = cAdv.SpokenLanguage2,
                        Mode = cAdv.Mode,
                        Duration = cAdv.Duration
                    };
                    db.SavedLists.Add(save);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Added to saved list successfully !!!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult ViewSavedList(string searchby, string search)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                if (searchby == "CourseName")
                {
                    return View(db.SavedLists.Where(a => a.MenteeID == userid && a.CourseName.StartsWith(search) || search == null).OrderByDescending(r => r.SavedPostID).ToList());
                }
                else if (searchby == "MentorName")
                {
                    return View(db.SavedLists.Where(a => a.MenteeID == userid && a.MentorName.StartsWith(search) || search == null).OrderByDescending(r => r.SavedPostID).ToList());
                }
                else
                {
                    return View(db.SavedLists.Where(a => a.MenteeID == userid && a.ClassName.StartsWith(search) || search == null).OrderByDescending(r => r.SavedPostID).ToList());
                }
            }
        }

        public ActionResult RemoveFromSavedList(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                SavedList savelist = db.SavedLists.Where(a => a.AdvertisementID == id && a.MenteeID == userid).FirstOrDefault();
                db.SavedLists.Remove(savelist);
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted from Saved list successfully !!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Notes()
        {
            return View();
        }


        [Authorize]
        public JsonResult NotesData()
        {
            FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<AddNotesMentee> List = db.AddNotesMentees.Where(a => a.UserID == userid).ToList();
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
        [ValidateAntiForgeryToken]
        public ActionResult AddorEditNotes(AddNotesMentee mentee)
       {
            if (ModelState.IsValid)
            { 
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
        [ValidateAntiForgeryToken]
        public ActionResult AddorEditNotesExternal(AddNotesMentee mentee)
        {
            if (ModelState.IsValid)
            {
                using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
                { 
                        mentee.UserID = userid;
                        mentee.CreationDate = System.DateTime.Now;
                        db.Entry(mentee).State = EntityState.Modified;
                        db.SaveChanges();
                }
            }
            return RedirectToAction("Notes", "Student");
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

        [HttpGet]
        public ActionResult ViewNotes(int id=0)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                return View(db.AddNotesMentees.Where(x => x.NoteID == id).FirstOrDefault<AddNotesMentee>());
            }
        }

        [HttpGet]
        public ActionResult ViewVideos()
        {
            return View();
        }


        public ActionResult ViewAdvertisement()
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                AdvertisementBookmarkViewModel AdvSaveModel = new AdvertisementBookmarkViewModel();
                AdvSaveModel.CourseAdvertisement = db.CourseAdvertisements.ToList();
                AdvSaveModel.SavedList = db.SavedLists.ToList();
                return View(AdvSaveModel);
            }
        }

        public ActionResult ViewFullAdvertisement(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                return View(db.CourseAdvertisements.Where(x => x.AdvertisementID == id).FirstOrDefault<CourseAdvertisement>());
            }
        }

        [NonAction]
        public void CheckAdvertisement(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                SavedList saveAdv = db.SavedLists.Where(a => a.AdvertisementID == id).FirstOrDefault();
                if (saveAdv == null)
                {
                    CourseAdvertisement cadv = db.CourseAdvertisements.Where(a => a.AdvertisementID == id).FirstOrDefault();
                    db.CourseAdvertisements.Remove(cadv);
                    db.SaveChanges();
                }
            }
        }


        public ActionResult ApplyCourse(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var user = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                Application appln = db.Applications.Where(x => x.AdvertisementID == id && x.MenteeID == userid).FirstOrDefault();
                if (appln != null)
                {
                    ViewBag.status = "Applied";
                    ViewBag.User = user;
                    ViewBag.Application = appln;
                    return View();
                }
                else
                {
                    ViewBag.Id = userid;
                    ViewBag.User = user;
                    ViewBag.Message = id;   //AdvertisementID
                    return View();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitApppliedCourse(Application app)
        {
            if (ModelState.IsValid)
            {
                using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
                {
                    var user = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                    Application app_form = new Application();
                    app_form.AdvertisementID = app.AdvertisementID;
                    int advid = app.AdvertisementID;
                    var advdetails = db.CourseAdvertisements.Find(advid);
                    app_form.MentorID = advdetails.MentorID;
                    app_form.MenteeID = userid;
                    app_form.MenteeName = user.FirstName;
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
                    app_form.ClassName = advdetails.ClassName;
                    app_form.CourseName = advdetails.CourseName;
                    app_form.MentorName = advdetails.MentorName;
                    app_form.AppliedTime = System.DateTime.Now;
                    app_form.MentorRemoveStatus = "Unremoved";
                    db.Applications.Add(app_form);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Applied !!!" }, JsonRequestBehavior.AllowGet);
                }   
            }
            else
            {
                return Json(new { success = "not", message = "Applied !!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewApppliedCourse(string filters)
        {
            FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();
            if (filters == "Approve")
            {
                return View(db.Applications.Where(a => a.MenteeID == userid && a.ApplicationStatus == "Approve").OrderByDescending(r => r.ApplicationID).ToList());
            }
            else if (filters == "Reject")
            {
                return View(db.Applications.Where(a => a.MenteeID == userid && a.ApplicationStatus == "Reject").OrderByDescending(r => r.ApplicationID).ToList());
            }
            else
            {
                return View(db.Applications.Where(a => a.MenteeID == userid).OrderByDescending(r => r.ApplicationID).ToList());
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
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var user = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                Appointment appln = db.Appointments.Where(x => x.AdvertisementID == id && x.MenteeID == userid).FirstOrDefault();
                if (appln != null)
                {
                    ViewBag.status = "Applied";
                    ViewBag.User = user;
                    ViewBag.Appointment = appln;
                    return View();
                }
                else
                {

                    ViewBag.Id = userid;
                    ViewBag.User = user;
                    ViewBag.Message = id;   //AdvertisementID
                    return View();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitAppointment(Appointment appoint)
        {
            if (ModelState.IsValid)
            {
                using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
                {
                    var user = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                    Appointment app_form = new Appointment();
                    app_form.AdvertisementID = appoint.AdvertisementID;
                    int advid = appoint.AdvertisementID;
                    var advdetails = db.CourseAdvertisements.Find(advid);
                    app_form.MentorID = advdetails.MentorID;
                    app_form.MenteeID = userid;
                    app_form.MenteeName = user.FirstName;
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
                    app_form.ClassName = advdetails.ClassName;
                    app_form.CourseName = advdetails.CourseName;
                    app_form.MentorName = advdetails.MentorName;
                    app_form.MenteeWorkingStatus = appoint.MenteeWorkingStatus;
                    app_form.AppointmentMode = appoint.AppointmentMode;
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
                    return Json(new { success = true, message = "Appointment request sent !!!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = "not", message = "Error while Applied !!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewApppliedAppointment(string filters)
        {
            FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();
            if (filters == "Confirmed")
            {
                return View(db.Appointments.Where(a => a.MenteeID == userid && a.AppointmentStatus == "Confirmed").OrderByDescending(r => r.AppointmentID).ToList());
            }
            else if (filters == "Cancelled")
            {
                return View(db.Appointments.Where(a => a.MenteeID == userid && a.AppointmentStatus == "Cancelled").OrderByDescending(r => r.AppointmentID).ToList());
            }
            else
            {
                return View(db.Appointments.Where(a => a.MenteeID == userid).OrderByDescending(r => r.AppointmentID).ToList());
            }
        }

        public ActionResult ViewAppliedAppointmentData(int id = 0)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                return View(db.Appointments.Where(a => a.AppointmentID == id).FirstOrDefault<Appointment>());
            }
        }


        public ActionResult ViewApprovedApplication()
        {
            return View();
        }

        public ActionResult ViewApprovedApplicationData()
        {
            FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<Application> applnList = db.Applications.Where(a => a.MenteeID == userid && a.ApplicationStatus == "Approve").ToList();
            return Json(applnList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ViewPayFees(int id)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                var user = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                Fee feeappln = db.Fees.Where(x => x.AdvertisementID == id && x.MenteeID == userid).FirstOrDefault();
                var cdv = db.CourseAdvertisements.Where(x => x.AdvertisementID == id).FirstOrDefault();
                if (feeappln != null)
                {
                    ViewBag.status = "Paid";
                    ViewBag.User = user;
                    ViewBag.Fee = feeappln;
                    return View();
                }
                else
                { 

                    ViewBag.Id = userid;
                    ViewBag.User = user;
                    ViewBag.Message = id;   //AdvertisementID
                    return View();
                }
            }
        }

        public ActionResult SubmitFeeApplication(Fee feeApln)
        {
            if (ModelState.IsValid)
            {
                int userid = Convert.ToInt32(Session["UserID"]);
                using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
                {
                    var user = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                    feeApln.MenteeID = userid;
                    feeApln.MenteeName = user.FirstName;
                    feeApln.MenteeState = user.State;
                    feeApln.MenteeEmailID = user.EmailID;
                    feeApln.MenteeGitHub = user.GitHubID;
                    feeApln.MenteeLinkedIn = user.LinkedInID;
                    feeApln.MenteeContactNo = user.ContactNo;
                    feeApln.MenteeAge = user.Age;

                    int advid = feeApln.AdvertisementID;
                    var advDetails = db.CourseAdvertisements.Where(a => a.AdvertisementID == advid).FirstOrDefault();

                    feeApln.MentorID = advDetails.MentorID;
                    feeApln.MentorEmailID = advDetails.EmailID;
                    feeApln.MentorName = advDetails.MentorName;
                    feeApln.CourseName = advDetails.CourseName;
                    feeApln.ClassName = advDetails.ClassName;

                    feeApln.PaymentTime = System.DateTime.Now;
                    feeApln.Fees = advDetails.Fees;
                    feeApln.PaymentStatus = "Pending";
                    feeApln.AdmissionStatus = "Pending";
                    feeApln.AmountEntered = advDetails.Fees;
                    feeApln.StatusCounterFee = 0;
                   
                    if(feeApln.PaymentMode == "Online")
                    {
                            db.Fees.Add(feeApln);
                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.SaveChanges();
                            Thread.Sleep(4000);
                            return Json(new { success = "yes", message = "Done !!!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.Fees.Add(feeApln);
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        Thread.Sleep(3000);
                        return Json(new { success = "yes", message = "Done !!!" }, JsonRequestBehavior.AllowGet);
                    }   
                }
            }
            else
            {
                return Json(new { success = "not", message = "Error while Applied !!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AdmissionStatus()
        {
            return View();
        }

        public ActionResult AdmissionStatusData()
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            FindYourMentorProjectEntities db = new FindYourMentorProjectEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<Fee> FeeList = db.Fees.Where(a => a.MenteeID == userid).ToList();
            return Json(FeeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FeeProcessed()
        {
            return View();
        }

        [NonAction]
        public void applicationnotificationmentor(string menteeName, string State, string mentorEmail, string CourseName, string mentorName, DateTime creationDate)
        {
            string subject = "";
            string body = "";

            var fromEmail = new MailAddress("payaldhara05@gmail.com", "Find Your Mentor");
            var toEmail = new MailAddress(mentorEmail);
            var fromEmailPassword = "****";

            subject = "Mentee Application Request";
            body = "Name : " + menteeName + "<br>" + "Location : " + State + "<br>" + "Above mentee has requested to apply for the Course Advertisement that you have posted on " + creationDate + " for courses " + CourseName + " under Mentor " + mentorName + ".<br> Please visit our portal for more infomation.<br><br><br><br>Thanks & Regards<br>Find Your Mentor";

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

        public ActionResult AdmissionReceipt(int id=0)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                return View(db.Fees.Where(a => a.FeesID == id).FirstOrDefault<Fee>());
            }
        }

        public ActionResult ShowFullAdvertisement(int id = 0)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                BigModel bigmodel = new BigModel();
                bigmodel.CourseAdvertisement = db.CourseAdvertisements.Where(x => x.AdvertisementID == id).FirstOrDefault<CourseAdvertisement>();
                bigmodel.Feedback = db.Feedbacks.Where(a => a.AdvertisementID == id).OrderByDescending(r => r.FeedbackID).ToList();
                bigmodel.Comment = db.Comments.Where(a => a.AdvertisementID == id).OrderByDescending(r => r.CommentID).ToList();
                bigmodel.ReplyToComment = db.ReplyToComments.Where(a => a.AdvertisementID == id).OrderByDescending(r => r.ReplyID).ToList();

                ViewBag.ID = id;

                return View(bigmodel);
            }
        }

        [HttpPost]
        public ActionResult postFeedback(Feedback data)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                RegisterStudent stud = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                data.Username = stud.FirstName + " " + stud.LastName;
                data.UserID = userid;
                data.commentedOn = System.DateTime.Now;
                db.Feedbacks.Add(data);
                db.SaveChanges();
                return RedirectToAction("ShowFullAdvertisement", new { id = data.AdvertisementID });
            }
        }

        [HttpPost]
        public ActionResult postComment(Comment data)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                RegisterStudent stud = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                data.mentorName = stud.FirstName + " " + stud.LastName;
                data.MenteeID = userid;
                data.commentedDate = System.DateTime.Now;
                db.Comments.Add(data);
                db.SaveChanges();
                return RedirectToAction("ShowFullAdvertisement", new { id = data.AdvertisementID });
            }
        }

        [HttpPost]
        public ActionResult postReply(ReplyToComment data, String val)
        {
            using (FindYourMentorProjectEntities db = new FindYourMentorProjectEntities())
            {
                RegisterStudent stud = db.RegisterStudents.Where(a => a.UserID == userid).FirstOrDefault();
                data.MentorName = stud.FirstName + " " + stud.LastName;
                data.MenteeID = userid;
                data.Message = val;
                db.ReplyToComments.Add(data);
                db.SaveChanges();
                return RedirectToAction("ShowFullAdvertisement", new { id = data.AdvertisementID });
            }
        }
    }
}