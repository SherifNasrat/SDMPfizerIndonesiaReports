using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDMIndonesiaReports.Models.CustomModels;
namespace SDMIndonesiaReports.Services
{
    public class TransactionDetailsService:ReportBase
    {
        public List<TransactionDetailsVM> GetReportData(int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            //try
            //{
                if (fromPeriodID == null)
                {
                    fromPeriodID = GetCurrentPeriod(countryID);
                }

                if (toPeriodID == null)
                {
                    toPeriodID = GetCurrentPeriod(countryID);
                }

                var data = context.SP_TransactionDetail(countryID, fromPeriodID, toPeriodID).Select(m => new TransactionDetailsVM
                {
                    Date = m.Date,
                    Invoice = "N/A",
                    Product_Name = m.Product_Name,
                    Cust_Code = m.Cust_Code,
                    Cust_Name = m.Cust_Name,
                    Sector = m.Sector,
                    Address = m.Address,
                    Province = m.Province,
                    City = m.City,
                    Brick = m.Brick,
                    SM = m.SM,
                    AM = m.AM,
                    HCR =m.HCR,
                    Profile= m.Profile,
                    Qty = (decimal)m.QTY,
                    Value = Math.Round((double)m.VALUE,4)
                }).AsQueryable();

                return data.ToList();

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
    }
}