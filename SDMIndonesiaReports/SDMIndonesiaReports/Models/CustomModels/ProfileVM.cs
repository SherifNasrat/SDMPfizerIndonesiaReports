using SDMIndonesiaReportsDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDMIndonesiaReports.Models.CustomModels
{
    public class ProfileVM
    {
         public ProfileVM()
        {
            this.EmployeeProfiles = new HashSet<EmployeeProfile>();
            this.ProfileAdditionalAccounts = new HashSet<ProfileAdditionalAccount>();
            this.ProfileGeographicalAreas = new HashSet<ProfileGeographicalArea>();
        }
    
        public int ProfileId { get; set; }
        public string ProfileCode { get; set; }
        public string ProfileName { get; set; }
        public int FK_CountryId { get; set; }
        public Nullable<long> FK_SalesTeamID { get; set; }
        public int FK_FromPeriodID { get; set; }
        public Nullable<int> FK_ToPeriodID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<int> FK_AddedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<int> FK_LastModifiedBy { get; set; }
        public Nullable<bool> IsWholesale { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual ICollection<EmployeeProfile> EmployeeProfiles { get; set; }
        public virtual Period Period { get; set; }
        public virtual Period Period1 { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual SystemUser SystemUser1 { get; set; }
        public virtual ICollection<ProfileAdditionalAccount> ProfileAdditionalAccounts { get; set; }
        public virtual SalesTeam SalesTeam { get; set; }
        public virtual ICollection<ProfileGeographicalArea> ProfileGeographicalAreas { get; set; }
    }
}