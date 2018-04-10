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
    public class TerritoryToBrickByAreaController : Controller
    {
        private readonly TerritoryToBrickByAreaService _territorytobrickbyarea = new TerritoryToBrickByAreaService();


        // GET: TerritotyToBrickByArea
        public ActionResult Index()
        {
            ViewBag.CountriesDL = _territorytobrickbyarea.GetCountries();

            return View("");
        }

        public ActionResult Periods_Read(int? countryID)
        {
            try
            {
                var result = _territorytobrickbyarea.GetPeriods(countryID);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ActionResult TerritoryToBrickbyArea_Read(DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            try
            {
                var result = _territorytobrickbyarea.GetReportData(countryID, fromPeriodID, toPeriodID).ToDataSourceResult(request);
                var returnresult = Json(result, JsonRequestBehavior.AllowGet);
                returnresult.MaxJsonLength = int.MaxValue;
                return returnresult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Export

        public static byte[] ExportPdfGeneric([DataSourceRequest]
                                              DataSourceRequest request,
            IQueryable entitiesQueryable, string[] propertyNames, string[] labels)
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
            var document = new Document(PageSize.A4, 10, 10, 10, 10);

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

        public static byte[] ExportXlsGeneric([DataSourceRequest]
                                              DataSourceRequest request,
            IQueryable entitiesQueryable, string[] propertyNames, string[] labels)
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
            //Populate the sheet with values from the grid data
            foreach (var entity in entities)
            {
                //Create a new row
                var row = sheet.CreateRow(rowNumber++);

                for (int columnIndex = 0; columnIndex < propertyNames.Length; columnIndex++)
                {
                    var propertyName = propertyNames[columnIndex];
                    var property = entityType.GetProperty(propertyName);
                    var propGetter = property.GetGetMethod();
                    var propertyValue = propGetter.Invoke(entity, null);
                    if (propertyValue != null)
                    {
                        var str = propertyValue.ToString();
                        int valInt;
                        double valDouble;
                        if (int.TryParse(str, out valInt))
                            row.CreateCell(columnIndex).SetCellValue(valInt);
                        else if (double.TryParse(str, out valDouble))
                            row.CreateCell(columnIndex).SetCellValue(valDouble);
                        else if (propertyValue is DateTime)
                            row.CreateCell(columnIndex).SetCellValue(propertyValue.ToString());
                        else
                            row.CreateCell(columnIndex).SetCellValue(str);
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

        #endregion

     
        public FileResult ExportXls([DataSourceRequest]
                                    DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            var data = _territorytobrickbyarea.GetReportData(countryID, fromPeriodID, toPeriodID).ToList();
            var list = data.Select(m => new
                         {
                             Period_Year = m.Period_Year.ToString(),
                             Profile_Code = m.Profile_Code,
                             Medical_Rep = m.Medical_Rep,
                             Province = m.Province,
                             City = m.City,
                             Brick_Code = m.Brick_Code,
                             Brick = m.Brick
                         }).AsQueryable();

            byte[] result = ExportBase.ExportXlsGeneric(
                request,
                list,
          new string[] { "Period_Year", "Profile_Code", "Medical_Rep", "Province", "City", "Brick_Code", "Brick" },
                new string[] { "Period", "Area", "HCR", "Province", "City", "Brick Code", "Brick" });
            return File(result, //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "TerritoryToBrickByArea Report.xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportPdf([DataSourceRequest]
                                    DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            var data = _territorytobrickbyarea.GetReportData(countryID, fromPeriodID, toPeriodID).ToList();
            var list = data.Select(m => new
            {
                Period_Year = m.Period_Year.ToString(),
                Profile_Code = m.Profile_Code,
                Medical_Rep = m.Medical_Rep,
                Province = m.Province,
                City = m.City,
                Brick_Code = m.Brick_Code,
                Brick = m.Brick
            }).AsQueryable();

            //Call Generic Export PDF method
            byte[] result = ExportBase.ExportPdfGeneric(
                request,
                list,
                 new string[] { "Period_Year", "Profile_Code", "Medical_Rep", "Province", "City", "Brick_Code", "Brick" },
                new string[] { "Period", "Area", "HCR", "Province", "City", "Brick Code", "Brick" });

            //Response carrying PDF file generated from byte-array
            return File(result, "application/pdf", "TerritoryToBrickByArea Report.pdf");
        }
       

    }
}