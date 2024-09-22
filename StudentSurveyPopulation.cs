using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace studentminiportal
{
    public class StudentSurveyPopulation
    {
        public int surveyID { get; set; }
        public string studentType { get; set; }
        public List<int> AssignmentId { get; set; } = new List<int>(); // Initialize here
        public List<string> section { get; set; } = new List<string>(); // Initialize here
        public List<int> semester { get; set; } = new List<int>(); // Initialize here
        public List<string> degree { get; set; } = new List<string>(); // Initialize here
        public List<string> gender { get; set; }
        // alumni student
        public List<string> address { get; set; } = new List<string>(); // Initialize here
        public List<string> Technology { get; set; } = new List<string>(); // Initialize here
        public List<int> graduation { get; set; } = new List<int>(); // Initialize here
    }

}