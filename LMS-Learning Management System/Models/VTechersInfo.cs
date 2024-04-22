using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class VTechersInfo
    {
        //public VTechersInfo()
        //{
        //    VTecherInfo = new HashSet<VTechersInfo>();
        //}
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Subject { get; set; }
        public string Class { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public string Photo { get; set; }
        [NotMapped]
        public virtual ICollection<VTechersInfo> VTecherInfo { get; set; }

    }
}
