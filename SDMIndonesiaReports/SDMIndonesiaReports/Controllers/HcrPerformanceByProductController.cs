using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Resources;
using SDMIndonesiaReportsDB.Model;
using SDMIndonesiaReports.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections;
using iTextSharp.text;
using System.Reflection;
using iTextSharp.text.pdf;
using System.IO;
using NPOI.HSSF.UserModel;
using SDMIndonesiaReports.Models.CustomModels;
using SDMIndonesiaReports.Helpers;
namespace SDMIndonesiaReports.Controllers
{
    public class HcrPerformanceByProductController : Controller
    {

        private readonly HcrPerformanceByProductService _hcrPerformanceByProductService = new HcrPerformanceByProductService();
        // GET: TeamToProduct
        public ActionResult Index()
        {
            ViewBag.CountriesDL = _hcrPerformanceByProductService.GetCountries();
            return View();
        }

        public ActionResult Periods_Read(int? countryID)
        {
            try
            {
                var result = _hcrPerformanceByProductService.GetPeriods(countryID);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ActionResult HcrPerformanceByProduct_Read(DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            try
            {
                var result = _hcrPerformanceByProductService.GetReportData(countryID, fromPeriodID, toPeriodID).ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public FileResult ExportXls([DataSourceRequest]
                                    DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            var data = _hcrPerformanceByProductService.GetReportData(countryID, fromPeriodID, toPeriodID).ToList();
            var list = data.Select(m => new
            {
                Profile = m.Profile,
                Hcr_Name = m.Hcr_Name,
                Team_Name = m.Team_Name,
                Product = m.Product,
                Attendance = m.Attendance,
                Bud = Math.Round((double)m.Bud, 4),
                Ach = Math.Round((double)m.Ach, 4),
                Ach_Percent = (m.Bud == 0 ? "" : Math.Round(((double)(m.Ach / m.Bud) * 100), 4).ToString())
            }).AsQueryable();

            byte[] result = ExportBase.ExportXlsGeneric(
                request,
                list,
                  new string[] { "Profile", "Hcr_Name", "Team_Name", "Product", "Attendance", "Bud", "Ach", "Ach_Percent" },
                new string[] { "Profile ID", "HCR Name","Team Name", "Product", "Attendance", "Bud", "Ach", "%" }); ;
            return File(result, //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "HcrPerformanceByProduct Report.xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportPdf([DataSourceRequest]
                                    DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            var data = _hcrPerformanceByProductService.GetReportData(countryID, fromPeriodID, toPeriodID).ToList();
            var list = data.Select(m => new
            {
                Profile = m.Profile,
                Hcr_Name = m.Hcr_Name,
                Team_Name = m.Team_Name,
                Product = m.Product,
                Attendance = m.Attendance,
                Bud = Math.Round((double)m.Bud, 4),
                Ach = Math.Round((double)m.Ach, 4),
                Ach_Percent = (m.Bud == 0 ? "" : Math.Round(((double)(m.Ach / m.Bud) * 100), 4).ToString())
            }).AsQueryable();

            //Call Generic Export PDF method
            byte[] result = ExportBase.ExportPdfGeneric(
                request,
                list,
       new string[] { "Profile", "Hcr_Name", "Team_Name", "Product", "Attendance", "Bud", "Ach", "Ach_Percent" },
                new string[] { "Profile ID", "HCR Name", "Team Name", "Product", "Attendance", "Bud", "Ach", "%" }); ;

            //Response carrying PDF file generated from byte-array
            return File(result, "application/pdf", "HcrPerformanceByProduct Report.pdf");
        }
    
    }
}