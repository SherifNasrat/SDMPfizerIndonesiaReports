using SDMIndonesiaReports.Models.CustomModels;
using SDMIndonesiaReportsDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SDMIndonesiaReports.Services
{
    public class TerritoryToBrickByBrickService : ReportBase
    {
        public SalesTeamVM getSalesTeams(int? countryID)
        {
            try
            {

               List<string> ListOfSalesTeamsNames = (from s in context.SalesTeams
                                             join st in context.SalesTeamTypes
                                             on s.FK_SalesTeamTypeID equals st.SalesTeamTypeID
                                             where st.FK_CountryId == countryID
                                             select s.SalesTeamName).ToList<string>();

                    SalesTeamVM Teams = new SalesTeamVM();
                    Teams.SalesTeamNames = new List<string>(ListOfSalesTeamsNames);
                    return Teams; ;
                
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //public List<TerritoryToBrickByBrickVM> getReportData(int? countryID,int? fromPeriodID,int? toPeriodID)
        //{
        //    //Get all SalesTeams for that country
        //    var bricks = context.GeographicalAreas.Where(g => g.LevelsNo == 3 & g.FK_CountryID == countryID).Select(g => new { g.GeographicalAreaID, g.GeographicalAreaName, g.GeographicalAreaCode }).ToList();
        //    List<int> brickIDs = new List<int>();
        //    foreach(var brick in bricks)
        //    {
        //        brickIDs.Add(brick.GeographicalAreaID);
        //    }
        //    var teamsIDs = (from s in context.SalesTeams
        //                    join t in context.SalesTeamTypes
        //                    on s.FK_SalesTeamTypeID equals t.SalesTeamTypeID
        //                    where t.FK_CountryId == countryID
        //                    select s.SalesTeamID).ToList();
        //    List<TerritoryToBrickByBrickVM> result = new List<TerritoryToBrickByBrickVM>();
        //    for(int i=0;i<bricks.Count;i++)
        //    {
                
        //        TerritoryToBrickByBrickVM item = new TerritoryToBrickByBrickVM();
        //        item.BrickCode = bricks[i].GeographicalAreaCode;
        //        item.BrickName = bricks[i].GeographicalAreaName;
        //        item.ProfileCodes = new string[teamsIDs.Count];
        //        item.ProfileCodes = context.SP_TerritoryBrickbyBrick_Helper(bricks[i].GeographicalAreaID, countryID,fromPeriodID,toPeriodID).Select(p => p.ProfileCode).ToArray();
        //        result.Add(item);
        //    }
            
        //    return result;
        //}
        public JsonResult getReportData(int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            try
            {
                if (fromPeriodID == null)
                {
                    fromPeriodID = GetCurrentPeriod(countryID);
                }
                if(toPeriodID == null)
                {
                    toPeriodID = GetCurrentPeriod(countryID);
                }
                int? LowestLevel = context.Countries.Where(c => c.CountryId == countryID).Select(c => c.MaxLevelsNumber).FirstOrDefault();
                //Get all SalesTeams for that country
                //var bricks = context.GeographicalAreas.Where(g => g.LevelsNo == LowestLevel & g.FK_CountryID == countryID).Select(g => new { g.GeographicalAreaID, g.GeographicalAreaName, g.GeographicalAreaCode }).ToList();
                var bricks = context.SP_TerritorytoBrickbyBrick_getBricks(countryID, fromPeriodID, toPeriodID, LowestLevel).ToList();
                List<int> brickIDs = new List<int>();
                foreach (var brick in bricks)
                {
                    brickIDs.Add(brick.BrickID);
                }
                var teamsIDs = (from s in context.SalesTeams
                                join t in context.SalesTeamTypes
                                on s.FK_SalesTeamTypeID equals t.SalesTeamTypeID
                                where t.FK_CountryId == countryID
                                select s.SalesTeamID).ToList();
                var teamsNames = (from s in context.SalesTeams
                                  join t in context.SalesTeamTypes
                                  on s.FK_SalesTeamTypeID equals t.SalesTeamTypeID
                                  where t.FK_CountryId == countryID
                                  select s.SalesTeamName).ToList();
                List<TerritoryToBrickByBrickVM> result = new List<TerritoryToBrickByBrickVM>();
                for (int i = 0; i < brickIDs.Count(); i++)
                {

                    TerritoryToBrickByBrickVM item = new TerritoryToBrickByBrickVM();
                    item.BrickCode = bricks[i].BrickCode;
                    item.BrickName = bricks[i].Brick;
                    item.ProfileCodes = new string[teamsIDs.Count];
                    item.ProfileCodes = context.SP_TerritoryBrickbyBrick_Helper(bricks[i].BrickID, countryID, fromPeriodID, toPeriodID).Select(p => p.ProfileCode).ToArray();
                    result.Add(item);
                }
                //Case When: Bricks > 0 , Teams > 0  => Display data 
                //Case When: Bricks = 0 , Teams > 0  => Display column headers only
                //Case When: Bricks > 0 , Teams = 0  => Display BrickCode,Brick only
                //Case When: Bricks = 0 , Teams = 0  => Empty Grid with BrickCode, Brick column headers only ?
                if (result.Count != 0 && result != null && teamsIDs.Count != 0)  //Case When: Bricks > 0 , Teams > 0  => Display data 
                {
                    string[,] endresult = new string[result.Count(), result[0].ProfileCodes.Length + 2];
                    for (int i = 0; i < brickIDs.Count; i++)
                    {
                        endresult[i, 0] = result[i].BrickCode;
                    }
                    for (int i = 0; i < brickIDs.Count; i++)
                    {
                        endresult[i, 1] = result[i].BrickName;
                    }
                    for (int i = 0; i < brickIDs.Count; i++)
                    {
                        for (int j = 0; j < result[0].ProfileCodes.Length; j++)
                        {
                            endresult[i, j + 2] = result[i].ProfileCodes[j];
                        }
                    }
                    return new JsonResult()
                    {
                        Data = new
                        {
                            dataArray = endresult,
                            teamsList = teamsNames,
                            rowCount = brickIDs.Count,
                            columnCount = teamsIDs.Count + 2
                        },


                        JsonRequestBehavior = JsonRequestBehavior.AllowGet

                    };
                }
                else
                {


                    var brickNames = bricks.Select(b => b.Brick).ToList();
                    var brickCodes = bricks.Select(b => b.BrickCode).ToList();
                    return new JsonResult()
                    {
                        Data = new
                        {
                            bricksList = bricks,
                            brickCodesList = brickCodes,
                            teamsList = teamsNames,
                            rowCount = brickIDs.Count,
                            columnCount = teamsIDs.Count
                        },


                        JsonRequestBehavior = JsonRequestBehavior.AllowGet

                    };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[,] getReportDataExport(int? countryID, int? fromPeriodID, int? toPeriodID)
        {
            try
            {
                if (fromPeriodID == null)
                {
                    fromPeriodID = GetCurrentPeriod(countryID);
                }
                if(toPeriodID == null)
                {
                   toPeriodID =  GetCurrentPeriod(countryID);
                }
                int? LowestLevel = context.Countries.Where(c => c.CountryId == countryID).Select(c => c.MaxLevelsNumber).FirstOrDefault();
                //Get all SalesTeams for that country
                //var bricks = context.GeographicalAreas.Where(g => g.LevelsNo == LowestLevel & g.FK_CountryID == countryID).Select(g => new { g.GeographicalAreaID, g.GeographicalAreaName, g.GeographicalAreaCode }).ToList();
                var bricks = context.SP_TerritorytoBrickbyBrick_getBricks(countryID, fromPeriodID, toPeriodID, LowestLevel).ToList();
                List<int> brickIDs = new List<int>();
                foreach (var brick in bricks)
                {
                    brickIDs.Add(brick.BrickID);
                }
                var teamsIDs = (from s in context.SalesTeams
                                join t in context.SalesTeamTypes
                                on s.FK_SalesTeamTypeID equals t.SalesTeamTypeID
                                where t.FK_CountryId == countryID
                                select s.SalesTeamID).Distinct().ToList();
                var teamsNames = (from s in context.SalesTeams
                                  join t in context.SalesTeamTypes
                                  on s.FK_SalesTeamTypeID equals t.SalesTeamTypeID
                                  where t.FK_CountryId == countryID
                                  select s.SalesTeamName).Distinct().ToList();
                List<TerritoryToBrickByBrickVM> result = new List<TerritoryToBrickByBrickVM>();
                for (int i = 0; i < brickIDs.Count(); i++)
                {

                    TerritoryToBrickByBrickVM item = new TerritoryToBrickByBrickVM();
                    item.BrickCode = bricks[i].BrickCode;
                    item.BrickName = bricks[i].Brick;
                    item.ProfileCodes = new string[teamsIDs.Count];
                    item.ProfileCodes = context.SP_TerritoryBrickbyBrick_Helper(bricks[i].BrickID, countryID, fromPeriodID, toPeriodID).Select(p => p.ProfileCode).ToArray();
                    result.Add(item);
                }
                //Case When: Bricks > 0 , Teams > 0  => Display data 
                //Case When: Bricks = 0 , Teams > 0  => Display column headers only
                //Case When: Bricks > 0 , Teams = 0  => Display BrickCode,Brick only
                //Case When: Bricks = 0 , Teams = 0  => Empty Grid with BrickCode, Brick column headers only ?
                if (result.Count != 0 && result != null && teamsIDs.Count != 0)  //Case When: Bricks > 0 , Teams > 0  => Display data 
                {
                    string[,] endresult = new string[result.Count(), result[0].ProfileCodes.Length + 2];
                    for (int i = 0; i < brickIDs.Count; i++)
                    {
                        endresult[i, 0] = result[i].BrickCode;
                    }
                    for (int i = 0; i < brickIDs.Count; i++)
                    {
                        endresult[i, 1] = result[i].BrickName;
                    }
                    for (int i = 0; i < brickIDs.Count; i++)
                    {
                        for (int j = 0; j < result[0].ProfileCodes.Length; j++)
                        {
                            endresult[i, j + 2] = result[i].ProfileCodes[j];
                        }
                    }
                    return endresult;
                   
                }
                else
                {


                    var brickNames = bricks.Select(b => b.Brick).ToList();
                    var brickCodes = bricks.Select(b => b.BrickCode).ToList();
                    string[,] empty = new string[1,1];
                    empty[0,0] = "";
                    return empty;
                    
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
      
        
           
    }
}