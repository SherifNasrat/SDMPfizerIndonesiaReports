using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDMIndonesiaReports.Models.CustomModels;
using SDMIndonesiaReports.Models;
namespace SDMIndonesiaReports.Services
{
    public class HcrPerformanceByProductService:ReportBase
    {
        public List<HcrPerformaceByProductVM> GetReportData(int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            try
            {
                if (fromPeriodID == null)
                {
                    fromPeriodID = GetCurrentPeriod(countryID);
                }

                if (toPeriodID == null)
                {
                    toPeriodID = GetCurrentPeriod(countryID);
                }
                var data = context.SP_HHCRPerformanceByProduct(countryID, fromPeriodID, toPeriodID)
                    .Select(m => new HcrPerformaceByProductVM

                {
                   Profile = m.Profile,
                   Hcr_Name = m.HCR,
                   Team_Name = m.Name,
                   Product = m.Product,
                    Attendance = m.Att,
                   Bud = Math.Round((double)m.Bud,4),
                   Ach = Math.Round((double)m.Ach,4),
                   Ach_Percent = (m.Bud == 0 ? "" : Math.Round(((double)(m.Ach / m.Bud)*100),4).ToString())
                });

                return data.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}