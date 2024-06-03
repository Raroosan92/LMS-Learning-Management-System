using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    [ModelMetadataType(typeof(MetaData_VLessonsDesc))]
    public partial class VLessonsDesc
    {
    }
    public partial class MetaData_VLessonsDesc
    {
        public int Id { get; set; }
        [Display(Name = "اسم الدرس")]
        public string Name { get; set; }
        [Display(Name = "وصف الدرس")]
        public string Description { get; set; }
        [Display(Name = "رابط الفيديو")]

        public string UrlVideo { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        [Display(Name = "الحالة")]

        public bool? Status { get; set; }
        [Display(Name = "اسم المُنشئ")]

        public string CreatedUser { get; set; }
        [Display(Name = "تاريخ الاضافة")]

        public DateTime? CreatedDate { get; set; }
        [Display(Name = "معلم المادة")]

        public string TeacherId { get; set; }
        [Display(Name = "المستوى")]

        public string SemesterDesc { get; set; }
        [Display(Name = "المستوى")]

        public int? Semester { get; set; }
        [Display(Name = "المادة")]

        public string SubjectDesc { get; set; }
        [Display(Name = "المستوى")]

        public string ClassDesc { get; set; }
        [Display(Name = "الحالة")]

        public string StatusDesc { get; set; }
    }
}
