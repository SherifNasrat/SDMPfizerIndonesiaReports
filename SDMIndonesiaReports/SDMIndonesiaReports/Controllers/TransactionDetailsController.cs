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
using System.Data;
namespace SDMIndonesiaReports.Controllers
{
    public class TransactionDetailsController : Controller
    {
        // GET: TransactionDetails
        private readonly TransactionDetailsService _TransactionDetails = new TransactionDetailsService();
        public ActionResult Index()
        {
            ViewBag.CountriesDL = _TransactionDetails.GetCountries();
            return View();
        }
        public ActionResult Periods_Read(int? countryID)
        {
            try
            {
                var result = _TransactionDetails.GetPeriods(countryID);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ActionResult TransactionDetails_Read(DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            //try
            //{
            var result = _TransactionDetails.GetReportData(countryID, fromPeriodID, toPeriodID).ToDataSourceResult(request);

            var res = Json(result, JsonRequestBehavior.AllowGet);
            res.MaxJsonLength = int.MaxValue;
            return res;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
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
            var data = _TransactionDetails.GetReportData(countryID, fromPeriodID, toPeriodID).ToList();
            //var list = data.Select(m => new
            //{
            //    Date = m.Date.ToString(),
            //    Invoice = "N/A",
            //    Product_Name = m.Product_Name.ToString(),
            //    Cust_Code = m.Cust_Code.ToString(),
            //    Cust_Name = m.Cust_Name.ToString(),
            //    Sector = m.Sector.ToString(),
            //    Address = m.Address.ToString(),
            //    Province = m.Province.ToString(),
            //    City = m.City.ToString(),
            //    Brick = m.Brick.ToString(),
            //    SM = m.SM.ToString(),
            //    AM = m.AM.ToString(),
            //    HCR = m.HCR.ToString(),
            //    Profile = m.Profile.ToString(),
            //    Qty = m.Qty,
            //    Value = m.Value
            //}).AsQueryable();
            DataTable mTable = new DataTable();

            string[] tableHeaders = new string[] { "Date", "Invoice", "Product", "Cust Code", "Cust Name", "Sector", "Address", "Province", "City", "Brick", "SM", "AM", "HCR", "Profile", "Qty", "Value" };
            for (int i = 0; i < tableHeaders.Length; i++)
            {
                mTable.Columns.Add(tableHeaders[i]);
            }

            for (int i = 0; i < data.Count(); i++)
            {
                string[] tempArr = new string[16];
                tempArr[0] = data[i].Date.ToString();
                tempArr[1] = data[i].Invoice;
                tempArr[2] = data[i].Product_Name;
                tempArr[3] = data[i].Cust_Code;
                tempArr[4] = data[i].Cust_Name;
                tempArr[5] = data[i].Sector;
                tempArr[6] = data[i].Address;
                tempArr[7] = data[i].Province;
                tempArr[8] = data[i].City;
                tempArr[9] = data[i].Brick;
                tempArr[10] = data[i].SM;
                tempArr[11] = data[i].AM;
                tempArr[12] = data[i].HCR;
                tempArr[13] = data[i].Profile;
                tempArr[14] = data[i].Qty.ToString();
                tempArr[15] = Math.Round((double)data[i].Value,4).ToString();
                mTable.Rows.Add(tempArr);

            }
            byte[] result = ExportXls_TransactionDetails(
                request,
                mTable,
       new string[] { "Date", "Invoice", "Product_Name", "Cust_Code", "Cust_Name", "Sector", "Address", "Province", "City", "Brick", "SM", "AM", "HCR", "Profile", "Qty", "Value" },
                new string[] { "Date", "Invoice", "Product", "Cust Code", "Cust Name", "Sector", "Address", "Province", "City", "Brick", "SM", "AM", "HCR", "Profile", "Qty", "Value" });
            return File(result, //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "TransactionDetails Report.xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportPdf([DataSourceRequest]
                                    DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            var data = _TransactionDetails.GetReportData(countryID, fromPeriodID, toPeriodID).ToList();
            //var list = data.Select(m => new
            //{
            //    Date = m.Date.ToString(),
            //    Invoice = "N/A",
            //    Product_Name = m.Product_Name.ToString(),
            //    Cust_Code = m.Cust_Code.ToString(),
            //    Cust_Name = m.Cust_Name.ToString(),
            //    Sector = m.Sector.ToString(),
            //    Address = m.Address.ToString(),
            //    Province = m.Province.ToString(),
            //    City = m.City.ToString(),
            //    Brick = m.Brick.ToString(),
            //    SM = m.SM.ToString(),
            //    AM = m.AM.ToString(),
            //    HCR = m.HCR.ToString(),
            //    Profile = m.Profile.ToString(),
            //    Qty = m.Qty,
            //    Value = m.Value
            //}).ToList();
            DataTable mTable = new DataTable();

            string[] tableHeaders = new string[] { "Date", "Invoice", "Product", "Cust Code", "Cust Name", "Sector", "Address", "Province", "City", "Brick", "SM", "AM", "HCR", "Profile", "Qty", "Value" };
            for (int i = 0; i < tableHeaders.Length; i++)
            {
                mTable.Columns.Add(tableHeaders[i]);
            }

            for (int i = 0; i < data.Count(); i++)
            {
                string[] tempArr = new string[16];
                tempArr[0] = data[i].Date.ToString();
                tempArr[1] = data[i].Invoice;
                tempArr[2] = data[i].Product_Name;
                tempArr[3] = data[i].Cust_Code;
                tempArr[4] = data[i].Cust_Name;
                tempArr[5] = data[i].Sector;
                tempArr[6] = data[i].Address;
                tempArr[7] = data[i].Province;
                tempArr[8] = data[i].City;
                tempArr[9] = data[i].Brick;
                tempArr[10] = data[i].SM;
                tempArr[11] = data[i].AM;
                tempArr[12] = data[i].HCR;
                tempArr[13] = data[i].Profile;
                tempArr[14] = data[i].Qty.ToString();
                tempArr[15] = Math.Round((double)data[i].Value, 4).ToString();
                mTable.Rows.Add(tempArr);

            }
            //Call Generic Export PDF method
            byte[] result = ExportPdf_TransactionDetails(
                request,
                mTable,
                 new string[] { "Date", "Invoice", "Product_Name", "Cust_Code", "Cust_Name", "Sector", "Address", "Province", "City", "Brick", "SM", "AM", "HCR", "Profile", "Qty", "Value" },
                new string[] { "Date", "Invoice", "Product", "Cust Code", "Cust Name", "Sector", "Address", "Province", "City", "Brick", "SM", "AM", "HCR", "Profile", "Qty", "Value" });

            //Response carrying PDF file generated from byte-array
            return File(result, "application/pdf", "TransactionDetails Report.pdf");
        }
        public static byte[] ExportPdf_TransactionDetails([DataSourceRequest] DataSourceRequest request, DataTable entitiesQueryable, string[] propertyNames, string[] labels)
        {
            request.PageSize = 0;
            IEnumerable entities = entitiesQueryable.ToDataSourceResult(request).Data;
            //step 0: check correctness of all property names

            // step 1: creation of a document-object
            var document = new Document(PageSize.A0, 1, 1, 10, 10);

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
            float[] arr = new float[] { 300f, 300f, 300f, 300f, 300f, 300f, 300f, 300f, 300f, 300f, 300f, 300f, 300f, 300f, 300f, 300f };
            for (int i = 0; i < numOfColumns; i++)
            {
                columnWidths[i] = arr[i];
            }
            dataTable.TotalWidth = 1950;
            dataTable.LockedWidth = true;
            dataTable.SetWidths(columnWidths);
            //dataTable.DefaultCell.MinimumHeight = 45.0f;

            // Adding headers
            foreach (var header in labels)
                dataTable.AddCell(new Phrase(header, arialFont));

            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;

            for (int i = 0; i < entitiesQueryable.Rows.Count; i++)
            {
                for (int j = 0; j < entitiesQueryable.Columns.Count; j++)
                {

                    if (entitiesQueryable.Rows[i][j] != null)
                        dataTable.AddCell(new Phrase(entitiesQueryable.Rows[i][j].ToString(), arialFont));
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
        public static byte[] ExportXls_TransactionDetails([DataSourceRequest] DataSourceRequest request, DataTable entitiesQueryable, string[] propertyNames, string[] labels)
        {
            request.PageSize = 0;
            //IEnumerable entities = entitiesQueryable.ToDataSourceResult(request).Data;

            //step 0: check correctness of all property names
            //var entityType = entities.GetType().GetGenericArguments()[0];
            //foreach (var propertyName in propertyNames)
            //{
            //    var property = entityType.GetProperty(propertyName);
            //    if (property == null)
            //        return null;
            //}

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
            for (int i = 0; i < entitiesQueryable.Rows.Count; i++)
            {
                //Create a new row
                var row = sheet.CreateRow(rowNumber++);

                for (int columnIndex = 0; columnIndex < propertyNames.Length; columnIndex++)
                {


                    string str = entitiesQueryable.Rows[i][columnIndex].ToString();
                    int valInt;
                    double valDouble;
                    if (int.TryParse(str, out valInt))
                        row.CreateCell(columnIndex).SetCellValue(valInt);
                    else if (double.TryParse(str, out valDouble))
                        row.CreateCell(columnIndex).SetCellValue(valDouble);
                    else
                        row.CreateCell(columnIndex).SetCellValue(str);


                }
            }

            //Write the workbook to a memory stream
            MemoryStream output = new MemoryStream();
            workbook.Write(output);

            return output.ToArray();
        }

    }

}