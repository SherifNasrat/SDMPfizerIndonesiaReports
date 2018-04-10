using SDMIndonesiaReportsDB.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDMIndonesiaReports.Services
{
    public class TransactionSummarybyChannelService : ReportBase
    {
        //public string[,] getReportData(int? countryID, int? fromPeriodID, int? toPeriodID)
        //{
        //    if (fromPeriodID == null)
        //    {
        //        fromPeriodID = GetCurrentPeriod(countryID);
        //    }
        //    if (toPeriodID == null)
        //    {
        //        toPeriodID = GetCurrentPeriod(countryID);
        //    }
        //    string[,] reportData;
        //    var staticPart = context.SP_TransactionSummarybyChannel_Helper(countryID, fromPeriodID, toPeriodID).ToList();
        //    var sectors = context.CustomerSectors.Where(s => s.FK_CountryId == countryID).Select(s => new { SectorID = s.CustomerSectorID, SectorName = s.CustomerSectorName }).Distinct().OrderBy(s => s.SectorName).ToList();
        //    int rowCount = staticPart.Count;
        //    //9 Fixed Columns, N-Sector Columns, 1 Qty Total Column 1 Amount Column
        //    int columnCount = sectors.Count + 11;
        //    reportData = new string[rowCount, columnCount];
        //    //Set Rows
        //    for (int i = 0; i < rowCount; i++)
        //    {
        //        reportData[i, 0] = staticPart[i].Province;
        //        reportData[i, 1] = staticPart[i].City;
        //        reportData[i, 2] = staticPart[i].Brick;
        //        reportData[i, 3] = staticPart[i].Product_Group;
        //        reportData[i, 4] = staticPart[i].SM;
        //        reportData[i, 5] = staticPart[i].AM;
        //        reportData[i, 6] = staticPart[i].HCR;
        //        reportData[i, 7] = staticPart[i].ProfileName;
        //        reportData[i, 8] = staticPart[i].Product;

        //    }
        //    //Set Sector Qty
        //    for (int i = 0; i < rowCount; i++)
        //    {
        //        var QtyPerRecord = context.SP_TransactionSummarybyChannel_getSectorsQtyPerRecord((int?)staticPart[i].Brick_ID, (int?)staticPart[i].Profile_ID, (int?)staticPart[i].DosageForm_ID, (int?)staticPart[i].HCR_ID, countryID, fromPeriodID, toPeriodID).ToList();
        //        decimal? QtyTotalPerRecord = 0;
        //        for (int j = 0; j < columnCount - 11; j++)
        //        {
        //            reportData[i, j + 9] = QtyPerRecord[j].Qty.ToString();
        //            QtyTotalPerRecord += QtyPerRecord[j].Qty;

        //        }
        //        reportData[i, columnCount - 2] = QtyTotalPerRecord.ToString();
        //    }
        //    //Set Amount
        //    for (int i = 0; i < rowCount; i++)
        //    {
        //        reportData[i, columnCount - 1] = staticPart[i].Amount.ToString();
        //    }
        //    return reportData;
        //}
        public JsonResult getJsonReportData(string[,] reportData, int rows, int columns, int? countryID)
        {

            if (reportData.Length == 0)
            {
                string[,] emptyReportData = new string[1, 1];
                emptyReportData[0, 0] = "";
                return new JsonResult()
                {

                    Data = new
                    {
                        dataArray = reportData,
                        sectorsNames = 0,
                        rowCount = rows,
                        columnCount = columns
                    },


                    JsonRequestBehavior = JsonRequestBehavior.AllowGet

                };

            }
            else
            {
                var sectors = context.CustomerSectors.Where(s => s.FK_CountryId == countryID).Select(s =>s.CustomerSectorName).Distinct().OrderBy(s=>s).ToList();
                return new JsonResult()
                {

                    Data = new
                    {
                        dataArray = reportData,
                        sectorsNames = sectors,
                        rowCount = rows,
                        columnCount = columns
                    },


                    JsonRequestBehavior = JsonRequestBehavior.AllowGet

                };
            }

        }
        public List<string> getSectorNames (int? countryID)
        {
            var sectors = context.CustomerSectors.Where(s => s.FK_CountryId == countryID).Select(s=>s.CustomerSectorName).Distinct().OrderBy(s=>s).ToList();
            return sectors;
        }

        public string[,] getReportData(int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            if(fromPeriodID == null)
            {
                fromPeriodID = GetCurrentPeriod(countryID);
            }
            if (toPeriodID == null)
            {
                toPeriodID = GetCurrentPeriod(countryID);
            }
            string[,] reportData;
            //var staticPart = context.SP_TransactionSummarybyChannel_Helper(countryID, fromPeriodID, toPeriodID).ToList();
            //var sectors = context.CustomerSectors.Where(s => s.FK_CountryId == countryID).Select(s => new { SectorID = s.CustomerSectorID, SectorName = s.CustomerSectorName }).Distinct().OrderBy(s => s.SectorName).ToList();
            //int rowCount = staticPart.Count;
            ////9 Fixed Columns, N-Sector Columns, 1 Qty Total Column 1 Amount Column
            //int columnCount = sectors.Count + 11;
            //reportData = new string[rowCount, columnCount];
            ////Set Rows
            //for (int i = 0; i < rowCount; i++)
            //{
            //    reportData[i, 0] = staticPart[i].Province;
            //    reportData[i, 1] = staticPart[i].City;
            //    reportData[i, 2] = staticPart[i].Brick;
            //    reportData[i, 3] = staticPart[i].Product_Group;
            //    reportData[i, 4] = staticPart[i].SM;
            //    reportData[i, 5] = staticPart[i].AM;
            //    reportData[i, 6] = staticPart[i].HCR;
            //    reportData[i, 7] = staticPart[i].ProfileName;
            //    reportData[i, 8] = staticPart[i].Product;

            //}
            SqlConnection m_connection = new SqlConnection();
            string entityConnectionString = ConfigurationManager.ConnectionStrings["SDW_TargetingEntities"].ConnectionString;
            string providerConnectionString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
            m_connection.ConnectionString = providerConnectionString;
            m_connection.Open();
            SqlCommand command;
            SqlDataReader reader;
            command = m_connection.CreateCommand();
            //Query String
            command.CommandText = "exec SP_TransactionSummaryByChannel_Sectors "+countryID+","+fromPeriodID+","+toPeriodID;
            reader = command.ExecuteReader(CommandBehavior.Default);
            DataTable m_Table = new DataTable("ReportSectorsQty");
            m_Table.Load(reader);
            m_connection.Close();
            m_Table.Columns[m_Table.Columns.Count - 1].ReadOnly = false;
            //Rounding Total Qty and Amount in DataTable
            for (int i = 0; i < m_Table.Rows.Count;i++)
            {
                //m_Table.Rows[i][m_Table.Columns.Count - 2] = Math.Round(Convert.ToDouble(m_Table.Rows[i][m_Table.Columns.Count - 2]), 4);
                m_Table.Rows[i][m_Table.Columns.Count - 1] = Math.Round(Convert.ToDouble(m_Table.Rows[i][m_Table.Columns.Count - 1]), 4);
            }
                //Set Sector Qty
                reportData = new string[m_Table.Rows.Count, m_Table.Columns.Count + 1];
            for (int i = 0; i < m_Table.Rows.Count; i++)
            {
               
                decimal? QtyTotalPerRecord = 0;
                for (int j = 0; j < m_Table.Columns.Count; j++)
                {
                    if(j!=m_Table.Columns.Count-1)
                    reportData[i, j] = m_Table.Rows[i][j].ToString();
                    else
                        reportData[i, m_Table.Columns.Count] = m_Table.Rows[i][j].ToString();
                    if (j > 8 && j != m_Table.Columns.Count - 1 && m_Table.Rows[i][j].ToString()!="")
                    QtyTotalPerRecord += (decimal?)m_Table.Rows[i][j];

                }
                reportData[i, m_Table.Columns.Count - 1] = Math.Round((double)QtyTotalPerRecord,4).ToString();
            }

            //Set Amount
            //for (int i = 0; i < rowCount; i++)
            //{
            //    reportData[i, columnCount - 1] = staticPart[i].Amount.ToString();
            //}
            return reportData;
        }
    }
}