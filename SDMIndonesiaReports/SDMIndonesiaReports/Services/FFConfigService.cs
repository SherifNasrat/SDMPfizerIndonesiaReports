using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDMIndonesiaReports.Models.CustomModels;
namespace SDMIndonesiaReports.Services
{
    public class FFConfigService : ReportBase
    {

        public FFConfigService()
        {
            countriesDLItems = new List<SelectListItem>();
            periodsDLItems = new List<SelectListItem>();
        }

        public List<FFConfigVM> GetReportData(int? countryID, int? periodID)
        {

            try
            {
                if (periodID == null)
                {
                    periodID = GetCurrentPeriod(countryID);
                }
                var data = context.SP_FFConfig(periodID, countryID).Select(m => new FFConfigVM
                {
                    Area = m.Area,
                    HCRName = m.HCR_Name,
                    Job = m.Job,
                    Status = (m.Vacant==true) ? "Active":"Vacant",
                    AM = m.AM,
                    SM = m.SM


                }).OrderByDescending(m => m.HCRName).AsQueryable();
                
                return data.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        

    }
}