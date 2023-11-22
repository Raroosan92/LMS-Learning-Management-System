using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    [ModelMetadataType(typeof(MetaData_Lesson))]
    public partial class Lesson
    {
    }


    public class MetaData_Lesson
    {
      
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlVideo { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public bool? Status { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Class Class { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
