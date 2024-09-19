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
    public class EventController : ApiController
    {
        projectdatabaseEntities12 db = new projectdatabaseEntities12();
        [HttpPost]
        public HttpResponseMessage CreateEvent()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

                // Logging to check received values
                foreach (var key in httpRequest.Form.AllKeys)
                {
                    var value = httpRequest.Form[key];
                    System.Diagnostics.Debug.WriteLine($"{key}: {value}");
                }

                var title = httpRequest["title"];
                var eventDate = DateTime.Parse(httpRequest["event_date"]);
                var venue = httpRequest["venue"];
                var description = httpRequest["description"];
                var postedFile = httpRequest.Files[0];

                // Additional logging to check values
                System.Diagnostics.Debug.WriteLine($"title: {title}");
                System.Diagnostics.Debug.WriteLine($"event_date: {eventDate}");
                System.Diagnostics.Debug.WriteLine($"venue: {venue}");
                System.Diagnostics.Debug.WriteLine($"description: {description}");

                string imagePath = null;
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string uploadsFolder = HttpContext.Current.Server.MapPath("~/Images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(postedFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    postedFile.SaveAs(filePath);
                    imagePath =uniqueFileName;
                }

                Events newEvent = new Events
                {
                    title = title,
                    event_date = eventDate,
                    venue = venue,
                    description = description,
                    image_path = imagePath
                };

                db.Events.Add(newEvent);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.Created, newEvent);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpDelete]
        public HttpResponseMessage DeleteEvent(int event_id)
        {
            try
            {
                var Event = db.Events.Where(s => s.event_id == event_id).FirstOrDefault();
                if(Event != null)
                {
                    db.Events.Remove(Event);                 
                }
                else
                {
                    return Request.CreateResponse("Event not founded");
                }
                int RowsEffected = db.SaveChanges();
                if (RowsEffected != 0)
                {
                    return Request.CreateResponse("Event Deleted");
                }
                else
                {
                    return Request.CreateResponse("Event Not Deleted");
                }
            }
            catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        [HttpPost]
        public HttpResponseMessage UpdateTheEvent()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var event_id = httpRequest.Form["event_id"];
                int id;
                if (!int.TryParse(event_id, out id))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid event ID.");
                }

                var eventItem = db.Events.FirstOrDefault(s => s.event_id == id);
                if (eventItem == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Event not found.");
                }

                var title = httpRequest.Form["title"];
                var eventDate = DateTime.Parse(httpRequest.Form["event_date"]);
                var venue = httpRequest.Form["venue"];
                var description = httpRequest.Form["description"];

                // Ensure the form contains a file
                if (httpRequest.Files.Count > 0)
                {
                    var postedFile = httpRequest.Files[0];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        string uploadsFolder = HttpContext.Current.Server.MapPath("~/Images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(postedFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        try
                        {
                            postedFile.SaveAs(filePath);
                            eventItem.image_path = uniqueFileName; // Update the image_path only if a new image is uploaded
                        }
                        catch (Exception fileEx)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error saving file: " + fileEx.Message);
                        }
                    }
                }

                eventItem.title = title;
                eventItem.event_date = eventDate;
                eventItem.venue = venue;
                eventItem.description = description;

                int rowsAffected = db.SaveChanges();
                if (rowsAffected > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Event edited successfully");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Event not edited.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error: " + ex.Message + " : " + ex.InnerException?.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetEvents()
        {
            try
            {
                var events = db.Events.Select(s=>new { 
                s.event_id,
                s.description,
                s.event_date,
                s.image_path,
                s.venue,
                s.title,
                }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, events);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        /// all upcomming events 
        [HttpGet]
        public HttpResponseMessage GetFutureEvents()
        {
            try
            {
                DateTime currentDate = DateTime.Now;

                var allEvents = db.Events.Select(s => s).Distinct().ToList();
               // var allEvents = db.Events.Where(s => s.event_date > currentDate).Distinct().ToList();
                if (allEvents != null)
                {
                    return Request.CreateResponse(allEvents);
                }
                else
                {
                    return Request.CreateResponse("Not founded Any event");
                }


            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        // getting the names of students who responsded on the survey particular section
        [HttpGet]
        public HttpResponseMessage GetStudentResponseSections(int answer,int surveyId,int questionId)
        {
            try
            {
                if (answer == 3)
                {
                    answer = -1;
                }
                else if(answer==4)
                {
                    answer = 0;
                }
                else if(answer==2)
                {
                    answer = 3;
                }else if (answer == 1)
                {
                    answer = 2;
                }else if (answer == 0)
                {
                    answer = 1;
                }
                var completedSurveyFinding = db.CompletedSurveys.Where(s => s.answer == answer && s.createsurvey.SurveyID == surveyId && s.QuestionId == questionId).Select(s => new
                {
                    s.bothstudent.student_id,
                    s.bothstudent.name,
                    s.bothstudent.arid_number,
                    s.bothstudent.address,
                }).Distinct().ToList();
                if (completedSurveyFinding == null)
                {
                    return Request.CreateResponse("No Response Founded");
                }
                return Request.CreateResponse(completedSurveyFinding);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }
        


        /* [HttpDelete]
         [Route("api/Events/DeleteEvent/{id}")]
         public HttpResponseMessage DeleteEvent(int id)
         {
             try
             {
                 var eventItem = db.Events.Find(id);
                 if (eventItem == null)
                 {
                     return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "Event not found" });
                 }

                 db.Events.Remove(eventItem);
                 db.SaveChanges();

                 return Request.CreateResponse(HttpStatusCode.OK, new { message = "Event deleted successfully" });
             }
             catch (Exception e)
             {
                 return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
             }
         }*/
    }
}
