using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDMIndonesiaReports.Models.CustomModels;
namespace SDMIndonesiaReports.Services 
{
    public class TerritoryToBrickByAreaService :ReportBase
    {
        
         public List<TerritoryToBrickByAreaVM> GetReportData(int? countryID,int? fromPeriodID,int? toPeriodID)
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
                 var data = context.SP_TerritoryToBrickByArea(countryID, fromPeriodID, toPeriodID).Select(m => new TerritoryToBrickByAreaVM
                 {
                     Period_Year = m.PeriodName,
                     Profile_Code = m.Profile_Code,
                     Medical_Rep = m.Medical_Rep,
                     Province = m.Province,
                     City = m.City,
                     Brick_Code = m.Brick_Code,
                     Brick = m.Brick
                 }).OrderByDescending(m => m.Period_Year).AsQueryable();
              
                 return data.ToList();
             }
             catch (Exception e)
             {
                 throw e;
             }

         }
    }
}