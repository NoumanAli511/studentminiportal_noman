//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace studentminiportal
{
    using System;
    using System.Collections.Generic;
    
    public partial class courses
    {
        public string course_name { get; set; }
        public Nullable<int> credit_hour { get; set; }
        public string course_code { get; set; }
        public string course_grade { get; set; }
    
        public virtual Student Student { get; set; }
    }
}
