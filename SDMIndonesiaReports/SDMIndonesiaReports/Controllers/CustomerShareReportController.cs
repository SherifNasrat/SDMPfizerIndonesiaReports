using Kendo.Mvc.UI;
using SDMIndonesiaReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using SDMIndonesiaReports.Helpers;
using System.Collections;
using NPOI.HSSF.UserModel;
using System.IO;
using SDMIndonesiaReports.Models.CustomModels;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SDMIndonesiaReports.Controllers
{
    public class CustomerShareReportController : Controller
    {
        private readonly CustomerShareReportService _customerShareReportService = new CustomerShareReportService();
        // GET: CustomerShareReport
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Countries_Read()
        {
            try
            {
                var listOfCountries = _customerShareReportService.GetCountries();
                return Json(listOfCountries, JsonRequestBehavior.AllowGet);
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
                var listOfPeriods = _customerShareReportService.GetPeriods(countryID);
                return Json(listOfPeriods, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
      
        public ActionResult CustomerShareReport_Read([DataSourceRequest] DataSourceRequest request, int? countryID, int? fromPeriodID,int? toPeriodID, string []profileIDs)
            {
            try
            {

                var result =  _customerShareReportService.GetReportData(countryID, fromPeriodID,toPeriodID, profileIDs).ToDataSourceResult(request);
                var res = Json(result, JsonRequestBehavior.AllowGet);
                res.MaxJsonLength = int.MaxValue;
                return res;
               // return View();
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public ActionResult Profiles_Read (int? countryID,int? fromPeriodID,int? toPeriodID)
        {
            try
            {
                var result = _customerShareReportService.getProfiles(countryID,fromPeriodID,toPeriodID);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public FileResult ExportPdf([DataSourceRequest] DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID, string[] profileIDs)
        {
            List<CustomerShareReportVM> data = _customerShareReportService.GetReportData(countryID, fromPeriodID, toPeriodID, profileIDs).ToList();


            //Call Generic Export PDF method
            byte[] result = ExportPdfCustomerShareReport(
                request,
                data,
                 new string[] { " Province", "City", "Brick", "PfizerCustCode", "pfizerCustName", "DosageFormCode", "Product", "AreaCode", "Percentage" },
                 new string[] { " Province", "City", "Brick", "Pfizer Cust Code", "pfizer Cust Name", "DosageForm Code", "Product", "AreaCode", "Percentage" });

            //Response carrying PDF file generated from byte-array
            return File(result, "application/pdf", "Cusomter Share Report.pdf");
        }
        public FileResult ExportXls([DataSourceRequest] DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID, string[] profileIDs)
        {
            List<CustomerShareReportVM> data = _customerShareReportService.GetReportData(countryID, fromPeriodID, toPeriodID, profileIDs).ToList();


            //Call Generic Export Xls method
            byte[] result = ExportXlsCustomerShareReport(
                request,
                data,
                 new string[] { " Province", "City", "Brick", "PfizerCustCode", "pfizerCustName", "DosageFormCode", "Product", "AreaCode", "Percentage" },
                new string[] { " Province", "City", "Brick", "Pfizer Cust Code", "pfizer Cust Name", "DosageForm Code", "Product", "AreaCode", "Percentage" });

            //Response carrying PDF file generated from byte-array
            return File(result, //The binary data of the XLS file
            "application/vnd.ms-excel", //MIME type of Excel files
            "Customer Share Report.xls");
        }
        public static byte[] ExportXlsCustomerShareReport([DataSourceRequest] DataSourceRequest request, List<CustomerShareReportVM> entitiesQueryable, string[] propertyNames, string[] labels)
        {
            request.PageSize = 0;
           
            
            //step 0: check correctness of all property names
          

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();

            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            var numOfColumns = labels.Length;

            //Create a header row
            var headerRow = sheet.CreateRow(0);

            //Set the column names in the header row
            for (int columnIndex = 0; columnIndex < numOfColumns; columnIndex++)
            {
                headerRow.CreateCell(columnIndex).SetCellValue(labels[columnIndex]);

                //(Optional) set the width of the columns
                sheet.SetColumnWidth(columnIndex, 50 * 256);
            }

            //(Optional) freeze the header row so it is not scrolled
            sheet.CreateFreezePane(0, 1, 0, 1);

            int rowNumber = 1;

            int entityRows = entitiesQueryable.Count();
            //Populate the sheet with values from the grid data
            for(int i=0;i<entityRows;i++)
            {
                //Create a new row
                var row = sheet.CreateRow(rowNumber++);
                string [] fields = new string[9];
                fields[0] = entitiesQueryable[i].Province_Name;
                fields[1] = entitiesQueryable[i].City_Name;
                fields[2] = entitiesQueryable[i].Brick_Name;
                fields[3] = entitiesQueryable[i].Pfizer_Customer_Code;
                fields[4] = entitiesQueryable[i].Pfizer_Customer_Name;
                fields[5] = entitiesQueryable[i].Dosage_Form_Code;
                fields[6] = entitiesQueryable[i].Dosage_Form_Name;
                fields[7] = entitiesQueryable[i].Area_Code;
                fields[8] = Math.Round(entitiesQueryable[i].Percentage,4).ToString();
                                    
                for (int columnIndex = 0; columnIndex < propertyNames.Length; columnIndex++)
                {

                    if (fields[columnIndex] != null)
                    {
                        
                        int valInt;
                        double valDouble;
                        if (int.TryParse(fields[columnIndex], out valInt))
                            row.CreateCell(columnIndex).SetCellValue(valInt);
                        else if (double.TryParse(fields[columnIndex], out valDouble))
                            row.CreateCell(columnIndex).SetCellValue(valDouble);
                        else
                            row.CreateCell(columnIndex).SetCellValue(fields[columnIndex]);
                    }
                    else
                        row.CreateCell(columnIndex).SetCellValue("");

                   
                }
            }

            //Write the workbook to a memory stream
            MemoryStream output = new MemoryStream();
            workbook.Write(output);

            return output.ToArray();
        }
        public static byte[] ExportPdfCustomerShareReport([DataSourceRequest] DataSourceRequest request, List<CustomerShareReportVM> entitiesQueryable, string[] propertyNames, string[] labels)
        {
            request.PageSize = 0;
           
            //step 0: check correctness of all property names
           

            // step 1: creation of a document-object
            var document = new Document(PageSize.A2.Rotate(), 10, 10, 10, 10);

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
            var numOfColumns = labels.Length;
            var dataTable = new PdfPTable(numOfColumns);

            dataTable.DefaultCell.Padding = 3;

            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            dataTable.DefaultCell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            float[] columnWidths = new float[numOfColumns];
            float[] arr = new float[] {200f,150f,150f,200f,200f,200f,150f,150f,200f };

            for (int i = 0; i < numOfColumns; i++)
            {
                columnWidths[i] = arr[i];
            }
            dataTable.TotalWidth = 1600;
            dataTable.LockedWidth = true;
            dataTable.SetWidths(columnWidths);
            int numOfRows = entitiesQueryable.Count();
            // Adding headers
            foreach (var header in labels)
                dataTable.AddCell(new Phrase(header, arialFont));
            for (int i = 0; i < numOfRows; i++)
            {
                string[] fields = new string[9];
                fields[0] = entitiesQueryable[i].Province_Name;
                fields[1] = entitiesQueryable[i].City_Name;
                fields[2] = entitiesQueryable[i].Brick_Name;
                fields[3] = entitiesQueryable[i].Pfizer_Customer_Code;
                fields[4] = entitiesQueryable[i].Pfizer_Customer_Name;
                fields[5] = entitiesQueryable[i].Dosage_Form_Code;
                fields[6] = entitiesQueryable[i].Dosage_Form_Name;
                fields[7] = entitiesQueryable[i].Area_Code;
                fields[8] = Math.Round(entitiesQueryable[i].Percentage, 4).ToString();
                for (int j = 0; j < numOfColumns; j++)
                {

                    if (fields[j] != null)

                        dataTable.AddCell(new Phrase(fields[j].ToString(), arialFont));
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
        public ActionResult Pdf_Export_Save_CustomerShareReport(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    }
}