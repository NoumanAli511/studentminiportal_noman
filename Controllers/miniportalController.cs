using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace studentminiportal.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MiniPortalController : ApiController
    {
        projectdatabaseEntities12 db = new projectdatabaseEntities12();

        [HttpGet]
        public HttpResponseMessage AdminLogin(string username, string password)
        {
            try
            {
                var admin = db.admin.FirstOrDefault(a => a.username == username && a.password == password);

                if (admin != null)
                {
                    var response = new
                    {
                        message = "Login successful",
                        admin = new
                        {
                            admin.admin_id,
                            admin.username,
                            admin.name,
                            admin.email,
                            admin.department,
                            admin.role,
                            admin.phone_no
                        }
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    var response = new
                    {
                        message = "Invalid username or password"
                    };

                    return Request.CreateResponse(HttpStatusCode.Unauthorized, response);
                }
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = "An error occurred",
                    error = ex.Message
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        //update Admin Profile
        [HttpPost]
        public HttpResponseMessage updateEmailAndPhoneNumber(string email,string phone_no)
        {
            try
            {
                var admin = db.admin.Select(s => s).FirstOrDefault();
                if (admin != null)
                {
                    admin.email = email;
                    admin.phone_no = phone_no;
                    db.SaveChanges();
                    return Request.CreateResponse("Email and Password Updated");
                }
                else
                {
                    return Request.CreateResponse("Admin not Founded");
                }

            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        [HttpPost]
        public HttpResponseMessage updateEmailAndPhoneNumberOfStudent(string email,string phone_no,int studentId)
        {
            try
            {
                var student = db.bothstudent.Where(s=>s.student_id==studentId).Select(s => s).FirstOrDefault();
                if (student != null)
                {
                    student.email = email;
                    student.phone_number= phone_no;
                    db.SaveChanges();
                    return Request.CreateResponse("Email and Password Updated");
                }
                else
                {
                    return Request.CreateResponse("Admin not Founded");
                }

            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }

        // Getting details of admin
        /*  [HttpGet]
          public HttpResponseMessage adminDetails()
          {
              try
              {
                  var Admin = db.admin.Select(s => new
                  {
                      s.admin_id,
                      s.username,
                      s.email,
                      s.password,
                      s.name,
                      s.department,
                      s.role,
                      s.phone_no
                  }).FirstOrDefault();

                  if(Admin != null)
                  {
                      return Request.CreateResponse(Admin);
                  }
                  else
                  {
                      return Request.CreateResponse();
                  }
              }catch(Exception cp)
              {
                  return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
              }
          }
        */
        [HttpGet]
        public HttpResponseMessage GetStudents(/*int graduationYear, string department, string section*/)
        {
            try
            {
                var students = db.Students.ToList();
                if (students.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, students);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No students found with the provided criteria.");
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }
        [HttpGet]
        public HttpResponseMessage FetchAllJobApplicationsPosted()
        {
            try
            {
                var AllJobs = db.JobApplications.Where(s => s.IsApproved == false).Select(s=>new
                {
                    student = new
                    {
                        s.bothstudent.student_id,
                        s.bothstudent.name,
                        s.bothstudent.arid_number,
                        s.bothstudent.email,
                        s.bothstudent.phone_number,
                        s.bothstudent.graduation_year,
                    },
                    jobDetails = new
                    {
                        s.ApplicationID,
                        s.JobTitle,
                        s.CompanyName,
                        s.JobDescription,
                        s.IsApproved,
                    }

                }).Distinct().ToList();
                if (AllJobs != null)
                {
                    return Request.CreateResponse(AllJobs);
                }
                else
                {
                    return Request.CreateResponse("No Jobs founded");
                }

            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        [HttpPost]
        public HttpResponseMessage UpdatingTheJobApplication(int Application_id,int status)
        {
            try
            {
                var JobApplication = db.JobApplications.Where(s => s.ApplicationID == Application_id && s.IsApproved==false).FirstOrDefault();
                if (JobApplication != null)
                {
                    if (status == 1)
                    {
                        JobApplication.IsApproved = true;
                    }
                    else
                    {
                        JobApplication.IsApproved = false;
                    }
                    int RowsEffected = db.SaveChanges();
                    if (RowsEffected > 0)
                    {
                        return Request.CreateResponse("Job Request Approved " + RowsEffected);
                    }
                    else
                    {
                        return Request.CreateResponse("Job Request Rejected " + RowsEffected);
                    }
                }
                else
                {
                    return Request.CreateResponse("Job Application not founded");
                }

            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }

    }
}
