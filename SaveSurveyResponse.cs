using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace studentminiportal
{
    public class SaveSurveyResponse
    {
        public int QuestionId { get; set; }
        public int answer { get; set; }
        public int surveyId { get; set; }
        public int studentId { get; set; }
    }
}