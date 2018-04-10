﻿using Kendo.Mvc.UI;
using SDMIndonesiaReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using SDMIndonesiaReports.Models.CustomModels;
using SDMIndonesiaReports.Helpers;
using System.Data;
using iTextSharp.text;
using System.Reflection;
using iTextSharp.text.pdf;
using System.IO;
using NPOI.HSSF.UserModel;


namespace SDMIndonesiaReports.Controllers
{
    public class TerritoryToBrickByBrickController : Controller
    {
        private readonly TerritoryToBrickByBrickService _territoryByBrickService = new TerritoryToBrickByBrickService();
        // GET: TerritoryToBrickByBrick
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult Countries_Read()
        {
            try
            {
                var listOfCountries = _territoryByBrickService.GetCountries();
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
                if(countryID != null)
                {
                    var result = _territoryByBrickService.GetPeriods(countryID);
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    empty.Add(new SelectListItem() { Text = "Select From Period", Value = null });
                    return Json(empty, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //[HttpPost]
        public ActionResult SalesTeams_Read(int? countryID)
        {
            try
            {
                var result = _territoryByBrickService.getSalesTeams(countryID);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpPost]
        public ActionResult TerritoryToBrickbyBrick_Read(int? countryID,int? fromPeriodID,int? toPeriodID)
        {
            try
            {
                var result = _territoryByBrickService.getReportData(countryID, fromPeriodID, toPeriodID);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
              
              
        }
        public FileResult ExportXls([DataSourceRequest]
                                    DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            string[,] data = _territoryByBrickService.getReportDataExport(countryID, fromPeriodID, toPeriodID);
            var columnNames = _territoryByBrickService.getSalesTeams(countryID);
            DataTable myTable = new DataTable();
            //Column Headers
            string[] tableHeaders = new string[columnNames.SalesTeamNames.Count()+2];
            tableHeaders[0] = "BrickCode";
            tableHeaders[1] = "Brick";
            for(int i=0;i<columnNames.SalesTeamNames.Count();i++)
            {
                tableHeaders[i + 2] = columnNames.SalesTeamNames[i];
            }
            for(int i=0;i<columnNames.SalesTeamNames.Count()+2;i++)
            {
                myTable.Columns.Add(tableHeaders[i]);
            }
            for(int i=0;i<data.GetLength(0);i++)
            {
                
                string [] tempArr = new string[data.GetLength(1)];
                for(int j=0;j<data.GetLength(1);j++)
                {
                    tempArr[j] = data[i,j];
                }
                
                myTable.Rows.Add(tempArr);
            }
         

            byte[] result = ExportXlsDynamicColumnsGeneric(
                request,
                myTable,
          tableHeaders,
                tableHeaders);
            return File(result, //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "Territory to Brick by Brick.xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportPdf([DataSourceRequest]
                                    DataSourceRequest request, int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            string[,] data = _territoryByBrickService.getReportDataExport(countryID, fromPeriodID, toPeriodID);
            var columnNames = _territoryByBrickService.getSalesTeams(countryID);
            DataTable myTable = new DataTable();
            //Column Headers
            string[] tableHeaders = new string[columnNames.SalesTeamNames.Count() + 2];
            tableHeaders[0] = "BrickCode";
            tableHeaders[1] = "Brick";
            for (int i = 0; i < columnNames.SalesTeamNames.Count(); i++)
            {
                tableHeaders[i + 2] = columnNames.SalesTeamNames[i];
            }
            for (int i = 0; i < columnNames.SalesTeamNames.Count() + 2; i++)
            {
                myTable.Columns.Add(tableHeaders[i]);
            }
            for (int i = 0; i < data.GetLength(0); i++)
            {

                string[] tempArr = new string[data.GetLength(1)];
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    tempArr[j] = data[i, j];
                }

                myTable.Rows.Add(tempArr);
            }

            //Call Generic Export PDF method
            byte[] result =ExportPdfDynamicColumnsGeneric(
                request,
                myTable,
                 tableHeaders,
                tableHeaders);

            //Response carrying PDF file generated from byte-array
            return File(result, "application/pdf", "Territory to Brick by Brick Report.pdf");
        }
        public static byte[] ExportXlsDynamicColumnsGeneric([DataSourceRequest] DataSourceRequest request, DataTable entitiesQueryable, string[] propertyNames, string[] labels)
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
        public static byte[] ExportPdfDynamicColumnsGeneric([DataSourceRequest] DataSourceRequest request, DataTable entitiesQueryable, string[] propertyNames, string[] labels)
        {
            request.PageSize = 0;
            // step 1: creation of a document-object
           
            //var document = new Document(PageSize.A0.Rotate(), 0, 0, 0, 0);
            var document = new Document(new Rectangle(14400, 14400, 90), 0, 0, 0, 0);

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

            //Create x Tables, each one contains 7 Columns, 2 for Bricks , 5 for SalesTeams
            //Calculate x: x is the number of tables => #SalesTeams/5 ceiling
            int numOfColumnsPerPage = 5;
            int numberOfTables = (numOfColumns - 2) / numOfColumnsPerPage;

            var dataTable = new PdfPTable(numOfColumns);

            dataTable.DefaultCell.Padding = 3;

            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            float[] columnWidths = new float[entitiesQueryable.Columns.Count];
            for (int i = 0; i < entitiesQueryable.Columns.Count; i++) //150
            {
                columnWidths[i] = 100f;
            }
            dataTable.DefaultCell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            dataTable.TotalWidth = 14400f;
            dataTable.LockedWidth = true;
            columnWidths[0] = 25f;
            columnWidths[1] = 50f;
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
       
    }
}