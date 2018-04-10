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
    public class BudAndAchByHCRController : Controller
    {
        private readonly BudAndAchByHCRService _budandachbyhcr = new BudAndAchByHCRService();
        // GET: TeamToProduct
        public ActionResult Index()
        {
            ViewBag.CountriesDL = _budandachbyhcr.GetCountries();
            ViewBag.jobTitlesDLItems = _budandachbyhcr.GetJobTitles();
            return View();
        }
        public ActionResult Countries_Read()
        {
            try
            {
                var listOfCountries = _budandachbyhcr.GetCountries();
                return Json(listOfCountries, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public ActionResult Positions_Read()
        {
            try
            {
                var listOfPositions = new List<SelectListItem>();
                listOfPositions.Add(new SelectListItem() { Text = "HCR", Value = "1" });
                listOfPositions.Add(new SelectListItem() { Text = "AM", Value = "2" });
                listOfPositions.Add(new SelectListItem() { Text = "SM", Value = "3" });
                listOfPositions.Add(new SelectListItem() { Text = "Country", Value = "4" });
                return Json(listOfPositions, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ActionResult Periods_Read(int? countryID)
        {
            try
            {
                var result = _budandachbyhcr.GetPeriods(countryID);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ActionResult Profiles_Read(int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            try
            {
                var result = _budandachbyhcr.getProfiles(countryID, fromPeriodID, toPeriodID);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ActionResult BudAndAchByHCR_Read(DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID,string[] profileIDs, int? positionID)
        {
            //try
            //{
                var result = _budandachbyhcr.GetReportData(countryID, fromPeriodID, toPeriodID,profileIDs, positionID).ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }



        public FileResult ExportXls([DataSourceRequest]
                                    DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID, string[] profileIDs, int? positionID)
        {
            string[] Headers = new string[11];
            string[] HeaderTitles = new string[11];
            var data = _budandachbyhcr.GetReportData(countryID, fromPeriodID, toPeriodID,profileIDs, positionID).ToList();
            var list = data.Select(m => new
            {
                Country = m.Country,
                SM = m.SM,
                AM = m.AM,
                HCR = m.HCR,
                Product_Group = m.Product_Group,
                Product = m.Product,
                Target_QTY = m.Target_QTY,
                Sales_QTY = m.Target_QTY,
                Target_Amount = Math.Round((double)m.Target_Amount,4),
                Sales_Amount = Math.Round((double)m.Sales_Amount,4),
                Percent = (m.Target_Amount == 0 ? "" : Math.Round((double)((m.Sales_Amount / m.Target_Amount) * 100),4).ToString())
            }).AsQueryable();
            if (positionID == 1 || positionID == null)
            {
                Headers = new string[] { "SM", "AM", "HCR", "Product_Group", "Product", "Target_QTY", "Sales_QTY", "Target_Amount", "Sales_Amount", "Percent" };
                HeaderTitles = new string[] { "SM", "AM", "HCR", "Product Group", "Product", "Bud QTY", "Ach QTY", "Bud Vlu", "Ach Vlu", "%" };
            }
            else if (positionID == 2)
            {
                Headers = new string[] { "SM", "AM", "Product_Group", "Product", "Target_QTY", "Sales_QTY", "Target_Amount", "Sales_Amount", "Percent" };
                HeaderTitles = new string[] { "SM", "AM", "Product Group", "Product", "Bud QTY", "Ach QTY", "Bud Vlu", "Ach Vlu", "%" };
            }
            else if (positionID == 3)
            {
                Headers = new string[] { "SM", "Product_Group", "Product", "Target_QTY", "Sales_QTY", "Target_Amount", "Sales_Amount", "Percent" };
                HeaderTitles = new string[] { "SM", "Product Group", "Product", "Bud QTY", "Ach QTY", "Bud Vlu", "Ach Vlu", "%" };
            }
            else
            {
                Headers = new string[] { "Country", "Product_Group", "Product", "Target_QTY", "Sales_QTY", "Target_Amount", "Sales_Amount", "Percent" };
                HeaderTitles = new string[] { "Country", "Product Group", "Product", "Bud QTY", "Ach QTY", "Bud Vlu", "Ach Vlu", "%" };
            }

            byte[] result = ExportBase.ExportXlsGeneric(
                request,
                list,
                 Headers,
                 HeaderTitles); ;
            return File(result, //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "BudAndAchByHCR Report.xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportPdf([DataSourceRequest]
                                    DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID,string[] profileIDs, int? positionID)
        {
            string[] Headers = new string[11];
            string[] HeaderTitles = new string[11];
            var data = _budandachbyhcr.GetReportData(countryID, fromPeriodID, toPeriodID, profileIDs, positionID).ToList();
            var list = data.Select(m => new
            {
                Country = m.Country,
                SM = m.SM,
                AM = m.AM,
                HCR = m.HCR,
                Product_Group = m.Product_Group,
                Product = m.Product,
                Target_QTY = m.Target_QTY,
                Sales_QTY = m.Target_QTY,
                Target_Amount = Math.Round((double)m.Target_Amount, 4),
                Sales_Amount = Math.Round((double)m.Sales_Amount, 4),
                Percent = (m.Target_Amount == 0 ? "" : Math.Round((double)((m.Sales_Amount / m.Target_Amount) * 100), 4).ToString())
            }).AsQueryable();
            if (positionID == 1 || positionID == null)
            {
                Headers = new string[] { "SM", "AM", "HCR", "Product_Group", "Product", "Target_QTY", "Sales_QTY", "Target_Amount", "Sales_Amount", "Percent" };
                HeaderTitles = new string[] { "SM", "AM", "HCR", "Product Group", "Product", "Bud QTY", "Ach QTY", "Bud Vlu", "Ach Vlu", "%" };
            }
            else if (positionID == 2)
            {
                Headers = new string[] { "SM", "AM", "Product_Group", "Product", "Target_QTY", "Sales_QTY", "Target_Amount", "Sales_Amount", "Percent" };
                HeaderTitles = new string[] { "SM", "AM", "Product Group", "Product", "Bud QTY", "Ach QTY", "Bud Vlu", "Ach Vlu", "%" };
            }
            else if (positionID == 3)
            {
                Headers = new string[] { "SM", "Product_Group", "Product", "Target_QTY", "Sales_QTY", "Target_Amount", "Sales_Amount", "Percent" };
                HeaderTitles = new string[] { "SM", "Product Group", "Product", "Bud QTY", "Ach QTY", "Bud Vlu", "Ach Vlu", "%" };
            }
            else
            {
                Headers = new string[] { "Country", "Product_Group", "Product", "Target_QTY", "Sales_QTY", "Target_Amount", "Sales_Amount", "Percent" };
                HeaderTitles = new string[] { "Country", "Product Group", "Product", "Bud QTY", "Ach QTY", "Bud Vlu", "Ach Vlu", "%" };
            }
            //Call Generic Export PDF method
            byte[] result = ExportPdf_BudAndAchByHCR(
                request,
                list,
                Headers,
                HeaderTitles,positionID); ;

            //Response carrying PDF file generated from byte-array
            return File(result, "application/pdf", "BudAndAchByHCR Report.pdf");
        }
        public static byte[] ExportPdf_BudAndAchByHCR([DataSourceRequest] DataSourceRequest request, IQueryable entitiesQueryable, string[] propertyNames, string[] labels, int? positionid)
        {
            request.PageSize = 0;
            IEnumerable entities = entitiesQueryable.ToDataSourceResult(request).Data;
            //step 0: check correctness of all property names
            var entityType = entities.GetType().GetGenericArguments()[0];
            foreach (var propertyName in propertyNames)
            {
                var property = entityType.GetProperty(propertyName);
                if (property == null)
                    return null;
            }

            // step 1: creation of a document-object
            var document = new Document(PageSize.A1, 1, 1, 10, 10);

            //var arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "simpfxo.ttf");
            //var arialFontPath = "C:\Users\\akram.elkorashy\\Downloads\\ARIALUNI.TTF";
            //var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            //var arialFontPath = Path.Combine(currentDirectory, ".\\assets\\fonts\\ARIALUNI.TTF");
            //var arialFontPath = "ARIALUNI.TTF";
            string dllPath = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            //int indexStartDelete = dllPath.LastIndexOf("/bin/");
            //string projectPath = dllPath.Substring(0, indexStartDelete);
            string fontpath = System.Web.HttpContext.Current.Server.MapPath("~/assets/fonts/");
            //string arialFontPath = projectPath +"/assets/fonts/ARIALUNI.TTF";
            //arialFontPath = "C:\\Users\\akram.elkorashy\\Downloads\\ARIALUNI.TTF";

            var arialBaseFont = BaseFont.CreateFont(string.Format("{0}ARIALUNI.ttf", fontpath), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            var arialFont = new Font(arialBaseFont);
            //string arialuniTff = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");

            ////Register the font with iTextSharp
            //iTextSharp.text.FontFactory.Register(arialuniTff);

            ////Create a new stylesheet
            //iTextSharp.text.html.simpleparser.StyleSheet ST = new iTextSharp.text.html.simpleparser.StyleSheet();
            ////Set the default body font to our registered font's internal name
            //ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FONT, "Arial Unicode MS");
            ////Set the default encoding to support Unicode characters
            //ST.LoadTagStyle(HtmlTags.BODY, HtmlTags., Base Font.IDENTITY_H);

            ////Parse our HTML using the stylesheet created above
            //List<IElement> list = HTMLWorker.ParseToList(new StringReader(resultCache), ST);

            //step 2: we create a memory stream that listens to the document
            var output = new MemoryStream();
            PdfWriter.GetInstance(document, output);
            //step 3: we open the document
            document.Open();

            //step 4: we add content to the document
            var numOfColumns = labels.Length;
            var dataTable = new PdfPTable(numOfColumns);

            dataTable.DefaultCell.Padding = 3;

            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            dataTable.DefaultCell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            float[] columnWidths = new float[numOfColumns];
            float[] arr = new float[] { };

            if (positionid == 1 || positionid == null)
            {
                arr = new float[] { 200f, 200f, 200f, 100f, 100f, 75f, 75f, 75f, 75f, 75f };

            }
            else if (positionid == 2)
            {
                arr = new float[] { 200f, 200f,100f,100f, 75f, 75f, 75f, 75f, 75f};

            }
            else if (positionid == 3)
            {
                arr = new float[] { 200f,100f,100f, 75f, 75f, 75f, 75f, 75f};

            }
            else
            {
                arr = new float[] { 100f,100f,100f, 75f, 75f, 75f, 75f, 75f };
            }
            for (int i = 0; i < numOfColumns; i++)
            {
                columnWidths[i] = arr[i];
            }
            dataTable.TotalWidth = 1600;
            dataTable.LockedWidth = true;
            dataTable.SetWidths(columnWidths);
            //dataTable.DefaultCell.MinimumHeight = 45.0f;

            // Adding headers
            foreach (var header in labels)
                dataTable.AddCell(new Phrase(header, arialFont));

            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;

            foreach (var entity in entities)
            {
                foreach (var propertyName in propertyNames)
                {
                    var property = entityType.GetProperty(propertyName);
                    var propGetter = property.GetGetMethod();
                    var propertyValue = propGetter.Invoke(entity, null);
                    if (propertyValue != null)
                        dataTable.AddCell(new Phrase(propertyValue.ToString(), arialFont));
                    else
                        dataTable.AddCell("");
                }
            }
            foreach (var header in labels)
                dataTable.AddCell("");
            //dataTable.
            // Add table to the document
            document.Add(dataTable);
            //This is important don't forget to close the document
            document.Close();

            return output.ToArray();
            //return controller.File(output.ToArray(), "application/pdf", fileName);
        }
    }
}

