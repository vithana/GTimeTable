//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GTimeTable
{
    using System;
    using System.Collections.Generic;
    
    public partial class Lecturer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lecturer()
        {
            this.LecturesSessions = new HashSet<LecturesSession>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string emp_id { get; set; }
        public string faculty { get; set; }
        public string dept { get; set; }
        public string center { get; set; }
        public int building { get; set; }
        public int lvl { get; set; }
        public string rank { get; set; }
    
        public virtual Building Building1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LecturesSession> LecturesSessions { get; set; }
    }
}
