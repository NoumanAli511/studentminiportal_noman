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
    
    public partial class ViewedEvents
    {
        public int id { get; set; }
    
        public virtual bothstudent bothstudent { get; set; }
        public virtual Events Events { get; set; }
    }
}