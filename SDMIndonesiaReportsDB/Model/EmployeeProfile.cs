//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SDMIndonesiaReportsDB.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeProfile
    {
        public long EmployeeProfileID { get; set; }
        public long FK_EmployeeID { get; set; }
        public int FK_ProfileID { get; set; }
        public decimal Percentage { get; set; }
        public bool AssignmentType { get; set; }
        public int FK_FromPeriodID { get; set; }
        public Nullable<int> FK_ToPeriodID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<int> FK_AddedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<int> FK_LastModifiedBy { get; set; }
    
        public virtual SystemUser SystemUser { get; set; }
        public virtual EmployeeRecord EmployeeRecord { get; set; }
        public virtual Period Period { get; set; }
        public virtual SystemUser SystemUser1 { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual Period Period1 { get; set; }
    }
}
