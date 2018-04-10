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
    public class FFConfigController : Controller
    {
        private readonly FFConfigService _ffconfigservice = new FFConfigService();
       // SDW_TargetingEntities context = new SDW_TargetingEntities();


        // GET: Reports

        public ActionResult Index()
        {

            ViewBag.CountriesDL = _ffconfigservice.GetCountries();

            return View();
        }
        //[DataSourceRequest] DataSourceRequest request
        public ActionResult Periods_Read(int? countryID)
        {
            try
            {
                var result = _ffconfigservice.GetPeriods(countryID);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult FFConfig_Read([DataSourceRequest] DataSourceRequest request, int? countryID, int? periodID)
        {
            try
            {

                var result = _ffconfigservice.GetReportData(countryID, periodID).ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #region Export

    

        #endregion

      

        public FileResult ExportPdf([DataSourceRequest] DataSourceRequest request, int? countryID, int? periodID)
        {
            var data = _ffconfigservice.GetReportData(countryID, periodID).ToList();
            var list = data.Select(r => new
            {
                Area = r.Area,
                HCRName = r.HCRName,
                Position = r.Job,
                Status = r.Status,
                AM = r.AM,
                SM = r.SM

            }).AsQueryable();

            //Call Generic Export PDF method
            byte[] result = ExportBase.ExportPdfGeneric(
                request,
                list,
                 new string[] { "Area", "HCRName", "Position", "Status", "AM", "SM" },
                new string[] { "Area", "HCR NAME", "Position", "Status", "AM", "SM" });

            //Response carrying PDF file generated from byte-array
            return File(result, "application/pdf", "FFConfig Report.pdf");
        }
        public FileResult ExportXls([DataSourceRequest] DataSourceRequest request, int? countryID, int? periodID)
        {
            var data = _ffconfigservice.GetReportData(countryID, periodID).ToList();
            var list = data.Select(r => new
            {
                Area = r.Area,
                HCRName = r.HCRName,
                Position = r.Job,
                Status = r.Status,
                AM = r.AM,
                SM = r.SM

            }).AsQueryable();

            //Call Generic Export PDF method
            byte[] result = ExportBase.ExportXlsGeneric(
                request,
                list,
                 new string[] { "Area", "HCRName", "Position", "Status", "AM", "SM" },
                new string[] { "Area", "HCR NAME", "Position", "Status", "AM", "SM" });

            //Response carrying PDF file generated from byte-array
            return File(result, //The binary data of the XLS file
            "application/vnd.ms-excel", //MIME type of Excel files
            "FFConfig Report.xls"); 
        }
    }
}