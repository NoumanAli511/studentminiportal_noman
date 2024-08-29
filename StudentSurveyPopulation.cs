using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace studentminiportal
{
    public class StudentSurveyPopulation
    {
        public int surveyID { get; set; }
        public string studentType{ get; set; }
        public List<string> section { get; set; }
        public List<int> semester { get; set; }
        public List<string> degree { get; set; }
        public string gender{ get; set; }
        //alumni student
        public List<string> address{ get; set; }
        public List<string> Technology{ get; set; }
        public List<int> graduation{ get; set; }
        
    }
}