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
    
    public partial class ProfileAdditionalAccount
    {
        public ProfileAdditionalAccount()
        {
            this.ProfileAdditionalAccount1 = new HashSet<ProfileAdditionalAccount>();
        }
    
        public long AdditionalAccountId { get; set; }
        public int FK_ProfileId { get; set; }
        public long FK_CustomerBranchId { get; set; }
        public decimal Percentage { get; set; }
        public short AssignmentSalesType { get; set; }
        public int FK_FromPeriodID { get; set; }
        public Nullable<int> FK_ToPeriodID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<int> FK_AddedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<int> FK_LastModifiedBy { get; set; }
        public Nullable<long> FK_AdditionalAccountID { get; set; }
        public Nullable<long> FK_DosageFormId { get; set; }
        public Nullable<long> FK_ProductId { get; set; }
        public Nullable<long> FK_ProfileAdditionalAccountsUploadID { get; set; }
    
        public virtual CustomerBranch CustomerBranch { get; set; }
        public virtual Period Period { get; set; }
        public virtual Period Period1 { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual SystemUser SystemUser1 { get; set; }
        public virtual ICollection<ProfileAdditionalAccount> ProfileAdditionalAccount1 { get; set; }
        public virtual ProfileAdditionalAccount ProfileAdditionalAccount2 { get; set; }
    }
}