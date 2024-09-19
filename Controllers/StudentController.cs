using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace studentminiportal.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class StudentController : ApiController
    {
        projectdatabaseEntities12 db = new projectdatabaseEntities12();
        [HttpGet]

        public HttpResponseMessage Login(string aridno, string password)
        {
            try
            {
                var std = db.bothstudent.Where(p => p.arid_number == aridno && p.password == password).Select(s => new
                {
                    s.student_id,
                    s.name,
                    s.address,
                    s.graduation_year,
                    s.status,
                    s.email,
                    s.section,
                    s.gender,
                    s.arid_number,
                    s.phone_number,
                    s.department

                }).FirstOrDefault();
                if (std!=null)
                {
                   
                    return Request.CreateResponse(HttpStatusCode.OK, std);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Arid number and password is incorrect");

                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage updateStudent(string email, string phone_no, int st_id)
        {
            try
            {
                var student = db.bothstudent.Where(s => s.student_id==st_id).FirstOrDefault();
                if (student != null)
                {
                    
                   student.email = email;
                   student.phone_number= phone_no;
                    db.SaveChanges();
                    return Request.CreateResponse("Email and Password Updated");
                }
                else
                {
                    return Request.CreateResponse("Student not Founded");
                }

            }
            catch (Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        ///Searching OF students
        [HttpGet]
        public HttpResponseMessage GetStudents(string graduationYear, string department, string section)
        {
            try
            {
                string year = graduationYear.ToString();

               // var studentsByYear = db.bothstudent.Where(s => s.graduation_year.Contains(year)).Select(s => s).Distinct().ToList();
                var students = db.bothstudent
                                 .Where(s => s.graduation_year == year && s.department == department && s.section == section)
                                 .Select(s => new { 
                                    s.name,
                                    s.arid_number,
                                    s.email,
                                    s.phone_number,
                                    s.department,
                                    s.semester,
                                    s.section,
                                    s.address,
                                    s.graduation_year,
                                    s.status,
                                    s.gender
                                 })
                                 .Distinct()
                                 .ToList();

                if (students != null && students.Count > 0)
                {
                    return Request.CreateResponse(students);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No students found with the provided criteria.");
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }
            
        [HttpGet]
        public HttpResponseMessage GetStudentsByName(string name)
        {
            try
            {
                var students = db.bothstudent
                                 .Where(s => s.name.ToLower().Contains(name.ToLower()))
                                 .Select(s => new
                                 {
                                     s.name,
                                     s.arid_number,
                                     s.email,
                                     s.phone_number,
                                     s.department,
                                     s.semester,
                                     s.section,
                                     s.address,
                                     s.graduation_year,
                                     s.status,
                                     s.gender
                                 })
                                 .Distinct()
                                 .ToList();

                if (students != null && students.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, students);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No students found with the provided name.");
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }


        //        [HttpGet]
        //    public HttpResponseMessage GetAlumni(int graduationYear, string department, string section)
        //      {
        //        try
        //      {
        //        var students = db.Alumni_.Where(s => s.graduation_year == graduationYear && s.department == department && s.section == section)
        //                       .Select(p => new
        //                     {
        //                       p.alumni_id,
        //                     p.name,
        //                   p.arid_number,
        //                 p.address,
        //               p.phone_number,
        //             p.email
        //       }).ToList();
        // if (students.Count > 0)
        //{
#pragma warning disable// CS02110 // Variable is assigned but its value is never used
        //  int w = 0;
#pragma warning restore // CS02110 // Variable is assigned but its value is never used
        //   return Request.CreateResponse(HttpStatusCode.OK, students);
        // }
        //else
        //{
        //  return Request.CreateResponse(HttpStatusCode.NotFound, "No students found with the provided criteria.");
        //}
        // }
        //catch (Exception e)
        //{
        //     return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
        //}

        //        }




        [HttpPost]
        public HttpResponseMessage CreateJob(string title,string company,string description,int student_id)
        {
            try
            {
                // Assuming studentID is sent as part of the request
                //yahan py foreign key ka concept hai
                var student = db.bothstudent.Where(s => s.student_id == student_id && s.status== "alumni").FirstOrDefault();
                if (student != null)
                {
                    JobApplications newJob = new JobApplications
                    {
                        JobTitle = title,
                        CompanyName = company,
                        JobDescription = description,
                        IsApproved = false,
                        bothstudent=student
                    };
                    db.JobApplications.Add(newJob);
                    db.SaveChanges();
                    return Request.CreateResponse("Job Posted");
                }
                else
                {
                    return Request.CreateResponse("Student not founded");
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        /*Get All Jobs Posted*/
        [HttpGet]
        public HttpResponseMessage FetchAllJobsApproved()
        {
            try
            {   
                    var Alljobs = db.JobApplications.Where(s=>s.IsApproved==true).Select(s => new
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
                    if (Alljobs != null)
                    {
                        return Request.CreateResponse(Alljobs);
                    }
                    else
                    {
                        return Request.CreateResponse("Jobs not Founded");
                    }
                



            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        
        // fetching all jobs posted by the Alumni
        [HttpGet]
        public HttpResponseMessage FetchAllJobsApplications()
        {
            try
            {
                var allApplications = db.JobApplications.Where(s => s.IsApproved == false ).Select(s=>new { 
                    s.JobTitle,
                    s.ApplicationID,
                    s.IsApproved,
                    s.JobDescription,
                    s.CompanyName,
                    studentInfo = new
                    {
                        s.bothstudent.email,
                        s.bothstudent.student_id,
                        s.bothstudent.phone_number,
                        s.bothstudent.name,
                        s.bothstudent.arid_number,
                        s.bothstudent.department
                    }
                }).Distinct().ToList();
                if (allApplications == null)
                {
                    return Request.CreateResponse("Jobs not Founded");
                }
                return Request.CreateResponse(allApplications);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }
        

        ///////////////// surveys for students
        [HttpGet]
        public HttpResponseMessage FetchAllSurveysForStudent(string aridNumber)
        {
            try
            {
                var Surveys = new object();
                var student = db.bothstudent.Where(s => s.arid_number == aridNumber).FirstOrDefault();
                if (student != null)
                {
                    if (student.status == "current")
                    {
                        Surveys = db.AssignedSurvey.Where(s => s.studentType == student.status && s.Semester == student.semester && s.Section == student.section && s.Department == student.department).Select(s => new
                        {
                            s.createsurvey.SurveyID,
                            s.createsurvey.SurveyTitle,
                            s.createsurvey.StartDate,
                            s.createsurvey.EndDate
                        }).Distinct().ToList();
                    }
                    else
                    {
                        int year = int.Parse(student.graduation_year);
                        Surveys = db.AssignedSurvey.Where(s => s.studentType == student.status && s.GraduationYear == year && s.City==student.address && s.Technology == s.Technology ).Select(s => new
                        {   s.createsurvey.SurveyID,
                            s.createsurvey.SurveyTitle,
                            s.createsurvey.StartDate,
                            s.createsurvey.EndDate
                        }).Distinct().ToList();
                    }
                    return Request.CreateResponse(Surveys);
                }
                else
                {
                    return Request.CreateResponse("Student  not founded");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }

        [HttpGet]
        public HttpResponseMessage FetchQuestionsonSurvey(int surveyId)
        {
            try
            {
                var survey = db.createsurvey.Where(s => s.SurveyID == surveyId).FirstOrDefault();
                if (survey != null)
                {
                    var questionsList =  db.survey_questions.Where(s => s.createsurvey.SurveyID == survey.SurveyID).Select(s => new
                    {
                        s.QuestionID,
                        s.QuestionText,
                        s.Option1,
                        s.Option2,
                        s.Option3,
                    }).Distinct().ToList();
                    if (questionsList.Count > 0)
                    {
                        return Request.CreateResponse(questionsList);
                    }
                    else
                    {
                        return Request.CreateResponse("Questions not founded");
                    }
                }
                else
                {
                    return Request.CreateResponse("Survey not founded");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        [HttpPost]
        public HttpResponseMessage PostOptionOfQuestion([FromBody] List<QuestionAndOptions> QuestionsAndOptionsList, int student_id, int surveyId)
        {
            try
            {
                var bothStudentFinding = db.bothstudent.Where(s => s.student_id == student_id).FirstOrDefault();
                if (bothStudentFinding == null)
                {
                    return Request.CreateResponse("Student not founded");
                }
                if (QuestionsAndOptionsList == null || !QuestionsAndOptionsList.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Questions and Options List must not be empty.");
                }

                foreach (var item in QuestionsAndOptionsList)
                {
                    var surveyQuestionOptions = new SurveyQuestionOptions
                    {
                        SurveyQuestionID = item.questionId,
                        OptionText = item.optionText
                    };
                    db.SurveyQuestionOptions.Add(surveyQuestionOptions);
                    db.SaveChanges(); // Save to get the OptionID

                    var surveyResponse = new SurveyResponse
                    {
                        SelectedOptionID = surveyQuestionOptions.OptionID,
                        bothstudent = bothStudentFinding,
                        SurveyQuestionID = surveyQuestionOptions.SurveyQuestionID,
                        SurveyID = surveyId
                    };
                    db.SurveyResponse.Add(surveyResponse);
                }

                int rowsEffected = db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, $"Questions and options added. Rows affected: {rowsEffected}");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message + (ex.InnerException != null ? ": " + ex.InnerException.Message : ""));
            }
        }



        [HttpPost]
        public HttpResponseMessage SubmitSurveyResponse(int surveyId, int questionId, int selectedOptionId, int studentId)
        {
            try
            {

                // Check if the survey, question, option, and student exist
                var survey = db.createsurvey.FirstOrDefault(s => s.SurveyID == surveyId);
                var question = db.survey_questions.FirstOrDefault(q => q.QuestionID == questionId);
                var option = db.SurveyQuestionOptions.FirstOrDefault(o => o.OptionID == selectedOptionId);
                var student = db.bothstudent.FirstOrDefault(s => s.student_id == studentId);

                if (survey == null || question == null || option == null || student == null)
                {
                    return Request.CreateResponse("Invalid survey, question, option, or student.");
                }

                // Save the response
                var response = new SurveyResponse
                {
                    SurveyID = surveyId,
                    SurveyQuestionID = questionId,
                    bothstudent = student,
                    SelectedOptionID = selectedOptionId,
               //     ResponseDate = DateTime.Now
                };
                db.SurveyResponse.Add(response);
                db.SaveChanges();

                return Request.CreateResponse("Response submitted successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse("An error occurred: " + ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage FetchSurveyResponses(int surveyId)
        {
            try
            {
                var responses = db.SurveyResponse
                    .Where(r => r.SurveyID == surveyId)
                    .Select(r => new
                    {
                        r.SurveyResponseID,
                        r.SurveyID,
                        r.SurveyQuestionID,
                        r.bothstudent.student_id,
                        r.SelectedOptionID,
                    //    r.ResponseDate
                    })
                    .ToList();

                return Request.CreateResponse(responses);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse("An error occurred: " + ex.Message);
            }
        }







        ////Comment Section
        [HttpPost]
        public HttpResponseMessage AddComment(int eventId, int studentId, string comment)
        {
            try
            {
                var student = db.bothstudent.FirstOrDefault(s => s.student_id == studentId);
                if (student == null)
                {
                    return Request.CreateResponse(student);
                }

                var eventExists = db.Events.FirstOrDefault(e => e.event_id == eventId);
                if (eventExists==null)
                {
                    return Request.CreateResponse(student);
                }

                var studentFinding = db.bothstudent.Where(s => s.student_id == studentId).FirstOrDefault();
                if (studentFinding == null)
                {
                    return Request.CreateResponse("student not founded");
                }
                var newComment = new commentedOnEvent
                {
                    Events=eventExists,
                    bothstudent=studentFinding,
                    commentText=comment,
                    date = DateTime.Now
                };
                db.commentedOnEvent.Add(newComment);
                 int RowsEffected=db.SaveChanges();
                if (RowsEffected > 1)
                {
                    return Request.CreateResponse("Succesfully Comment");
                }
                return Request.CreateResponse("Not inserted");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse("An error occurred: " + ex.Message);
            }
        }

        //getting all comments=
        [HttpGet]
        public HttpResponseMessage FetchingAllCommits(int eventId)
        {
            try
            {
                var eventFinding = db.Events.Where(s => s.event_id == eventId).FirstOrDefault();
                if (eventFinding == null)
                {
                    return Request.CreateResponse("Event Not founded");
                }
                var EventsComments = db.commentedOnEvent.Where(s => s.Events.event_id == eventFinding.event_id).Select(s => new
                {
                    s.commentText,
                    s.bothstudent.arid_number,
                    s.bothstudent.student_id,
                    s.id,
                    s.Events.event_id,
                    s.date,
                    s.bothstudent.name
                }).Distinct().ToList();
                if (EventsComments == null)
                {
                    return Request.CreateResponse("Comments not founded");
                }
                return Request.CreateResponse(EventsComments);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage MarkAsViewed(int eventId,int student_id)
        {
            try
            {
                var eventFinding = db.Events.Where(s => s.event_id == eventId).FirstOrDefault();
                if (eventFinding == null)
                {
                    return Request.CreateResponse("Event Not founded");
                }   
                var studentFinding = db.bothstudent.Where(s => s.student_id == student_id).FirstOrDefault();
                if (studentFinding == null)
                {
                    return Request.CreateResponse("Student not founded");
                }
                var ViewedEventFinding = db.ViewedEvents.Where(s => s.bothstudent.student_id == student_id && s.Events.event_id == eventId).FirstOrDefault();
                if (ViewedEventFinding != null) {
                    return Request.CreateResponse("View Added Already");
                }
                var ViewedEvent = new ViewedEvents
                {
                    bothstudent=studentFinding,
                    Events=eventFinding
                };
                db.ViewedEvents.Add(ViewedEvent);
                db.SaveChanges();
                return Request.CreateResponse("Viewed Successfully");
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }
        [HttpGet]
        public HttpResponseMessage fetchAllViewedEvent(int eventId)
        {
            try
            {
                var EventFiding = db.Events.Where(s => s.event_id == eventId).FirstOrDefault();
                if (EventFiding == null)
                {
                    return Request.CreateResponse("Event not founded");
                }
                var ViewedPeople = db.ViewedEvents.Where(s => s.Events.event_id == EventFiding.event_id).Select(s => new
                {
                    s.id,
                    s.bothstudent.name,
                    s.bothstudent.arid_number,
                    s.bothstudent.student_id,
                }).Distinct().ToList();
                if (ViewedPeople == null)
                {
                    return Request.CreateResponse("No Views");
                }
                return Request.CreateResponse(ViewedPeople);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }
    }
}
