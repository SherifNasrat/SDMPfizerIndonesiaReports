using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDMIndonesiaReports.Models.CustomModels;

namespace SDMIndonesiaReports.Services
{
    public class TeamToProductService:ReportBase
    {
        public List<TeamToProductVM> GetReportData(int? countryID, int? fromPeriodID, int? toPeriodID)
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
                    var data = context.SP_TeamToProduct(countryID, fromPeriodID, toPeriodID).Select(m => new TeamToProductVM
                    {
                        Team_Code = m.Team_Code,
                        Team_Name = m.Team_Name,
                        AP = m.AP,
                        Year = m.Year,
                        Tier = m.Tier,
                        Product_Group = m.Product_Group,
                        Product = m.Product
                    }).OrderByDescending(m => m.Team_Name).AsQueryable();

                    return data.ToList();
               
            }catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}