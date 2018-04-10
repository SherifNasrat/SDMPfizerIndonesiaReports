using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDMIndonesiaReportsDB.Model;
using SDMIndonesiaReports.Models.CustomModels;
using System.Web.Mvc;

namespace SDMIndonesiaReports.Services
{
    public class CustomerShareReportService : ReportBase
    {

        public List<SelectListItem> getProfiles(int? countryID,int? fromPeriodID,int? toPeriodID)
        {
            /*
             ( ProfileAdditionalAccount.FK_FromPeriodID <= @ToPeriodID OR @ToPeriodID IS NULL)
  And (ProfileAdditionalAccount.FK_ToPeriodID >= (@FromPeriodID) OR ProfileAdditionalAccount.FK_ToPeriodID IS null)
  AND (ProfileAdditionalAccount.FK_FromPeriodID <= @FromPeriodID)
             */
            if (fromPeriodID == null)
            {
                fromPeriodID = GetCurrentPeriod(countryID);
            }
            if(toPeriodID == null)
            {
                toPeriodID = GetCurrentPeriod(countryID); 
            }
            var result = context.SP_GetProfileByPeriods(countryID, fromPeriodID, toPeriodID).Select(p => new SelectListItem { Text = p.profilename, Value=p.ProfileId.ToString()}).Distinct().OrderBy(p => p.Text).ToList();

            return result;
        }
        public List<CustomerShareReportVM> GetReportData(int? cID, int? fromPeriodID, int? toPeriodID,string [] profiles )
        {
            try
            {
                
                string profilesCSV = null;
                if (toPeriodID == null)
                {
                    toPeriodID = GetCurrentPeriod(cID);
                   
                }
                if (fromPeriodID == null)
                {
                    fromPeriodID = GetCurrentPeriod(cID);
                }



                if (profiles != null)
                {
                    if (profiles[0] == "" || profiles[0] == "null")
                        profiles = null;
                    else
                    profilesCSV = string.Join(",", profiles);
                }
                   

                
               
                var sp_Data = context.SP_CustomerShareReport(cID,fromPeriodID,toPeriodID,profilesCSV).Select(m => new CustomerShareReportVM
                {
                    Province_Name = m.Province_Name,
                    City_Name = m.City_Name,
                    Brick_Name = m.Brick_Name,
                    Pfizer_Customer_Code = m.Pfizer_Customer_Code,
                    Pfizer_Customer_Name = m.Pfizer_Customer_Name,
                    Dosage_Form_Code = m.Dosage_Form_Code,
                    Dosage_Form_Name = m.Dosage_Form_Name,
                    Product_Name = m.Product_Name,
                    Area_Code = m.Area_Code,
                    Percentage = Convert.ToDecimal(Math.Round((double)m.Percentage,4)),
                }).OrderByDescending(m => m.Pfizer_Customer_Name).AsQueryable();
                return sp_Data.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

    }
}