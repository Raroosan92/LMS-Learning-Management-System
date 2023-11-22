﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    [ModelMetadataType(typeof(MetaData_Subject))]
    public partial class Subject
    {
    }


    public class MetaData_Subject
    {
        public MetaData_Subject()
        {
            Enrollments = new HashSet<Enrollment>();
            Lessons = new HashSet<Lesson>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUser { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
