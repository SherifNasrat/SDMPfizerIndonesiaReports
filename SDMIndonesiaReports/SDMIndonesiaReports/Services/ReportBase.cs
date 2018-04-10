using SDMIndonesiaReportsDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDMIndonesiaReports.Shared;

namespace SDMIndonesiaReports.Services
{
    public class ReportBase
    {

        private SDW_TargetingEntities _db = new SDW_TargetingEntities();
        public SDW_TargetingEntities context { get { return _db; } set { _db = value; } }
        public List<SelectListItem> countriesDLItems { set; get; }
        public List<SelectListItem> periodsDLItems { set; get; }
        public List<SelectListItem> provinceDLItems { set; get; }
        public List<SelectListItem> cityDLItems { set; get; }
        public List<SelectListItem> brickDLItems { set; get; }
        public List<SelectListItem> jobTitlesDLItems { set; get; }

        public ReportBase()
        {
            provinceDLItems = new List<SelectListItem>();
            countriesDLItems = new List<SelectListItem>();
            periodsDLItems = new List<SelectListItem>();
            cityDLItems = new List<SelectListItem>();
            brickDLItems = new List<SelectListItem>();
            jobTitlesDLItems = new List<SelectListItem>();
        }
        public List<SelectListItem> GetCountries()
        {
            List<Country> countries=new List<Country>();
            //if (SessionContainer.User.IsAdmin)
            //    countries = context.Countries.Where(c => c.SystemUserCountries.Where(d => d.FKSystemUserID = SessionContainer.User.UserID));
           var cIDs = context.SP_GetSystemUserCountries(SessionContainer.User.UserID).ToList();
            foreach (var id in cIDs)
            {
                countries.Add(context.Countries.Where(c => c.CountryId == id.Value).FirstOrDefault());
            }
            foreach (var c in countries.OrderBy(c => c.CountryName))
            {
                countriesDLItems.Add(new SelectListItem() { Text = c.CountryName.ToString(), Value = c.CountryId.ToString() });
            }
            return countriesDLItems;
        }
        public List<SelectListItem> GetJobTitles()
        {
            foreach (var c in context.Levels.OrderBy(c => c.LevelName))
            {
                jobTitlesDLItems.Add(new SelectListItem() { Text = c.LevelName.ToString(), Value = c.LevelId.ToString() });
            }
            return jobTitlesDLItems;
        }


        public List<SelectListItem> GetPeriods(int? countryID)
        {

            if (countryID != null)
            {
                var countryPeriodList = (from cp in context.CountryPeriods
                                         join p in context.Periods
                                         on cp.FK_PeriodID equals p.PeriodID
                                         where cp.FK_CountryID == countryID
                                         select p).OrderByDescending(p => p.DateFrom);


                foreach (var p in countryPeriodList)
                {
                    if (p.DateFrom <= DateTime.Now)
                        periodsDLItems.Add(new SelectListItem() { Text = p.PeriodCode.ToString() + "-" + p.PeriodName.ToString(), Value = p.PeriodID.ToString() });
                }

            }
            else
            {
                var countryPeriodList = context.Periods.OrderByDescending(p => p.DateFrom).Select(p => p);

                foreach (var p in countryPeriodList)
                {
                    periodsDLItems.Add(new SelectListItem() { Text = p.PeriodCode.ToString() + "-" + p.PeriodName.ToString(), Value = p.PeriodID.ToString() });
                }
            }

            return periodsDLItems;
        }


         public int? GetCurrentPeriod(int? countryid)
        {
            return (from cp in context.CountryPeriods
             join p in context.Periods
             on cp.FK_PeriodID equals p.PeriodID
             where cp.FK_CountryID == countryid && p.DateFrom <= DateTime.Now
             select p.PeriodID).OrderByDescending(p => p).FirstOrDefault();
        }
    }
}