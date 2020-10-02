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
    
    public partial class Student
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Student()
        {
            this.Sessions = new HashSet<Session>();
            this.NotAvailbleTimesOfGroups = new HashSet<NotAvailbleTimesOfGroup>();
            this.SuitableRoomsforGroups = new HashSet<SuitableRoomsforGroup>();
        }
    
        public int id { get; set; }
        public string acdamicYear { get; set; }
        public string semester { get; set; }
        public string program { get; set; }
        public string groupNo { get; set; }
        public string groupId { get; set; }
        public string subGroupNo { get; set; }
        public string subGropId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Session> Sessions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotAvailbleTimesOfGroup> NotAvailbleTimesOfGroups { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SuitableRoomsforGroup> SuitableRoomsforGroups { get; set; }
    }
}
