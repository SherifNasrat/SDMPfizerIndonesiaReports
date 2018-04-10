using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDMIndonesiaReports.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections;
using iTextSharp.text;
using System.Reflection;
using iTextSharp.text.pdf;
using System.IO;
using NPOI.HSSF.UserModel;
using SDMIndonesiaReports.Helpers;
namespace SDMIndonesiaReports.Controllers
{
    public class TeamToProductController : Controller
    {
        private readonly TeamToProductService _teamToProductService = new TeamToProductService();
        // GET: TeamToProduct
        public ActionResult Index()
        {
            ViewBag.CountriesDL = _teamToProductService.GetCountries();
            return View();
        }
        public ActionResult Periods_Read(int? countryID)
        {
            try
            {
                var result = _teamToProductService.GetPeriods(countryID);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ActionResult TeamToProduct_Read(DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            try
            {
                var result = _teamToProductService.GetReportData(countryID, fromPeriodID, toPeriodID).ToDataSourceResult(request);
                
                var res= Json(result, JsonRequestBehavior.AllowGet);
                res.MaxJsonLength = int.MaxValue;
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
   

     
        public FileResult ExportXls([DataSourceRequest]
                                    DataSourceRequest request,int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            var data = _teamToProductService.GetReportData(countryID, fromPeriodID, toPeriodID).ToList();
            var list = data.Select(m => new
                         {
                             Team_Code = m.Team_Code,
                             Team_Name = m.Team_Name,
                             AP = m.AP,
                             Year = m.Year,
                             Tier = m.Tier,
                             Product_Group = m.Product_Group,
                             Product = m.Product
                         }).AsQueryable();

            byte[] result = ExportBase.ExportXlsGeneric(
                request,
                list,
                 new string[] { "Team_Code", "Team_Name", "AP", "Year", "Tier", "Product_Group", "Product" },
                new string[] { "Team Code", "Team Name", "AP", "Year", "Tier", "Product Group", "Product" });
            return File(result, //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "TeamToProduct Report.xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportPdf([DataSourceRequest]
                                    DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            var data = _teamToProductService.GetReportData(countryID, fromPeriodID, toPeriodID).ToList();
            var list = data.Select(m => new 
            {
                Team_Code = m.Team_Code,
                Team_Name = m.Team_Name,
                AP = m.AP,
                Year = m.Year,
                Tier = m.Tier,
                Product_Group = m.Product_Group,
                Product = m.Product
            }).AsQueryable();

            //Call Generic Export PDF method
            byte[] result = ExportBase.ExportPdfGeneric(
                request,
                list,
                 new string[] { "Team_Code", "Team_Name", "AP", "Year", "Tier", "Product_Group", "Product" },
                new string[] { "Team Code", "Team Name", "AP", "Year", "Tier","Product Group","Product" });

            //Response carrying PDF file generated from byte-array
            return File(result, "application/pdf", "TeamToProduct Report.pdf");
        }
    

    }
}