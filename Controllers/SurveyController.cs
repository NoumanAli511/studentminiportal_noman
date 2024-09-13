using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
namespace studentminiportal.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SurveyController : ApiController
    {

        projectdatabaseEntities10 db = new projectdatabaseEntities10();
        [HttpPost]
        public HttpResponseMessage Addsurvey(createsurvey createsurvey)
        {
            try
            {
                db.createsurvey.Add(createsurvey);
                db.SaveChanges();
                return Request.CreateResponse(createsurvey);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }
        
        //Select Population
        [HttpPost]
        public HttpResponseMessage ManagePopulation(StudentSurveyPopulation surveyPopulation)
        {
            try
            {
                if (surveyPopulation != null)
                {
                    var survey = db.createsurvey.Where(s => s.SurveyID == surveyPopulation.surveyID).FirstOrDefault();
                    if (survey == null) return Request.CreateResponse("Survey Not founded");
                    var studentType = surveyPopulation.studentType;
                    if (studentType == "current")
                    {
                        foreach (var degree in surveyPopulation.degree)
                        {
                            foreach (var semester in surveyPopulation.semester)
                            {
                                foreach (var section in surveyPopulation.section)
                                {
                                    AssignedSurvey population = new AssignedSurvey
                                    {
                                        Department = degree,
                                        Semester = semester,
                                        Section = section,
                                        Gender = surveyPopulation.gender,
                                        City = null, // Set as necessary surveyPopulation.City.First()
                                        Technology = null, // Set as necessary
                                        GraduationYear = 0, // Set as necessary
                                        studentType = studentType,
                                        createsurvey = survey,
                                    };
                                    db.AssignedSurvey.Add(population);
                                    db.SaveChanges();
                                    // Add the assigned survey to the database context and save changes
                                }
                            }
                        }
                    }
                    else
                    {
                        // insert for the alumni value
                        foreach (var city in surveyPopulation.address)
                        {
                            foreach (var technology in surveyPopulation.Technology)
                            {
                                foreach (var graduationYear in surveyPopulation.graduation)
                                {
                                    AssignedSurvey assignedSurvey = new AssignedSurvey
                                    {
                                        Department = surveyPopulation.degree.First(), // Set as necessary
                                        Semester = null, // Set as necessary
                                        Section = null, // Set as necessary
                                        Gender = surveyPopulation.gender,
                                        City = city,
                                        Technology = technology,
                                        GraduationYear = graduationYear,
                                        createsurvey = survey,
                                        studentType = studentType,
                                    };

                                    // Add the assigned survey to the database context and save changes
                                    db.AssignedSurvey.Add(assignedSurvey);
                                    db.SaveChanges();
                                }
                            }
                        }

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, "Data successfully inserted");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid data");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error: " + ex.Message);
            }
        }

        // Fetching Id by name
      [HttpGet]
        public HttpResponseMessage FetchingId(string title)
        {
            try
            {
                var id = db.createsurvey.Where(s => s.SurveyTitle.Trim().Equals(title.Trim())).FirstOrDefault();
                if (id != null)
                {
                    return Request.CreateResponse(id.SurveyID);
                }
                else
                {
                    return Request.CreateResponse(0);
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }


        [HttpPost]
      
        public HttpResponseMessage CreateSurvey(List<QuestionAdding> Questions)
        {
            try
            {
                survey_questions survey_question = new survey_questions();
                if (Questions != null)
                {
                    foreach(var question in Questions)
                    {
                        var surveyFinding = db.createsurvey.Where(s => s.SurveyID == question.surveyID).FirstOrDefault();
                        if (surveyFinding == null)
                        {
                            return Request.CreateResponse("Survey not fonded");
                        }
                        survey_question.QuestionText = question.questionText;
                        survey_question.Option1 = question.option1;
                        survey_question.Option2 = question.option2;
                        survey_question.Option3 = question.option3;
                        survey_question.createsurvey = surveyFinding;
                        db.survey_questions.Add(survey_question);
                        db.SaveChanges();
                    }
                }
                else
                {
                    return Request.CreateResponse("Questions must not be empty");
                }
                return Request.CreateResponse("Questions Added");
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }

        }



        //Dropdown of the Surveys on the admin side
        [HttpGet]
        public HttpResponseMessage FetchAllSurveys()
        {
            try
            {
                var listOfSurveysConducted = db.createsurvey.Select(s => new
                {
                    surveyID = s.SurveyID,
                    title=s.SurveyTitle,
                    startDate = s.StartDate,
                    endDate= s.EndDate.ToString()
                }).Distinct().ToList();
                if (listOfSurveysConducted.Count > 0)
                {
                    return Request.CreateResponse(listOfSurveysConducted);
                }
                else
                {
                    return Request.CreateResponse("Not founded any survey");
                }
            }
            catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }

        /*[HttpPost]
        public HttpResponseMessage DeleteSurvey(int surveyId)
        {

        }
        */

        [HttpPost]
        public HttpResponseMessage SurveyCurrentStudent(List<surveyCurrentStudent> data)
        {
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The survey list cannot be null.");
            }

            try
            {
                foreach (var item in data)
                {
                    // If you're sending the degree, semester, and section as strings, split them here
                    item.degree = item.degree.Split(',')[0];
                    item.semester = item.semester.Split(',')[0];
                    item.section = item.section.Split(',')[0];

                    db.surveyCurrentStudent.Add(item);
                    db.SaveChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK, "Entries successfully added.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // fetching all the surveys as the student's info
        [HttpGet]
        public HttpResponseMessage SurveyFetchingAsStudentInfo(string StudentId)
        {
            try
            {
                DateTime currentDate = DateTime.Parse(DateTime.Today.ToShortDateString().Split(' ')[0]);

                var studentfetching = db.bothstudent.Where(s => s.arid_number == StudentId).FirstOrDefault();
                if (studentfetching == null)
                    return Request.CreateResponse("Student Not found");

                // Fetching the surveys based on the student's information
                if (studentfetching.status == "current")
                {
                    string gender = studentfetching.gender == "M" ? "male" : "female";
                    // Fetch current student surveys
                    var AllCurrentStudentSurveys = db.AssignedSurvey
                        .Where(s => s.studentType == "current"
                                    && s.Section == studentfetching.section
                                    && s.Semester == studentfetching.semester
                                    && s.Gender == gender
                                    && s.Department == studentfetching.department
                                    && s.createsurvey.EndDate > currentDate.Date
                                    && !db.CompletedSurveys.Any(cs => cs.createsurvey.SurveyID == s.createsurvey.SurveyID && cs.bothstudent.student_id == studentfetching.student_id))
                        .Select(s => new
                        {
                            s.createsurvey.SurveyTitle,
                            s.createsurvey.StartDate,
                            s.createsurvey.EndDate,
                            s.createsurvey.SurveyID,
                            
                               questions=s.createsurvey.survey_questions.Select(q => new
                                {
                                   options = new
                                   {
                                       q.QuestionID,
                                       q.Option1,
                                       q.Option2,
                                       q.Option3,
                                   },
                                    q.QuestionText,
                                })
                        })
                        .ToList();

                    // Returning the response
                    return Request.CreateResponse(AllCurrentStudentSurveys);
                }
                else
                {
                    string gender = studentfetching.gender == "M" ? "male" : "female";

                    // Fetch alumni student surveys
                    var AllAlumniStudentSurveys = db.AssignedSurvey
                        .Where(s => s.studentType == "alumni"
                                    && s.City == studentfetching.address
                                    && s.Department == studentfetching.department
                                    && s.GraduationYear.ToString() == studentfetching.graduation_year
                                  && s.Gender == gender
                                    && s.Technology == studentfetching.Technology
                                    && s.createsurvey.EndDate > currentDate
                                    && !db.CompletedSurveys.Any(cs => cs.createsurvey.SurveyID == s.createsurvey.SurveyID && cs.bothstudent.student_id == studentfetching.student_id))
                        .Select(s => new
                        {
                            s.createsurvey.SurveyTitle,
                            s.createsurvey.StartDate,
                            s.createsurvey.EndDate,
                            s.createsurvey.SurveyID,


                               questions = s.createsurvey.survey_questions.Select(q => new
                               {
                                   options = new
                                   {
                                       q.QuestionID,
                                       q.Option1,
                                       q.Option2,
                                       q.Option3,
                                   },
                                   q.QuestionText,
                               })
                        })
                        .ToList();

                    return Request.CreateResponse(AllAlumniStudentSurveys);
                }
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage SurveyAlluminiStudent(List<surveyAllumini> data)
        {
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The survey list cannot be null.");
            }

            try
            {
                foreach (var item in data)
                {
                    db.surveyAllumini.Add(item);
                    db.SaveChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK, 200);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }




        [HttpGet]
        public HttpResponseMessage getTheSurveyyTitle(string title)
        {
            try
            {
                var survey = db.createsurvey.Where(s => s.SurveyTitle == title).FirstOrDefault();
                if (survey != null)
                {
                    return Request.CreateResponse(survey);
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
        [HttpGet]
        public IHttpActionResult Alumni(List<surveyAllumini> surveyAlluminis)
        {

            foreach (var item in surveyAlluminis)
            {
                db.surveyAllumini.Add(item);
                db.SaveChanges();
            }

            return Ok(db.survey_questions);
        }
        // Submit the Questions Response to the Database
        [HttpPost]
        public HttpResponseMessage SaveTheResponse(List<SaveSurveyResponse> Responses)
        {
            try
            {
                if (Responses == null || !Responses.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No responses provided.");
                }

                foreach (var response in Responses)
                {
                    var surveyfinding = db.createsurvey.Where(s => s.SurveyID == response.surveyId).FirstOrDefault();
                    var studentFinding = db.bothstudent.Where(s => s.student_id == response.studentId).FirstOrDefault();
                    if (surveyfinding == null)
                    {
                        return Request.CreateResponse("Survey not founded");
                    }
                    if (surveyfinding == null)
                    {
                        return Request.CreateResponse("Student not founded");
                    }
                    CompletedSurveys completedSurveys = new CompletedSurveys();
                    completedSurveys.QuestionId = response.QuestionId;
                    completedSurveys.answer = response.answer;
                    completedSurveys.createsurvey = surveyfinding;
                    completedSurveys.bothstudent = studentFinding;
                    db.CompletedSurveys.Add(completedSurveys);
                    
                }
                int RowsEffected = db.SaveChanges();
                if (RowsEffected != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Responses submitted successfully.");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Responses not Submitted.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }
        [HttpGet]
        public HttpResponseMessage GetSurveyResult(int survey_id)
        {
            try
            {
                // LINQ query to join survey questions with completed surveys and group by question details
                var rawQuery = from sq in db.survey_questions
                               join cs in db.CompletedSurveys on sq.QuestionID equals cs.QuestionId into csGroup
                               from cs in csGroup.DefaultIfEmpty()
                               where sq.createsurvey.SurveyID == survey_id
                               group cs by new { sq.QuestionID, sq.QuestionText, sq.Option1, sq.Option2, sq.Option3 } into g
                               select new
                               {
                                   QuestionID = g.Key.QuestionID,
                                   QuestionText = g.Key.QuestionText,
                                   Option1 = g.Key.Option1,
                                   Option2 = g.Key.Option2,
                                   Option3 = g.Key.Option3,
                                   Option1Count = g.Count(cs => cs != null && cs.answer == 1), // Count for Option 1
                                   Option2Count = g.Count(cs => cs != null && cs.answer == 2), // Count for Option 2
                                   Option3Count = g.Count(cs => cs != null && cs.answer == 3), // Count for Option 3
                                   SkipCount = g.Count(cs => cs != null && cs.answer == -1),  // Count for skipped questions
                                   MaybeCount = g.Count(cs => cs != null && cs.answer == 0)   // Count for "maybe" responses
                               };

                // Executing the query and transforming the results into a more readable format
                var rawData = rawQuery.ToList();

                var result = rawData.Select(item => new
                {
                    item.QuestionID,
                    item.QuestionText,
                    OptionCounts = new Dictionary<string, int>
            {
                { item.Option1, item.Option1Count },
                { item.Option2, item.Option2Count },
                { item.Option3, item.Option3Count },
                { "Skipped", item.SkipCount },   // Adding the skipped count to the dictionary
                { "Maybe", item.MaybeCount }     // Adding the maybe count to the dictionary
            }
                }).ToList();

                // Returning the final results as an HTTP response
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if necessary
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        // Admin Job Approving
        [HttpPost]
        public HttpResponseMessage AproveTheJob(int application_id,int status)
        {
            try {
                var Application = db.JobApplications.Where(s => s.ApplicationID == application_id && s.IsApproved==false).FirstOrDefault();
                if (Application == null)
                {
                    return Request.CreateResponse("Application Not Founded");
                }
                if (status == 1)
                {
                    Application.IsApproved = true;
                }
                else
                {
                    Application.IsApproved = false;
                }
                int RowsEffected=db.SaveChanges();
                return Request.CreateResponse("Job Application Updated");
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }

        // Survey Editing and updating the Surveys
        [HttpGet]
        public HttpResponseMessage FetchSingleSurvey(int surveyId)
        {
            try
            {
                var surveyFinding = db.createsurvey.Where(s => s.SurveyID == surveyId).Select(s =>
                new {
                    s.SurveyID,
                    s.EndDate,
                    s.StartDate,
                    s.SurveyTitle
                }).FirstOrDefault();
                if (surveyFinding == null)
                {
                    return Request.CreateResponse("Not founded");
                }
                return Request.CreateResponse(surveyFinding);
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage UpdatingSurvey(int surveyId, string surveyTitle, DateTime startDate, DateTime endDate)
        {
            try
            {
                var SurveyFinding = db.createsurvey.Where(s => s.SurveyID == surveyId).FirstOrDefault();
                if (SurveyFinding == null)
                {
                    return Request.CreateResponse("Not founded");
                }
                SurveyFinding.SurveyTitle = surveyTitle;
                SurveyFinding.StartDate = startDate;
                SurveyFinding.EndDate = startDate;
                int RowsEffected=db.SaveChanges();
                return Request.CreateResponse("Updated the Survey");
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage fetchQuestions(int surveyId)
        {
            try
            {
                var SurveyFinding = db.createsurvey.Where(s => s.SurveyID == surveyId).FirstOrDefault();
                if (SurveyFinding == null)
                {
                    return Request.CreateResponse("Survey Finding");
                }
                var QuestionsOnSurvey = db.survey_questions.Where(s => s.createsurvey.SurveyID == surveyId).Select(s => new
                {
                    s.QuestionID,
                    s.QuestionText,
                    s.Option1,
                    s.Option2,
                    s.Option3,
                    s.createsurvey.SurveyID
                }).Distinct().ToList();
                if (QuestionsOnSurvey == null)
                {
                    return Request.CreateResponse("Questions not Founded");
                }
                return Request.CreateResponse(QuestionsOnSurvey);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }
        //Update the Survey Questions
        [HttpPost]
        public HttpResponseMessage UpdateSurveyQuestions(List<QuestionAdding> Questions)
        {
            try
            {
                survey_questions survey_question = new survey_questions();
                if (Questions != null)
                {
                    foreach (var question in Questions)
                    {
                        if (question.questionId ==0)
                        {
                            var surveyFinding = db.createsurvey.Where(s => s.SurveyID == question.surveyID).FirstOrDefault();
                            if (surveyFinding == null)
                            {
                                return Request.CreateResponse("Survey not fonded");
                            }
                            survey_question.QuestionText = question.questionText;
                            survey_question.Option1 = question.option1;
                            survey_question.Option2 = question.option2;
                            survey_question.Option3 = question.option3;
                            survey_question.createsurvey = surveyFinding;
                            db.survey_questions.Add(survey_question);
                        }
                        else
                        {
                            var questionFinding = db.survey_questions.Where(s => s.QuestionID == question.questionId).FirstOrDefault();
                            if (questionFinding == null)
                            {
                                return Request.CreateResponse("Question not founded");
                            }
                            questionFinding.QuestionText = question.questionText;
                            questionFinding.Option1 = question.option1;
                            questionFinding.Option2 = question.option2;
                            questionFinding.Option3 = question.option3;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    return Request.CreateResponse("Questions must not be empty");
                }
                            
                int RowsEffected = db.SaveChanges();
                return Request.CreateResponse("Questions Updated "+RowsEffected);
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }

        }

        // updating the population
        [HttpGet]
        public HttpResponseMessage FetchTheCurrentPopulation(int surveyId)
        {
            try
            {
                var surveyFinding = db.createsurvey.Where(s => s.SurveyID == surveyId).FirstOrDefault();
                if (surveyFinding == null)
                {
                    return Request.CreateResponse("Survey Not Founded");
                }
                var Population = db.AssignedSurvey.Where(s => s.createsurvey.SurveyID == surveyFinding.SurveyID && s.studentType=="current").Select(s => new
                {
                    s.Section,
                    s.Semester,
                    s.Gender,
                    s.Department,
                    s.studentType,
                    s.createsurvey.SurveyID,
                    s.AssignmentID
                }).Distinct().ToList();
                if (Population == null)
                {
                    return Request.CreateResponse("Population Not Founded");
                }
                return Request.CreateResponse(Population);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }
        //Fetching the AlumniPopulation
        [HttpGet]
        public HttpResponseMessage FetchTheAlumniPopulation(int surveyId)
        {
            try
            {
                var surveyFinding = db.createsurvey.Where(s => s.SurveyID == surveyId).FirstOrDefault();
                if (surveyFinding == null)
                {
                    return Request.CreateResponse("Survey Not Founded");
                }
                var Population = db.AssignedSurvey.Where(s => s.createsurvey.SurveyID == surveyFinding.SurveyID && s.studentType == "alumni").Select(s => new
                {
                    s.Gender,
                    s.City,
                    s.Department,
                    s.studentType,
                    s.Technology,
                    s.GraduationYear,
                    s.createsurvey.SurveyID,
                    s.AssignmentID
                }).Distinct().ToList();
                if (Population == null)
                {
                    return Request.CreateResponse("Population Not Founded");
                }
                return Request.CreateResponse(Population);
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }



        //Manage Population Edit
        [HttpPost]
public HttpResponseMessage ManagePopulationUpdating(StudentSurveyPopulation surveyPopulation)
{
    try
    {
        if (surveyPopulation != null)
        {
            var survey = db.createsurvey.Where(s => s.SurveyID == surveyPopulation.surveyID).FirstOrDefault();
            if (survey == null) return Request.CreateResponse(HttpStatusCode.NotFound, "Survey Not found");

            var studentType = surveyPopulation.studentType;

            if (studentType == "current")
            {
                if (surveyPopulation.degree == null || surveyPopulation.semester == null || surveyPopulation.section == null || surveyPopulation.AssignmentId == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Missing required fields for current students");
                }

                foreach (var degree in surveyPopulation.degree)
                {
                    foreach (var semester in surveyPopulation.semester)
                    {
                        foreach (var section in surveyPopulation.section)
                        {
                            foreach (var assignmentId in surveyPopulation.AssignmentId)
                            {
                                var AssignmentFounding = db.AssignedSurvey.Where(s => s.AssignmentID == assignmentId).FirstOrDefault();
                                if (AssignmentFounding != null)
                                {
                                    AssignmentFounding.Department = degree;
                                    AssignmentFounding.Semester = semester;
                                    AssignmentFounding.Section = section;
                                    AssignmentFounding.Gender = surveyPopulation.gender;
                                    AssignmentFounding.studentType = studentType;
                                    AssignmentFounding.createsurvey = survey;
                                }
                            }
                            AssignedSurvey population = new AssignedSurvey
                            {
                                Department = degree,
                                Semester = semester,
                                Section = section,
                                Gender = surveyPopulation.gender,
                                City = null, // Set as necessary
                                Technology = null, // Set as necessary
                                GraduationYear = 0, // Set as necessary
                                studentType = studentType,
                                createsurvey = survey,
                            };
                            db.AssignedSurvey.Add(population);
                        }
                    }
                }
            }
            else
            {
                if (surveyPopulation.address == null || surveyPopulation.Technology == null || surveyPopulation.graduation == null || surveyPopulation.degree == null || surveyPopulation.AssignmentId == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Missing required fields for alumni students");
                }

                foreach (var city in surveyPopulation.address)
                {
                    foreach (var technology in surveyPopulation.Technology)
                    {
                        foreach (var graduationYear in surveyPopulation.graduation)
                        {
                            foreach (var assignmentId in surveyPopulation.AssignmentId)
                            {
                                var AssignmentFounding = db.AssignedSurvey.Where(s => s.AssignmentID == assignmentId).FirstOrDefault();
                                if (AssignmentFounding != null)
                                {
                                    AssignmentFounding.Department = surveyPopulation.degree.FirstOrDefault(); // Check if degree is empty
                                    AssignmentFounding.Gender = surveyPopulation.gender;
                                    AssignmentFounding.City = city;
                                    AssignmentFounding.Technology = technology;
                                    AssignmentFounding.GraduationYear = graduationYear;
                                    AssignmentFounding.studentType = studentType;
                                    AssignmentFounding.createsurvey = survey;
                                }
                            }
                            AssignedSurvey assignedSurvey = new AssignedSurvey
                            {
                                Department = surveyPopulation.degree.FirstOrDefault(), // Check if degree is empty
                                Semester = null,
                                Section = null,
                                Gender = surveyPopulation.gender,
                                City = city,
                                Technology = technology,
                                GraduationYear = graduationYear,
                                createsurvey = survey,
                                studentType = studentType,
                            };

                            db.AssignedSurvey.Add(assignedSurvey);
                        }
                    }
                }
            }
            int RowsEffected = db.SaveChanges();
            if (RowsEffected != 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Data successfully inserted");
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Data Not inserted");
        }
        else
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid data");
        }
    }
    catch (Exception ex)
    {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error: " + ex.Message);
    }
}

        [HttpGet]
        public HttpResponseMessage DeleteQuestionFromSurveyQuestions(int questionId)
        {
            try
            {
                var questionFinding = db.survey_questions.Where(s => s.QuestionID == questionId).FirstOrDefault();
                if (questionFinding == null)
                {
                    return Request.CreateResponse("Question not founded");
                }
                db.survey_questions.Remove(questionFinding);
                int RowsEffected = db.SaveChanges();
                if (RowsEffected > 0)
                {
                    return Request.CreateResponse("Question Deleted");
                }
                return Request.CreateResponse("Question Not Deleted");
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }



    }
}
