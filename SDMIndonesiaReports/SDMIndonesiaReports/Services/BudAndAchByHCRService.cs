using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDMIndonesiaReports.Models.CustomModels;
namespace SDMIndonesiaReports.Services
{
    public class BudAndAchByHCRService:ReportBase
    {
        public List<SelectListItem> getProfiles(int? countryid, int? fromPeriodID, int? toPeriodID)
        {
            /*
             ( ProfileAdditionalAccount.FK_FromPeriodID <= @ToPeriodID OR @ToPeriodID IS NULL)
  And (ProfileAdditionalAccount.FK_ToPeriodID >= (@FromPeriodID) OR ProfileAdditionalAccount.FK_ToPeriodID IS null)
  AND (ProfileAdditionalAccount.FK_FromPeriodID <= @FromPeriodID)
             */
            if (fromPeriodID == null)
            {
                //fromPeriodID = context.Periods.Where(p => p.Year == DateTime.Now.Year).OrderByDescending(p => p.PeriodID).Select(p => p.PeriodID).FirstOrDefault();
                fromPeriodID = GetCurrentPeriod(countryid);
            }
            if(toPeriodID==null)
            {
                toPeriodID = GetCurrentPeriod(countryid);
            }
            var result = context.SP_GetProfileByPeriods(countryid, fromPeriodID, toPeriodID).Select(p => new SelectListItem { Text = p.profilename, Value = p.ProfileId.ToString() }).Distinct().OrderBy(p => p.Text).ToList();

            return result;
           
        }
        public List<BudAndAchByHCRVM> GetReportData(int? countryID, int? fromPeriodID, int? toPeriodID, string[] profiles, int? positionID)
        {
            //try
            //{
                 string profilesCSV = null;
                if (profiles != null)
                {
                    if (profiles[0] == "" || profiles[0]=="null")
                        profiles = null;
                    else
                        profilesCSV = string.Join(",", profiles);
                }
                if (fromPeriodID == null)
                {
                    fromPeriodID = GetCurrentPeriod(countryID);
                }

                if (toPeriodID == null)
                {
                    toPeriodID = GetCurrentPeriod(countryID);
                }

                if (positionID == 1 || positionID == null)
                {
                    var data = context.SP_BudAndAchbyHCR(countryID, fromPeriodID, toPeriodID, profilesCSV).Select(m => new BudAndAchByHCRVM
                    {
                        Country = null,
                        SM = m.SM,
                        AM = m.AM,
                        HCR = m.HCR,
                        Product_Group = m.Product_Group,
                        Product = m.Product,
                        Target_QTY = m.Target_Quantity,
                        Sales_QTY = m.Sales_Quantity,
                        Target_Amount = Math.Round((double)m.Target_Amount,4),
                        Sales_Amount = Math.Round((double)m.Sales_Amount,4),
                        Percent = (m.Target_Amount == 0 ? "" : Math.Round((double)((m.Sales_Amount / m.Target_Amount) * 100),4).ToString())
                       
                    }).OrderByDescending(m => m.HCR).AsQueryable();

                    return data.ToList();
                }
                else if (positionID == 2)
                {//
                    var data = context.SP_BudAndAchByAM(countryID, fromPeriodID, toPeriodID, profilesCSV).Select(m => new BudAndAchByHCRVM
                    {
                        Country = null,
                        SM = m.SM,
                        AM = m.AM,
                        HCR = null,
                        Product_Group = m.Product_Group,
                        Product = m.Product,
                        Target_QTY = m.Target_Quantity,
                        Sales_QTY = m.Sales_Quantity,
                        Target_Amount = Math.Round((double)m.Target_Amount, 4),
                        Sales_Amount = Math.Round((double)m.Sales_Amount, 4),
                        Percent = (m.Target_Amount == 0 ? "" : Math.Round((double)((m.Sales_Amount / m.Target_Amount) * 100), 4).ToString())
                    }).OrderByDescending(m => m.AM).AsQueryable();

                    return data.ToList();
                }
                else if (positionID ==3)
                {
                    var data = context.SP_BudAndAchBySM(countryID, fromPeriodID, toPeriodID, profilesCSV).Select(m => new BudAndAchByHCRVM
                    {
                        Country=null,
                        SM = m.SM,
                        AM = null,
                        HCR = null,
                        Product_Group = m.Product_Group,
                        Product = m.Product,
                        Target_QTY = m.Target_Quantity,
                        Sales_QTY = m.Sales_Quantity,
                        Target_Amount = Math.Round((double)m.Target_Amount, 4),
                        Sales_Amount = Math.Round((double)m.Sales_Amount, 4),
                        Percent = (m.Target_Amount == 0 ? "" : Math.Round((double)((m.Sales_Amount / m.Target_Amount) * 100), 4).ToString())
                    }).OrderByDescending(m => m.SM).AsQueryable();

                    return data.ToList();
                }
                else
                {
                    var data = context.SP_BudAndAchbyCountry(countryID, fromPeriodID, toPeriodID, profilesCSV).Select(m => new BudAndAchByHCRVM
                    {
                        Country = m.Country,
                        SM = null,
                        AM = null,
                        HCR = null,
                        Product_Group = m.Product_Group,
                        Product = m.Product,
                        Target_QTY = m.Target_Quantity,
                        Sales_QTY = m.Sales_Quantity,
                        Target_Amount = Math.Round((double)m.Target_Amount, 4),
                        Sales_Amount = Math.Round((double)m.Sales_Amount, 4),
                        Percent = (m.Target_Amount == 0 ? "" : Math.Round((double)((m.Sales_Amount / m.Target_Amount) * 100), 4).ToString())
                    }).OrderByDescending(m => m.Country).AsQueryable();

                    return data.ToList();
                }

            //}
            //catch (Exception ex)
            //{
            //    //throw ex;
            //  //  var data = new BudAndAchByHCRVM();
            //    return new List<BudAndAchByHCRVM>();
            //}
        }
    }
}